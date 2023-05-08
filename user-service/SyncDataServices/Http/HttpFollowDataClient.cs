using System.Text;
using Newtonsoft.Json;
using user_service.Dto;
using user_service.Models;

namespace user_service.SyncDataServices.Http;

public class HttpFollowDataClient:ICommandDataClient
{
    private readonly HttpClient _httpClient;

    public HttpFollowDataClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task SendUserToFollow(string user)
    {
        var httpContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8,"application/json");
        var response = await _httpClient.PostAsync("http://localhost:5269/api/follow/", httpContent);

        Console.WriteLine(response.IsSuccessStatusCode
            ? $"User {user} has been followed successfully!"
            : $"User {user} has not been followed successfully!");
    }

    
}