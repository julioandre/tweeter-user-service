using Microsoft.AspNetCore.Identity;

namespace user_service.Models;

public class UserEntity:IdentityUser
{
   
    public string FirstName { get; set; }
    public string LastName { get; set; }
   
}