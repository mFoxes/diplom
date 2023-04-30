using AuthServer;
using AuthServer.Auth;
using LdapConnector;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

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

app.UseIdentityServer();

app.Run();