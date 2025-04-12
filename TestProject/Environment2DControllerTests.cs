using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ProjectNaam.WebApi.Controllers;
using ProjectNaam.WebApi.Repositories;
using ProjectNaam.WebApi.Models;

namespace TestProject
{
    [TestClass]
    public class Environment2DControllerTests
    {
        private Mock<IEnvironment2DRepository> _mockRepo;
        private Environment2DController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IEnvironment2DRepository>();
            _controller = new Environment2DController(_mockRepo.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        public async Task GetAll_ReturnsOkWithEnvironments()
        {
            // Arrange
            var environments = new List<Environment2D> {
                new Environment2D { Id = "1", Name = "Test", UserId = "user123" }
            };
            _mockRepo.Setup(r => r.GetAllAsync("user123")).ReturnsAsync(environments);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedList = okResult.Value as List<Environment2D>;
            Assert.AreEqual(1, returnedList.Count);
        }

        [TestMethod]
        public async Task Create_ReturnsCreatedAtAction()
        {
            // Arrange
            var env = new Environment2D { Name = "New", MaxHeight = 10, MaxWidth = 10 };

            // Act
            var result = await _controller.Create(env);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            var returnedEnv = createdResult.Value as Environment2D;
            Assert.AreEqual("user123", returnedEnv.UserId);
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync("fake-id")).ReturnsAsync((Environment2D)null);

            // Act
            var result = await _controller.GetById("fake-id");

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
