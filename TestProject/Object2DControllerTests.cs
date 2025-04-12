using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using ProjectNaam.WebApi.Controllers;
using ProjectNaam.WebApi.Models;
using ProjectNaam.WebApi.Repositories;

namespace TestProject
{
    [TestClass]
    public class Object2DControllerTests
    {
        private Mock<IObject2DRepository> _mockRepo;
        private Object2DController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = new Mock<IObject2DRepository>();
            _controller = new Object2DController(_mockRepo.Object);
        }

        [TestMethod]
        public async Task GetAll_ReturnsListOfObjects()
        {
            var envId = "env123";
            var mockList = new List<Object2D> { new Object2D { Id = "1", EnvironmentId = envId } };
            _mockRepo.Setup(r => r.GetObjectsByEnvironmentIdAsync(envId)).ReturnsAsync(mockList);

            var result = await _controller.GetAll(envId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(mockList, result.Value);
        }

        [TestMethod]
        public async Task GetById_ReturnsObject_WhenFound()
        {
            var objId = "obj1";
            var obj = new Object2D { Id = objId };
            _mockRepo.Setup(r => r.GetObjectByIdAsync(objId)).ReturnsAsync(obj);

            var result = await _controller.GetById(objId) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(obj, result.Value);
        }

        [TestMethod]
        public async Task Create_ReturnsCreatedResult()
        {
            var envId = "env456";
            var obj = new Object2D { Id = "newId" };

            var result = await _controller.Create(envId, obj) as CreatedAtActionResult;

            _mockRepo.Verify(r => r.CreateObjectAsync(obj), Times.Once);
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            Assert.AreEqual(obj, result.Value);
        }
    }
}
