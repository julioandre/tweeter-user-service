using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
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
            var token = new JwtSecurityToken(_configuration["JwtIssuer"], _configuration["JwtAudience"], claims,expires: DateTime.Now.AddMinutes(25),signingCredentials:credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // private UserEntity GetUser(string username, bool includeCache=false)
        // {
        //     UserEntity user;
        //     if (includeCache)
        //     {
        //         try
        //         {
        //             user = _cacheService.GetData<UserEntity>(username);
        //             if (user != null)
        //             {
        //                 return user;
        //             }
        //         
        //         }
        //         catch (Exception exception)
        //         {
        //         
        //         }
        //     }
        //    
        //     user = _dbContext.Users.FirstOrDefault(u => u.UserName == username);
        //     
        //    if (user != null)
        //    {
        //        try
        //        {
        //            _cacheService.SetData(user.UserName, user, TimeSpan.FromDays(2));
        //        }
        //        catch (Exception e)
        //        {
        //            
        //        }
        //       
        //
        //    }
        //
        //    return user;
        // }



    }
}

