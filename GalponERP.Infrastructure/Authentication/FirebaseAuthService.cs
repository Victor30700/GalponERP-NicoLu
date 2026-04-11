using GalponERP.Application.Interfaces;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;

namespace GalponERP.Infrastructure.Authentication;

public class FirebaseAuthService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly FirestoreDb _firestoreDb;

    public FirebaseAuthService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment env)
    {
        _httpContextAccessor = httpContextAccessor;
        
        var projectId = configuration["Firebase:ProjectId"] ?? "galpon-erp-default";
        
        // Intentar encontrar el archivo en varios lugares posibles
        var pathsToTry = new[] 
        {
            Path.Combine(env.ContentRootPath, "firebase-admin.json"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-admin.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "firebase-admin.json"),
            // Ruta específica para tu entorno local si las anteriores fallan
            "D:\\scripts-csharp\\Pollos_NicoLu\\Pollos-NicoLu\\GalponERP.Api\\firebase-admin.json"
        };

        string? credentialPath = pathsToTry.FirstOrDefault(File.Exists);
        
        if (credentialPath != null)
        {
            Console.WriteLine($"[FirebaseService] Credenciales cargadas desde: {credentialPath}");
            _firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                Credential = GoogleCredential.FromFile(credentialPath)
            }.Build();
        }
        else
        {
            Console.WriteLine("[FirebaseService] ¡ERROR CRÍTICO! No se encontró firebase-admin.json en ninguna de las rutas intentadas:");
            foreach(var p in pathsToTry) Console.WriteLine($" - {p}");
            
            _firestoreDb = FirestoreDb.Create(projectId);
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
