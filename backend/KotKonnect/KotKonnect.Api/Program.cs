using System.Text;
using System.Text.Json.Serialization;
using KotKonnect.Api.EndPoints;
using KotKonnect.Api.Middleware;
using KotKonnect.Api.Security;
using KotKonnect.Core;
using KotKonnect.Core.IGateways;
using KotKonnect.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Enums sérialisés en chaînes ("PUBLIE"...) -> contrat frontend préservé.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddOpenApi();

builder.Services.AddCoreServices();                                // UseCases (logique métier)
builder.Services.AddInfrastructureServices(builder.Configuration); // repos + gateways + sécurité + MySQL

// JWT : préoccupation d'authentification web (génération + validation), gérée ici dans l'Api.
// La section "Jwt" est lue une seule fois et réutilisée pour les deux usages.
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Section 'Jwt' manquante.");
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS pour le frontend Angular.
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Gestion d'erreurs centralisée (enveloppe tout le pipeline).
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Angular");
app.UseAuthentication();
app.UseAuthorization();

// Endpoints par feature.
app.AddAuthRoutes();
app.AddBienRoutes();
app.AddCandidatureRoutes();
app.AddProfilRoutes();

app.Run();
