using System.Linq.Expressions;
using Hangfire;
using Hangfire.Common;
using ProjectsService.Infrastructure.Services.HangfireJobsInitializer;
using ProjectsService.Tests.UnitTests.Extensions;

namespace ProjectsService.Tests.UnitTests.Tests.Services.InfrastructureServices;

public class HangfireJobsInitializerTests
{
    private readonly Mock<IRecurringJobManager> _recurringJobManagerMock;
    private readonly Mock<ILogger<HangfireJobsInitializer>> _loggerMock;
    private readonly HangfireJobsInitializer _initializer;

    public HangfireJobsInitializerTests()
    {
        _recurringJobManagerMock = new Mock<IRecurringJobManager>();
        var mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<HangfireJobsInitializer>>();
        _initializer = new HangfireJobsInitializer(_recurringJobManagerMock.Object, mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void StartBackgroundJobs_SchedulesRecurringJob()
    {
        // Act
        _initializer.StartBackgroundJobs();

        // Assert
        _recurringJobManagerMock.Verify(r => r.AddOrUpdate(
            "update_project_statuses",
            It.IsAny<Job>(),
            Cron.Hourly(),
            It.IsAny<RecurringJobOptions>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Starting background jobs initialization", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Recurring job 'update_project_statuses' scheduled to run every minute", Times.Once());
    }
}