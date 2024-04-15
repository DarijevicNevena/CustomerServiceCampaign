using Microsoft.AspNetCore.Mvc;
using Moq;
using CustomerService.Controllers;
using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;

namespace CustomerServiceTest
{
    [TestFixture]
    public class CampaignControllerTests
    {
        private Mock<ICampaignService> _mockCampaignService;
        private Mock<ICampaignReportService> _mockReportService;
        private CampaignController _controller;

        [SetUp]
        public void Setup()
        {
            _mockCampaignService = new Mock<ICampaignService>();
            _mockReportService = new Mock<ICampaignReportService>();
            _controller = new CampaignController(_mockCampaignService.Object, _mockReportService.Object);
        }

        [Test]
        public async Task GetCampaigns_ReturnsOkObjectResult_WithListOfCampaigns()
        {
            // Arrange
            var mockCampaigns = new List<CampaignReadDto> { new CampaignReadDto { CampaignName = "New Campaign" } };
            _mockCampaignService.Setup(x => x.GetAllCampaignsAsync()).ReturnsAsync(mockCampaigns);

            // Act
            var result = await _controller.GetCampaigns();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<List<CampaignReadDto>>());
            var returnedCampaigns = okResult.Value as List<CampaignReadDto>;
            Assert.That(returnedCampaigns, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task GetCampaignById_NotFoundResult_WhenCampaignDoesNotExist()
        {
            // Arrange
            _mockCampaignService.Setup(x => x.GetCampaignByIdAsync(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetCampaignById(0);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateCampaign_ReturnsBadRequest_WhenCampaignIsNull()
        {
            // Act
            var result = await _controller.CreateCampaign(null);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateCampaign_ReturnsCreatedResponse_WhenCampaignIsCreated()
        {
            // Arrange
            var campaignDto = new CampaignWriteDto { CampaignName = "Valid Campaign" };
            var createdCampaign = new CampaignReadDto { CampaignName = "Valid Campaign" };
            _mockCampaignService.Setup(x => x.CreateNewCampaignAsync(campaignDto)).ReturnsAsync(createdCampaign);

            // Act
            var result = await _controller.CreateCampaign(campaignDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<CampaignReadDto>());
            var returnedCampaign = okResult.Value as CampaignReadDto;
            Assert.That(returnedCampaign.CampaignName, Is.EqualTo("Valid Campaign"));
        }

        [Test]
        public async Task DeleteCampaign_ReturnsNoContent_WhenDeletedSuccessfully()
        {
            // Arrange
            _mockCampaignService.Setup(x => x.DeleteCampaignAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCampaign(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task ExportPurchasesToCsv_ReturnsFile_WhenDataIsAvailable()
        {
            // Arrange
            string csvContent = "test,csv,content";
            _mockReportService.Setup(x => x.GenerateCampaignPurchasesReport(It.IsAny<int>())).ReturnsAsync(csvContent);

            // Act
            var result = await _controller.ExportPurchasesToCsv(1);

            // Assert
            Assert.That(result, Is.InstanceOf<FileContentResult>());
            var fileResult = result as FileContentResult;
            Assert.That(fileResult.ContentType, Is.EqualTo("text/csv"));
        }
    }
}
