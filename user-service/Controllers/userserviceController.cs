using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using user_service.Models;


/// Remove email frrom errors
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

        public userserviceController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, ILogger<userserviceController> logger, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
        }
        // From Body to prevent passing sensitive info in the url headers
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
                return Ok(result);
            }
            catch (Exception e)
            {
                
                _logger.LogError(e, $"Error while logging in  user {userDTO.Email}");
                return Problem($"Error while logging in  user {userDTO.Email}",statusCode:500);
            }
        }



    }
}

