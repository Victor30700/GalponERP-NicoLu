using GalponERP.Application.Interfaces;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GalponERP.Infrastructure.Authentication;

public class FirebaseAuthService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly FirestoreDb? _firestoreDb;
    private readonly ILogger<FirebaseAuthService> _logger;

    public FirebaseAuthService(
        IHttpContextAccessor httpContextAccessor, 
        IConfiguration configuration, 
        IWebHostEnvironment env,
        ILogger<FirebaseAuthService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        
        var projectId = configuration["Firebase:ProjectId"] ?? "galpon-erp-default";
        
        try 
        {
            // Buscar ruta de credenciales en configuración o usar ContentRootPath por defecto
            var credentialPath = configuration["Firebase:CredentialPath"];
            
            if (string.IsNullOrEmpty(credentialPath))
            {
                credentialPath = Path.Combine(env.ContentRootPath, "firebase-admin.json");
            }

            if (File.Exists(credentialPath))
            {
                _logger.LogInformation("Cargando credenciales de Firebase desde: {Path}", credentialPath);
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
                _firestoreDb = new FirestoreDbBuilder
                {
                    ProjectId = projectId,
                    Credential = GoogleCredential.GetApplicationDefault()
                }.Build();
            }
            else
            {
                _logger.LogCritical("FALTA ARCHIVO DE CONFIGURACIÓN: No se encontró 'firebase-admin.json' en {Path}. Las funciones de autenticación y Firestore fallarán.", credentialPath);
                // Permitimos que la app arranque, pero Firestore será nulo o fallará al usarse
                _firestoreDb = null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error crítico al inicializar Firebase Service.");
            _firestoreDb = null;
        }
    }

    public Task<string?> GetUserIdAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // En Firebase, a veces se usa un claim personalizado 'user_id' si 'NameIdentifier' no está disponible
        if (string.IsNullOrEmpty(userId))
        {
            userId = _httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value;
        }

        return Task.FromResult(userId);
    }

    public Task<string?> GetUserEmailAsync()
    {
        var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        return Task.FromResult(email);
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public async Task<string> CreateUserAsync(string email, string password, string displayName, IDictionary<string, object>? extraUserData = null)
    {
        if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
        {
            throw new InvalidOperationException("Firebase Admin SDK no ha sido inicializado. Verifique el archivo firebase-admin.json.");
        }

        if (_firestoreDb == null)
        {
            throw new InvalidOperationException("Firestore no ha sido inicializado. Verifique el archivo firebase-admin.json.");
        }

        var userArgs = new UserRecordArgs
        {
            Email = email,
            Password = password,
            DisplayName = displayName,
            EmailVerified = false,
            Disabled = false,
        };

        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);

        // Guardar datos adicionales en la colección 'users' de Firestore
        var userDocRef = _firestoreDb.Collection("users").Document(userRecord.Uid);
        
        var userData = new Dictionary<string, object>
        {
            { "uid", userRecord.Uid },
            { "email", email },
            { "displayName", displayName },
            { "createdAt", Timestamp.GetCurrentTimestamp() }
        };

        if (extraUserData != null)
        {
            foreach (var kvp in extraUserData)
            {
                userData[kvp.Key] = kvp.Value;
            }
        }

        await userDocRef.SetAsync(userData);

        return userRecord.Uid;
    }

    public async Task<string?> GetUidByEmailAsync(string email)
    {
        if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
        {
            _logger.LogWarning("Intentando usar Firebase Auth sin inicialización.");
            return null;
        }

        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
            return userRecord.Uid;
        }
        catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
        {
            return null;
        }
    }
}
