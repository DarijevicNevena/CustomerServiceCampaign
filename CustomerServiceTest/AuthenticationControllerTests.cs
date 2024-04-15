using CustomerService.Controllers;
using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CustomerService.Tests
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private Mock<IAuthenticationService> _mockAuthenticationService;
        private AuthenticationController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _controller = new AuthenticationController(_mockAuthenticationService.Object);
        }


        [Test]
        public async Task Login_ReturnsNotFound_WhenAgentNotFound()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "nonexistent@example.com", Password = "password" };
            _mockAuthenticationService.Setup(service => service.AuthenticateAgent(loginDto.Email, loginDto.Password)).Throws(new KeyNotFoundException());

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            Assert.AreEqual("Agent not found.", notFoundResult.Value);
        }

        [Test]
        public async Task Login_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", Password = "password" };
            _mockAuthenticationService.Setup(service => service.AuthenticateAgent(loginDto.Email, loginDto.Password)).Throws(new Exception());

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }
    }
}
