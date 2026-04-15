using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi;
using ValoInfo.Application.Interfaces;
using ValoInfo.Infrastructure.Repositories;
using ValoInfo.Api.Middleware;
using DotNetEnv;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

// Add services to the container.
// Add CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Registrar Firestore
builder.Services.AddSingleton(provider =>
{
    var base64 = Environment.GetEnvironmentVariable("FIRESTORE_CREDENTIALS_BASE64") ?? throw new Exception("Falta variable de entorno");
    var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));

    var credential = GoogleCredential.FromJson(json);

    return new FirestoreDbBuilder
    {
        ProjectId = "valo-info",
        Credential = credential
    }.Build();
});

// DI Repository
builder.Services.AddScoped<IAgentRepository, FirestoreAgentRepository>();

// Configurar versionamiento de API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); // versión por defecto
    options.AssumeDefaultVersionWhenUnspecified = true; // si no especifica versión, usa la default
    options.ReportApiVersions = true; // agrega encabezados con info de versiones
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // versionar por URL (/v1/...)
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // genera grupos v1, v2, ...
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var apiVersionProvider = builder.Services.BuildServiceProvider()
                        .GetRequiredService<IApiVersionDescriptionProvider>();

builder.Services.AddSwaggerGen(options =>
{
    foreach (var description in apiVersionProvider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = $"ValoInfo API {description.ApiVersion}",
            Version = description.ApiVersion.ToString()
        });
    }
});

var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
