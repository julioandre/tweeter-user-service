using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Protocol;
using user_service.Cache;
using user_service.Data;
using user_service.Models;
using user_service.Services;


namespace user_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userserviceController:ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly ILogger<userserviceController>_logger;
        private readonly IMapper _mapper;
        private IConfiguration _configuration;
        private IUserService _userService;
        private ApplicationDbContext _dbContext;
        private ICacheService _cacheService;
        private static string _jsonfilepath = "/Users/julioandre/RiderProjects/tweeter-clone/user-service/user-service/DummyData/user_mockData.json";

        public userserviceController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, ILogger<userserviceController> logger, IMapper mapper, IUserService userService,IConfiguration configuration, ApplicationDbContext dbContext, ICacheService cacheService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _dbContext = dbContext;
            _userService = userService;
            _cacheService = cacheService;
        }
        // From Body to prevent passing sensitive info in the url headers
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)
        {
            _logger.LogInformation($"Registeration request for {userDto.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = _mapper.Map<UserEntity>(userDto);
                user.UserName = userDto.Email;
                var result = await _userManager.CreateAsync(user,userDto.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
               
                return Ok(result);
            }
            catch (Exception e)
            {
                
                _logger.LogError(e, $"Error while registering user {userDto.Email}");
                return Problem($"Error while registering user {userDto.Email}",statusCode:500);
            }
        }

        // [HttpPost]
        // [Route("registerall")]
        // public async Task<IActionResult> RegisterAll()
        // {
        //     var _dummyData = ConvertingJson();
        //     //Console.WriteLine(_dummyData[0].ToJson());
        //     foreach (var users in _dummyData)
        //     {
        //         //Console.WriteLine(users);
        //         
        //         try
        //         {
        //             var user = _mapper.Map<UserEntity>(users);
        //             user.UserName = users.Email;
        //             var result = await _userManager.CreateAsync(user, users.Password);
        //             if (!result.Succeeded)
        //             {
        //                 Console.WriteLine(result.Errors);
        //             }
        //             
        //         }
        //         catch (Exception ex)
        //         {
        //             Console.WriteLine(ex.Message);
        //         }
        //    
        //     }
        //
        //     return Ok();
        // }

        [HttpPost, Authorize]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
            //Getting infor from JWT web token
            //var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation($"Login request for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _signInManager.PasswordSignInAsync(userDTO.Email, userDTO.Password, false, false);
                if (!result.Succeeded)
                {
                    return Unauthorized(userDTO);
                }

                var user = _userService.GetUser(userDTO.Email,includeCache:true);
                var token = GenerateToken(user);
                return Ok(token);
            }
            catch (Exception e)
            {
                
                _logger.LogError(e, $"Error while logging in  user {userDTO.Email}");
                return Problem($"Error while logging in  user {userDTO.Email}",statusCode:500);
            }
        }

        private string GenerateToken(UserEntity user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
                


            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims,expires: DateTime.Now.AddMinutes(25),signingCredentials:credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static List<UserDTO> ConvertingJson()
        {
            using StreamReader reader = new(_jsonfilepath);
            var json = reader.ReadToEnd();
            List<UserDTO> users = JsonConvert.DeserializeObject<List<UserDTO>>(json);
            return users;
        }



    }
}

