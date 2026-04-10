using GalponERP.Api.Middleware;
using GalponERP.Api.BackgroundJobs;
using GalponERP.Application;
using GalponERP.Infrastructure;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Inicializar Firebase Admin SDK
var firebaseConfigPath = Path.Combine(builder.Environment.ContentRootPath, "firebase-admin.json");
if (File.Exists(firebaseConfigPath))
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(firebaseConfigPath)
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
    });

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<AlertaInventarioJob>();

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
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seeding Automático del Admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<GalponERP.Infrastructure.Persistence.GalponDbContext>();
        var adminUid = "utq0GMrQZESnNsyQWUEFOV5fKf23";
        var existingAdmin = context.Usuarios.FirstOrDefault(u => u.FirebaseUid == adminUid);
        
        if (existingAdmin == null)
        {
            var admin = new GalponERP.Domain.Entities.Usuario(Guid.NewGuid(), adminUid, "Admin Maestro", "Admin");
            context.Usuarios.Add(admin);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Un error ocurrió durante el seeding del Administrador.");
    }
}

app.Run();
