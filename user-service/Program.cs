using System.Text;
using KafkaFlow;
using KafkaFlow.Serializer;
using KafkaFlow.TypedHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using user_service.Data;
using Microsoft.IdentityModel.Tokens;
using user_service.Cache;
using user_service.Messaging;
using user_service.Models;
using user_service.Services;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
const string topicName = "followServiceTopic";
builder.Services.AddControllers();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IHostedService, Consumers>();
builder.Services.AddScoped<IProducers, Producers>();
//Logging to console
builder.Services.AddLogging(configure => configure.AddConsole());
// builder.Services.AddKafkaFlowHostedService(kafka => kafka.UseMicrosoftLog().AddCluster(cluster =>
// {
//     cluster.WithBrokers(new[] { "localhost:9092" })
//         .AddConsumer(consumer => consumer.Topic(topicName)
//             .WithGroupId("test-Kafka")
//             .WithBufferSize(100)
//             .WithWorkersCount(3)
//             .WithAutoOffsetReset(AutoOffsetReset.Earliest)
//             .AddMiddlewares(middleware => middleware.AddSerializer<JsonCoreSerializer>().AddTypedHandlers(handlers=> handlers.AddHandler<TaskHandler1>())));
// }));

// var provider = builder.Services.BuildServiceProvider();
// var bus = provider.CreateKafkaBus();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("Jwt")["Issuer"],
        ValidAudience = builder.Configuration.GetSection("Jwt")["Audience"],
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt")["Key"]))

    };
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential 
    // cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.ConsentCookieValue = "true";
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    
        options.Configuration = "localhost:6378";
        options.InstanceName = "user-redis-cache";
   
});
builder.Services.AddDefaultIdentity<UserEntity>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCookiePolicy();
app.MapControllers();
app.MapRazorPages();
app.Run();
// await bus.StartAsync();
// Console.WriteLine("Press Key to exit...");
// Console.ReadKey();