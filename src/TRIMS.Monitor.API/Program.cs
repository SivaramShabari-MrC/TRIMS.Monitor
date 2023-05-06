using Microsoft.AspNetCore.Authentication.JwtBearer;
using TRIMS.Monitor.Manager;
using TRIMS.Monitor.Service;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Repository;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

var AppSettings = builder.Configuration.GetSection(AppConfig.ApplicationConfig).Get<ApplicationConfig>();
var AuthSettings = builder.Configuration.GetSection(AppConfig.AuthenticationConfig).Get<AuthenticationConfig>();
var AzureAdSettings = builder.Configuration.GetSection(AppConfig.AzureAd).Get<AzureAdConfig>();


//
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(AppSettings.Version, new OpenApiInfo
    {
        Title = AppSettings.Title,
        Version = AppSettings.Version,
        Description = AppSettings.Description,
        Contact = new OpenApiContact
        {
            Name = AppSettings.Contact,
            Email = AppSettings.ContactEmail
        }
    });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows()
        {
            Implicit = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri(AuthSettings.AuthorizationUrl),
                TokenUrl = new Uri(AuthSettings.TokenUrl),
                Scopes = new Dictionary<string, string>
                            {
                                { AuthSettings.Scopes, AuthSettings.Description }
                            }
            }
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
                Scheme = "oauth2",
                Name = "oauth2",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddSingleton(builder.Configuration.Get<AppSettingsConfig>());

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSingleton<IFileMonitorThreadManager, FileMonitorThreadManager>();
builder.Services.AddSingleton<IFileMonitorThreadService, FileMonitorThreadService>();
builder.Services.AddSingleton<IBAIFileStatusManager, BAIFileStatusManager>();
builder.Services.AddSingleton<IBAIFileStatusRepository, BAIFileStatusRepository>();
builder.Services.AddSingleton<ITransactionManager, TransactionManager>();
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
builder.Services.AddSingleton<IScheduledTaskService, ScheduledTaskService>();
builder.Services.AddSingleton<IScheduledTaskManager, ScheduledTaskManager>();
builder.Services.AddSingleton<IContactApiService, ContactApiService>();
builder.Services.AddSingleton<ISecurityAuditRepository, SecurityAuditRepository>();

builder.Services.AddCors(o => o.AddPolicy("AllowAll",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                      }));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"{AppSettings.Version}/swagger.json", AppSettings.Title);
        c.EnableValidator(null);
        c.OAuthClientId(AzureAdSettings.ClientId);
        c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
    });
    IdentityModelEventSource.ShowPII = true;
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();


app.Run();
