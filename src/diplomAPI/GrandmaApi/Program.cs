using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using GrandmaApi.Database;
using GrandmaApi.Extensions;
using LdapConnector;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using GrandmaApi.Mappers;
using GrandmaApi.Mediatr.Pipelines;
using GrandmaApi.Middlewares;
using GrandmaApi.Models.Configs;
using GrandmaApi.Notification;
using GrandmaApi.RabbitMqBus;
using Microsoft.OpenApi.Models;
using GrandmaApi.Localization;
using GrandmaApi.SignalR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using Quartz;
using Serilog;
using Singularis.Internal.Domain.Services.Database;
using Swashbuckle.AspNetCore.Filters;
using Singularis.Internal.Infrastructure.MongoDb;
using Singularis.Internal.Domain.Entities;
using GrandmaApi.Notification.MessageServices;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var mongoDbConfig = config.GetSection("MongoDbConfiguration").Get<MongoDbConfig>();
var logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

builder.Logging.AddSerilog(logger);
builder.Services.AddHttpClient();

builder.Services.AddOptions<LdapConfig>().Bind(config.GetSection("LdapConfiguration"));
builder.Services.AddOptions<ImagesConfig>().Bind(config.GetSection("ImagesConfig"));
builder.Services.AddOptions<RabbitMqConfig>().Bind(config.GetSection("RabbitMqConfig"));
builder.Services.AddOptions<ThresholdConfig>().Bind(config.GetSection("ThresholdConfig"));
builder.Services.AddOptions<SmtpClientConfig>().Bind(config.GetSection("SmtpClientConfig"));
builder.Services.AddOptions<MattermostConfig>().Bind(config.GetSection("MattermostConfig"));
builder.Services.AddOptions<EmailNotifierConfig>().Bind(config.GetSection("EmailNotifierConfig"));
builder.Services.AddOptions<LocalizationConfig>().Bind(config.GetSection("LocalizationConfig"));
builder.Services.AddOptions<MattermostNotifierConfig>().Bind(config.GetSection("MattermostNotifierConfig"));

builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.PropertyNamingPolicy = null;
        var enumConverter = new JsonStringEnumConverter(namingPolicy: JsonNamingPolicy.CamelCase);
        options.PayloadSerializerOptions.Converters.Add(enumConverter);
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ICollectionMapper, TypeNameCollectionMapper>();
builder.Services.AddSingleton<IDatabaseProvider, DatabaseProvider>(sp =>
{
    var connectionString = config.GetConnectionString("MongoDb");
    var readModelSource = mongoDbConfig.DatabaseName;
    var logger = sp.GetService<ILogger<DatabaseProvider>>();
    BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
    ConventionRegistry.Remove("__defaluts__");
    ConventionRegistry.Register(
        "__defaults__",
        DefaultConventionPack.Instance,
        t => typeof(IEntity<Guid>).IsAssignableFrom(t));
    ConventionRegistry.Register(
        "__dto__",
        DtoConventionPack.Instance,
        t => typeof(IEntity<Guid>).IsAssignableFrom(t));

    return new DatabaseProvider(
        connectionString,
        readModelSource,
        x =>
        {
            x.ClusterConfigurator = cc =>
            {
                cc.Subscribe<MongoDB.Driver.Core.Events.CommandStartedEvent>(e => logger.LogDebug($"{e.CommandName} - {e.Command.ToJson()}"));
            };
        }
    );
});
builder.Services.AddScoped<IGrandmaRepository>(sp =>
{
    return new GrandmaRepository(
        () => sp.GetRequiredService<IDatabaseProvider>().Database,
        sp.GetRequiredService<ICollectionMapper>(),
        new RepositorySettings(mongoDbConfig.IsolationLevel, TimeSpan.FromSeconds(mongoDbConfig.TransactionTimeout)));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000",
                                "http://localhost:5000")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});
builder.Services.AddScoped<ILdapRepository, LdapRepository>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddSingleton<TemplateProvider>();
builder.Services.AddScoped<INotifier, Notifier>();
builder.Services.AddScoped<IEmailMessageService, EmailService>();
builder.Services.AddScoped<IMattermostMessageService, MattermostService>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(HttpRequestsValidation<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(BrokerMessagesValidation<,>));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        var enumConverter = new JsonStringEnumConverter(namingPolicy: JsonNamingPolicy.CamelCase);
        options.JsonSerializerOptions.Converters.Add(enumConverter);
    });
builder.Services.AddHostedService<RabbitMqListener>();
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.AddJobAndTrigger<NotificationService>(config);
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddScoped<IRabbitMqMessageService, RabbitMqMessageService>();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Authority = config.GetConnectionString("AuthServer");
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });
var app = builder.Build();
app.UseMiddleware<PushCorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRouting();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(builder =>
{
    builder.WithOrigins(config.GetValue<string>("Origin"));
    builder.AllowAnyHeader();
    builder.WithMethods("GET", "POST");
    builder.AllowCredentials();
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<GrandmaHub>("/hubs/booking");
});

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Compose")
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            if (httpReq.Headers.ContainsKey("X-Forwarded-Host"))
            {
                var prefix = httpReq.Headers["X-Forwarded-Prefix"];
                var basePath = httpReq.Headers["X-Forwarded-Host"];
                var serverUrl = $"{httpReq.Scheme}://{basePath}/{prefix}";
                swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = serverUrl } };
            }
        });
    });
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("v1/swagger.json", "API v1");
    });
}

app.Run();