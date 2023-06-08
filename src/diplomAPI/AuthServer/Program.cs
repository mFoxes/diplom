using AuthServer;
using AuthServer.Auth;
using LdapConnector;
using Serilog;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000",
                                "http://localhost:5000")
                                .AllowAnyHeader()
                                .AllowCredentials()
                                .AllowAnyMethod();
                      });
});

builder.Logging.AddSerilog(logger);

builder.Services.AddIdentityServer()
    .AddInMemoryClients(IdentityServerConfig.Clients)
    .AddInMemoryApiResources(IdentityServerConfig.ApiResources)
    .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes)
    .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
    .AddResourceOwnerValidator<UserValidator>()
    .AddDeveloperSigningCredential();

builder.Services.AddOptions<LdapConfig>().Bind(configuration.GetSection("LdapConfiguration"));
builder.Services.AddScoped<ILdapRepository, LdapRepository>();
builder.Services.AddScoped<CredentialsValidator>();
var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
app.UseIdentityServer();
app.Run();