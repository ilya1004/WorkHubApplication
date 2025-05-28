using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Infrastructure.Data;
using ProjectsService.Infrastructure.Services.DbStartupService;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.Services.InfrastructureServices;

public class DbStartupServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DbStartupService>> _loggerMock;
    private readonly DbStartupService _service;

    public DbStartupServiceTests()
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DbStartupService>>();
        var scopeMock = new Mock<IServiceScope>();
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();

        serviceProviderMock.Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(scopeFactoryMock.Object);
        scopeFactoryMock.Setup(sf => sf.CreateScope()).Returns(scopeMock.Object);
        scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);

        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository)
            .Returns(new Mock<IQueriesRepository<Category>>().Object);
        _unitOfWorkMock.Setup(u => u.CategoryCommandsRepository)
            .Returns(new Mock<ICommandsRepository<Category>>().Object);
        _unitOfWorkMock.Setup(u => u.ProjectCommandsRepository)
            .Returns(new Mock<ICommandsRepository<Project>>().Object);
        _unitOfWorkMock.Setup(u => u.LifecycleCommandsRepository)
            .Returns(new Mock<ICommandsRepository<Lifecycle>>().Object);
        _unitOfWorkMock.Setup(u => u.FreelancerApplicationCommandsRepository)
            .Returns(new Mock<ICommandsRepository<FreelancerApplication>>().Object);

        _service = new DbStartupService(serviceProviderMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public async Task InitializeDb_SkipsSeeding_WhenCategoriesExist()
    {
        // Arrange
        var categories = new List<Category> { new() { Id = Guid.NewGuid(), Name = "Test Category" } };
        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.ListAsync(
                It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Category, object>>[]>()))
            .ReturnsAsync(categories);

        // Act
        await _service.InitializeDb();

        // Assert
        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, "Database already initialized, skipping seeding", Times.Once());
    }

    [Fact]
    public async Task InitializeDb_SeedsData_WhenNoCategoriesExist()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.CategoryQueriesRepository.ListAsync(
                It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Category, object>>[]>()))
            .ReturnsAsync(new List<Category>());

        // Act
        await _service.InitializeDb();

        // Assert
        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.AddAsync(
            It.Is<Category>(c => c.Name == "Web Development"), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.AddAsync(
            It.Is<Category>(c => c.Name == "Mobile Development"), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.AddAsync(
            It.Is<Category>(c => c.Name == "Graphic Design"), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.AddAsync(
            It.Is<Category>(c => c.Name == "Marketing"), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.CategoryCommandsRepository.AddAsync(
            It.Is<Category>(c => c.Name == "Data Analysis"), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.ProjectCommandsRepository.AddAsync(
            It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(u => u.LifecycleCommandsRepository.AddAsync(
            It.IsAny<Lifecycle>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(u => u.FreelancerApplicationCommandsRepository.AddAsync(
            It.IsAny<FreelancerApplication>(), It.IsAny<CancellationToken>()), Times.Once());
        _unitOfWorkMock.Verify(u => u.SaveAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Database initialization completed successfully", Times.Once());
    }
}