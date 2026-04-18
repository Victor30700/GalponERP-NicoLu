using GalponERP.Api.Middleware;
using GalponERP.Api.BackgroundJobs;
using GalponERP.Application;
using GalponERP.Application.Interfaces;
using GalponERP.Infrastructure;
using GalponERP.Infrastructure.Authentication;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Inicializar Firebase Admin SDK
var firebaseConfigPath = Path.Combine(builder.Environment.ContentRootPath, "firebase-admin.json");
if (File.Exists(firebaseConfigPath))
{
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseConfigPath);
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.GetApplicationDefault()
    });
}

// Configurar Seguridad (Firebase JWT)
var projectId = builder.Configuration["Firebase:ProjectId"] ?? "galpon-erp-default"; // Fallback para desarrollo si no está en appsettings
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{projectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{projectId}",
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var firebaseUid = context.Principal?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
                if (string.IsNullOrEmpty(firebaseUid)) return;

                // Obtener el ICurrentUserContext de la solicitud actual
                var currentUserContext = context.HttpContext.RequestServices.GetRequiredService<ICurrentUserContext>() as CurrentUserContext;
                var usuarioRepository = context.HttpContext.RequestServices.GetRequiredService<GalponERP.Domain.Interfaces.Repositories.IUsuarioRepository>();
                
                var usuario = await usuarioRepository.ObtenerPorFirebaseUidAsync(firebaseUid);

                if (usuario != null)
                {
                    // Si el usuario está desactivado por el administrador (Active = 0)
                    // o eliminado físicamente (aunque aquí ya lo filtraría el IsActive del Repo)
                    if (usuario.Active == 0)
                    {
                        context.Fail("El usuario no está activo en el sistema.");
                        return;
                    }

                    currentUserContext?.SetUser(usuario.Id, firebaseUid);
                    
                    var claims = new List<System.Security.Claims.Claim>
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, usuario.Rol.ToString())
                    };
                    var appIdentity = new System.Security.Claims.ClaimsIdentity(claims);
                    context.Principal?.AddIdentity(appIdentity);
                }
                else
                {
                    // Si el usuario no existe en la DB local (o está soft-deleted)
                    context.Fail("Usuario no registrado o inactivo.");
                }
            }
        };
    });

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000", "https://localhost:3000", "https://127.0.0.1:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddHostedService<AlertaInventarioJob>();
builder.Services.AddHostedService<AlertaSanitariaJob>();
builder.Services.AddHostedService<AnalisisDatosJob>();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GalponERP API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Incluir comentarios XML
    var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
    if (File.Exists(apiXmlPath)) c.IncludeXmlComments(apiXmlPath);

    var appXmlFile = "GalponERP.Application.xml";
    var appXmlPath = Path.Combine(AppContext.BaseDirectory, appXmlFile);
    if (File.Exists(appXmlPath)) c.IncludeXmlComments(appXmlPath);
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GalponERP API V1");
    c.RoutePrefix = "swagger"; // La interfaz estará en /swagger
});

// Redireccionar raíz a Swagger para mayor comodidad
app.MapGet("/", context => 
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseExceptionHandler();
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seeding Automático del Admin y Catálogos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Seeder Automático para Catálogos Base (Categorías y Unidades)
        await GalponERP.Infrastructure.Persistence.GalponDbSeeder.SeedAsync(services);

        var context = services.GetRequiredService<GalponERP.Infrastructure.Persistence.GalponDbContext>();
        var adminUid = "utq0GMrQZESnNsyQWUEFOV5fKf23";
        var existingAdmin = await context.Usuarios.FirstOrDefaultAsync(u => u.FirebaseUid == adminUid);
        
        if (existingAdmin == null)
        {
            var admin = new GalponERP.Domain.Entities.Usuario(
                Guid.NewGuid(), 
                adminUid, 
                "admin@galponerp.com",
                "Admin Maestro", 
                "Principal", 
                new DateTime(1980, 1, 1), 
                "Dirección del Galpón", 
                "Gerente", 
                "0000000000",
                GalponERP.Domain.Entities.RolGalpon.Admin);
            context.Usuarios.Add(admin);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Un error ocurrió durante el seeding de la base de datos.");
    }
}

// Mensaje explícito en consola para encontrar la URL fácilmente
var loggerStart = app.Services.GetRequiredService<ILogger<Program>>();
var urls = string.Join(", ", app.Urls);
loggerStart.LogInformation("=================================================");
loggerStart.LogInformation("SERVIDOR GALPON ERP INICIADO CORRECTAMENTE");
loggerStart.LogInformation("URL DE LA API: http://localhost:5167/swagger");
loggerStart.LogInformation("=================================================");

app.Run();
