using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomerService.Controllers;
using CustomerService.Models;
using CustomerService.Services.Contracts;
using CustomerService.Validators.EntityValidators;
using CustomerService.Models.ModelDto;
using FluentValidation;

namespace CustomerService.Tests
{
    [TestFixture]
    public class PurchaseControllerTests
    {
        private Mock<IPurchaseService> _mockPurchaseService;
        private Mock<ICustomerService> _mockCustomerService;
        private Mock<IValidator<Purchase>> _mockPurchaseValidator;
        private PurchaseController _controller;
        private Mock<HttpContext> _mockHttpContext;

        [SetUp]
        public void Setup()
        {
            _mockPurchaseService = new Mock<IPurchaseService>();
            _mockCustomerService = new Mock<ICustomerService>();
            var httpClient = new System.Net.Http.HttpClient();

            // Create an instance of your custom PurchaseValidator
            var purchaseValidator = new PurchaseValidator(null, _mockPurchaseService.Object, null, null);

            // Controller setup
            _controller = new PurchaseController(_mockPurchaseService.Object, _mockCustomerService.Object, purchaseValidator, httpClient);

            // Mock HttpContext to simulate User Authentication
            _mockHttpContext = new Mock<HttpContext>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "1"),
        new Claim(ClaimTypes.Name, "user@example.com")
            }, "TestAuthentication"));
            _mockHttpContext.Setup(c => c.User).Returns(user);
            _controller.ControllerContext.HttpContext = _mockHttpContext.Object;
        }

        [Test]
        public async Task GetPurchases_ReturnsOkResult_WithListOfPurchases()
        {
            // Arrange
            var purchases = new List<Purchase> { new Purchase { Id = 1, CustomerId = 1, AgentId = 1, CampaignId = 3, Price = 200, Discount = 10, Date = DateTime.Today } };
            var purchaseReadDtos = purchases.Select(p => new PurchaseReadDto { CustomerId = p.CustomerId, AgentId = p.AgentId, CampaignId = p.CampaignId, Price = p.Price, Discount = p.Discount, Date = p.Date });

            _mockPurchaseService.Setup(service => service.GetAllPurchasesAsync())
                .Returns(Task.FromResult<IEnumerable<PurchaseReadDto>>(purchaseReadDtos));

            // Act
            var result = await _controller.GetPurchases();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedPurchases = okResult.Value as IEnumerable<PurchaseReadDto>;
            Assert.IsNotNull(returnedPurchases);

            // Compare each property of the first element in both collections
            var expectedPurchase = purchaseReadDtos.First();
            var actualPurchase = returnedPurchases.First();
            Assert.AreEqual(expectedPurchase.CustomerId, actualPurchase.CustomerId);
            Assert.AreEqual(expectedPurchase.AgentId, actualPurchase.AgentId);
            Assert.AreEqual(expectedPurchase.CampaignId, actualPurchase.CampaignId);
            Assert.AreEqual(expectedPurchase.Price, actualPurchase.Price);
            Assert.AreEqual(expectedPurchase.Discount, actualPurchase.Discount);
            Assert.AreEqual(expectedPurchase.Date, actualPurchase.Date);
        }

        [Test]
        public async Task GetPurchaseById_ReturnsNotFound_WhenPurchaseDoesNotExist()
        {
            // Arrange
            _mockPurchaseService.Setup(s => s.GetPurchaseByIdAsync(It.IsAny<int>())).Throws(new KeyNotFoundException());

            // Act
            var result = await _controller.GetPurchaseById(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task DeletePurchase_ReturnsNoContent_WhenDeletedSuccessfully()
        {
            // Arrange
            _mockPurchaseService.Setup(s => s.DeletePurchaseAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePurchase(1);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
