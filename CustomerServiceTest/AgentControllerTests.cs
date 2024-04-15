using CustomerService.Controllers;
using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CustomerService.Tests
{
    [TestFixture]
    public class AgentControllerTests
    {
        private Mock<IAgentService> _mockAgentService;
        private AgentController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAgentService = new Mock<IAgentService>();
            _controller = new AgentController(_mockAgentService.Object);
        }

        [Test]
        public async Task GetAgents_ReturnsOkResult_WithListOfAgents()
        {
            // Arrange
            var agents = new List<AgentReadDto> { new AgentReadDto { FirstName = "Nikola", LastName="Mirkovic", Email="nikolica@gmail.com" } };
            _mockAgentService.Setup(service => service.GetAllAgentsAsync()).ReturnsAsync(agents);

            // Act
            var result = await _controller.GetAgents();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedAgents = okResult.Value as IEnumerable<AgentReadDto>;
            Assert.IsNotNull(returnedAgents);
            CollectionAssert.AreEqual(agents, returnedAgents);
        }

        [Test]
        public async Task GetAgentById_ReturnsOkResult_WithAgent()
        {
            // Arrange
            var agent = new AgentReadDto { FirstName = "Nikola", LastName = "Mirkovic", Email = "nikolica@gmail.com" };
            _mockAgentService.Setup(service => service.GetAgentByIdAsync(1)).ReturnsAsync(agent);

            // Act
            var result = await _controller.GetAgentById(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedAgent = okResult.Value as AgentReadDto;
            Assert.IsNotNull(returnedAgent);
            Assert.AreEqual(agent.FirstName, returnedAgent.FirstName);
            Assert.AreEqual(agent.LastName, returnedAgent.LastName);
            Assert.AreEqual(agent.Email, returnedAgent.Email);
        }

        [Test]
        public async Task GetAgentById_ReturnsNotFound_WhenAgentNotFound()
        {
            // Arrange
            _mockAgentService.Setup(service => service.GetAgentByIdAsync(1)).Throws(new KeyNotFoundException());

            // Act
            var result = await _controller.GetAgentById(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }
    }
}

