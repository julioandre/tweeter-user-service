using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using user_service.Controllers;
using user_service.Models;
using user_service.Services;

namespace userControllerUnitTest;

public class userControllerUnitTest
{
    //private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly ILogger<userserviceController>_logger;
    private readonly IMapper _mapper;
    private readonly  IConfiguration _configuration;
    private readonly IUserService _service;

    public userControllerUnitTest()
    {
        
        _signInManager = A.Fake<SignInManager<UserEntity>>();
        _logger = A.Fake<Logger<userserviceController>>();
        _mapper = A.Fake<IMapper>();
        _configuration = A.Fake<IConfiguration>();
        _service = A.Fake<IUserService>();
    }
    
    [Fact]
    public async Task Register_ReturnsOK()
    {
        var _userManager = new Mock<UserManager<UserEntity>>(
            new Mock<IUserStore<UserEntity>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<UserEntity>>().Object,
            new IUserValidator<UserEntity>[0],
            new IPasswordValidator<UserEntity>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<UserEntity>>>().Object);
        _userManager
            .Setup(userManager => userManager.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .Returns(Task.FromResult(IdentityResult.Success));
        _userManager
            .Setup(userManager => userManager.AddToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()));
        var user = A.Fake<UserDTO>();
        var userEnt = A.Fake<UserEntity>();
        //A.CallTo(() => _mapper.Map<UserEntity>(userEnt)).Returns(user);
        var controller =
            new userserviceController(_userManager.Object, _signInManager, _logger, _mapper, _service, _configuration);
        
        var result = (RedirectToActionResult) await controller.Register(user);
        Assert.Equal("Index",result.ActionName);
        
    }
}