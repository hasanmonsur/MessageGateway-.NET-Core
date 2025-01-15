using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using Serilog;
using SmsGatewaySystem.Repository;
using SmsGatewaySystem.Contracts;
using SmsGatewaySystem.Authorization;
using SmsGatewaySystem.Helpers;
using SmsGatewaySystem.Data;
using MassTransit;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args) ;
//builder.Host.UseSerilog();

builder.Host.UseSerilog((hostingContext, loggerConfig) =>
                    loggerConfig.ReadFrom.Configuration(hostingContext.Configuration)
                );


builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});


ConfigurationManager configuration = builder.Configuration;

//LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
//LoggerManager logger = new LoggerManager();
//builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
// configure DI for application services
//builder.Services.AddScoped<IJwtUtils, JwtUtils>();
// Add services to the container.
builder.Services.AddSingleton<DapperContext>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ISmsRepository, SmsRepository>();
builder.Services.AddScoped<ISmsQueueHelper, SmsQueueHelper>();
builder.Services.AddScoped<ICommHelper, CommHelper>();
//builder.Services.AddScoped<IMapper, Mapper>();


//builder.Services.AddCors();


// Add services to the container.
builder.Services.AddMemoryCache();

// Access configuration values directly
var rmQhost = builder.Configuration["RabbitMQ:Host"];
var urmQsername = builder.Configuration["RabbitMQ:Username"];
var rmQpassword = builder.Configuration["RabbitMQ:Password"];

// MassTransit Configuration
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SmsGPConsumer>();
    x.AddConsumer<SmsRBConsumer>();
    x.AddConsumer<SmsBLConsumer>();
    x.AddConsumer<SmsTTConsumer>();
    x.AddConsumer<OtpGPConsumer>();
    x.AddConsumer<OtpRBConsumer>();
    x.AddConsumer<OtpBLConsumer>();
    x.AddConsumer<OtpTTConsumer>();


    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rmQhost, h =>
        {
            h.Username(urmQsername);
            h.Password(rmQpassword);
        });

        // Dynamically create queue for SMS service
        cfg.ReceiveEndpoint(QueueNameGenerator.GetQueueName("sms-gp-service"), e =>
        {
            e.ConfigureConsumer<SmsGPConsumer>(context);
        });

        cfg.ReceiveEndpoint(QueueNameGenerator.GetQueueName("sms-rb-service"), e =>
        {
            e.ConfigureConsumer<SmsRBConsumer>(context);
        });

        cfg.ReceiveEndpoint(QueueNameGenerator.GetQueueName("sms-bl-service"), e =>
        {
            e.ConfigureConsumer<SmsBLConsumer>(context);
        });

        cfg.ReceiveEndpoint(QueueNameGenerator.GetQueueName("sms-tt-service"), e =>
        {
            e.ConfigureConsumer<SmsTTConsumer>(context);
        });



        // Dynamically create queue for Email service
        cfg.ReceiveEndpoint(QueueNameGenerator.GetQueueName("otp-gp-service"), e =>
        {
            e.ConfigureConsumer<OtpGPConsumer>(context);
        });

        cfg.ReceiveEndpoint(QueueNameGenerator.GetQueueName("otp-rb-service"), e =>
        {
            e.ConfigureConsumer<OtpRBConsumer>(context);
        });

        cfg.ReceiveEndpoint(QueueNameGenerator.GetQueueName("otp-bl-service"), e =>
        {
            e.ConfigureConsumer<OtpBLConsumer>(context);
        });

        cfg.ReceiveEndpoint(QueueNameGenerator.GetQueueName("otp-tt-service"), e =>
        {
            e.ConfigureConsumer<OtpTTConsumer>(context);
        });


    });
});



builder.Services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
                ("BasicAuthentication", null);



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OTP API",
        Version = "v1",
        Description = "SBL SMS MW API Services.",
        Contact = new OpenApiContact
        {
            Name = "Sonali Bank Limited"
        },
    });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Description = "Basic auth added to authorization header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "basic",
        Type = SecuritySchemeType.Http
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Basic" }
                    },
                    new List<string>()
                }
            });
});

builder.Services.AddAuthorization();

builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); 
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
Console.WriteLine("Application has started running...");


// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MapperProfile));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.UseCors(x => x
//                .AllowAnyOrigin()
//                .AllowAnyMethod()
//                .AllowAnyHeader());

app.MapControllers();

app.Run();
