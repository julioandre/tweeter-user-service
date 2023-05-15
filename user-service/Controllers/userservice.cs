using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using user_service.Models;

namespace user_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userservice:ControllerBase
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly ILogger<userservice>_logger;
        private readonly IMapper _mapper;

        public userservice(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, ILogger<userservice> logger, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpPost]
        // From Body to prevent passing sensitive info in the url headers
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation($"Registeration request for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = _mapper.Map<UserEntity>(userDTO);
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                
                _logger.LogError(e, $"Error while registering user {userDTO.Email}");
                return Problem($"Error while registering user {userDTO.Email}",statusCode:500);
            }
        }



    }
}

