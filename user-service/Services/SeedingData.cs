using System.Text.Json.Nodes;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using user_service.Data;
using user_service.Models;

namespace user_service.Services;

public class SeedingData
{
    
    
    
    private static string _jsonfilepath = "/Users/julioandre/RiderProjects/tweeter-clone/user-service/user-service/DummyData/user_mockData.json";
   

    public static void InitializeData(WebApplication app)
    {
        
            using (var serviceScope = app.Services.CreateScope())
            {
                
                SeedData(serviceScope.ServiceProvider.GetService<ApplicationDbContext>(),serviceScope.ServiceProvider.GetService<UserManager<UserEntity>>() ,serviceScope.ServiceProvider.GetService<IMapper>());
            }
    }

    public static List<UserDTO> ConvertingJson()
    {
        using StreamReader reader = new(_jsonfilepath);
        var json = reader.ReadToEnd();
        List<UserDTO> users = JsonConvert.DeserializeObject<List<UserDTO>>(json);
        return users;
    }

    public static void SeedData(ApplicationDbContext context, UserManager<UserEntity> userManager,IMapper mapper)
    {
        
        var _dummyData = ConvertingJson();
        foreach (var users in _dummyData)
        {
            
            try
            {
                var user = mapper.Map<UserEntity>(users);
                user.UserName = users.Email;
                var result = userManager.CreateAsync(user, users.Password);
                if (result.Result.Succeeded)
                {
                    Console.WriteLine("I Succeded");
                }
                else
                {
                    Console.WriteLine("I didnt Succeded");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }
    }
}