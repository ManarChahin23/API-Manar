using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectNaam.WebApi.Models;
using ProjectNaam.WebApi.Repositories;

[TestClass]
public class Environment2DRepositoryTests
{
    private class TestableEnvironment2DRepository : Environment2DRepository
    {
        private readonly IDbConnection _mockConnection;

        public TestableEnvironment2DRepository(string connectionString, IDbConnection mockConnection)
            : base(connectionString)
        {
            _mockConnection = mockConnection;
        }

        public override IDbConnection CreateConnection()
        {
            return _mockConnection;
        }
    }

    [TestMethod]
    public async Task ValidateEnvironment_ShouldThrow_WhenNameIsEmpty()
    {
        // Arrange
        var mockDb = new Mock<IDbConnection>();
        var repo = new TestableEnvironment2DRepository("fake", mockDb.Object);
        var env = new Environment2D { Name = "", MaxWidth = 100, MaxHeight = 50, UserId = "1" };

        // Act + Assert
        var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => repo.ValidateEnvironment(env, true));
        Assert.AreEqual("Name must be between 1 and 25 characters.", ex.Message);
    }

    [TestMethod]
    public async Task ValidateEnvironment_ShouldThrow_WhenSizeIsInvalid()
    {
        var mockDb = new Mock<IDbConnection>();
        var repo = new TestableEnvironment2DRepository("fake", mockDb.Object);
        var env = new Environment2D { Name = "Test", MaxWidth = 500, MaxHeight = 5, UserId = "1" };

        var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => repo.ValidateEnvironment(env, true));
        Assert.AreEqual("Width must be 20-200, Height 10-100.", ex.Message);
    }

}