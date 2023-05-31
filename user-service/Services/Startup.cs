// using Microsoft.EntityFrameworkCore;
// using user_service.Data;
// using user_service.Models;
//
// namespace user_service.Services;
//
// // File to register services in Program.cs
// public class Startup
// {
//     public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
//     {
//         services.AddControllers();
//         services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(configuration.GetConnectionString("user-db-connection")));
//         services.Configure<CookiePolicyOptions>(options =>
//         {
//             // This lambda determines whether user consent for non-essential 
//             // cookies is needed for a given request.
//             options.CheckConsentNeeded = context => true;
//             options.ConsentCookieValue = "true";
//             options.MinimumSameSitePolicy = SameSiteMode.None;
//         });
//         services.AddDefaultIdentity<UserEntity>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//         services.AddEndpointsApiExplorer();
//         services.AddSwaggerGen();
//         return services;
//     }
//
//     public static IApplicationBuilder AddApplication(this IApplicationBuilder app)
//     {
//        
//
//         app.UseHttpsRedirection();
//
//         app.UseAuthorization();
//         app.UseCookiePolicy();
//         return app;
//     }
// }