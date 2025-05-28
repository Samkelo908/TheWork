using AgriEnergyConnect.API.Controllers;
using AgriEnergyConnect.API.Models;
using AgriEnergyConnect.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace AgriEnergyConnect.API.Tests.Controllers
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _mockAuthService;
        private AuthController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [TestMethod]
        public async Task Login_ReturnsOk_WithValidCredentials()
        {
            // Arrange
            var model = new LoginModel { Email = "test@test.com", Password = "password" };
            var expectedResponse = new AuthResponse { Token = "token", UserId = "1" };

            _mockAuthService.Setup(x => x.LoginAsync(model))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(expectedResponse, okResult.Value);
        }

        [TestMethod]
        public async Task Login_ReturnsUnauthorized_WithInvalidCredentials()
        {
            // Arrange
            var model = new LoginModel { Email = "test@test.com", Password = "wrong" };

            _mockAuthService.Setup(x => x.LoginAsync(model))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public async Task Register_ReturnsOk_WithValidModel()
        {
            // Arrange
            var model = new RegisterModel { Email = "test@test.com", Password = "password", Role = "Farmer" };

            _mockAuthService.Setup(x => x.RegisterAsync(model))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Register(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var model = new RegisterModel { Email = "test@test.com", Password = "password", Role = "Farmer" };

            _mockAuthService.Setup(x => x.RegisterAsync(model))
                .ThrowsAsync(new Exception("Registration failed"));

            // Act
            var result = await _controller.Register(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}