using Moq;
using OrderProcessingTask.Core.Domain;
using OrderProcessingTask.Core.Domain.Exceptions;
using OrderProcessingTask.Core.Infrastructure.Logging;
using OrderProcessingTask.Core.Infrastructure.Notification;
using OrderProcessingTask.Core.Infrastructure.Persistence;
using OrderProcessingTask.Core.Infrastructure.Repositories;
using OrderProcessingTask.Core.Services;
using OrderProcessingTask.Core.Services.Validators;

namespace OrderProcessingTask.Core.UnitTests;

public class AddOrderTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IOrderValidator> _mockOrderValidator;
    private readonly Mock<ILogger> _mockILogger;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly OrderService _sut;

    public AddOrderTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockOrderValidator = new Mock<IOrderValidator>();
        _mockILogger = new Mock<ILogger>();
        _mockNotificationService = new Mock<INotificationService>();
        _sut = new OrderService(_mockOrderValidator.Object, _mockILogger.Object, _mockOrderRepository.Object,
            _mockNotificationService.Object);
    }

    [Fact]
    public async Task AddOrderAsync_ShouldCallRepository_WhenOrderIsNew()
    {
        // Arrange
        var newOrder = new Order { Id = 1, Description = "Phone" };
        _mockOrderRepository.Setup(r => r.AddOrderAsync(newOrder)).Returns(Task.CompletedTask);
        _mockOrderValidator.Setup(v => v.IsValid(newOrder.Id)).Returns(true);

        // Act
        await _sut.AddOrderAsync(newOrder);

        // Assert
        _mockOrderRepository.Verify(r => r.AddOrderAsync(It.Is<Order>(o => o.Id == newOrder.Id)), Times.Once);
        _mockILogger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        _mockILogger.Verify(
            l => l.LogInfo(It.Is<string>(s => s.Contains($"Order with id {newOrder.Id} added successfully."))),
            Times.Once);
        _mockNotificationService.Verify(n => n.Send(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task AddOrderAsync_ShouldLogErrorAndThrow_WhenOrderAlreadyExists()
    {
        // Arrange
        var duplicateOrder = new Order { Id = 1, Description = "Laptop" };
        var exception = new OrderAlreadyExistsException(duplicateOrder.Id);
        _mockOrderValidator.Setup(v => v.IsValid(duplicateOrder.Id)).Returns(true);

        _mockOrderRepository.Setup(r => r.AddOrderAsync(duplicateOrder))
            .ThrowsAsync(exception);

        // Act
        await Assert.ThrowsAsync<OrderAlreadyExistsException>(() => _sut.AddOrderAsync(duplicateOrder));

        // Assert
        _mockILogger.Verify(
            l => l.LogError(
                It.Is<string>(s => s.Contains($"Failed to add new order with id {duplicateOrder.Id}")),
                exception),
            Times.Once);
        _mockNotificationService.Verify(n => n.Send(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AddOrderAsync_ShouldBeThreadSafe_AndOnlyAddOneOrder_WhenMultipleThreadsAddSameId()
    {
        // Arrange
        var order = new Order { Id = 1, Description = "Concurrent Item" };
        var successCount = 0;
        var failureCount = 0;

        // Using real in-memory store to simulate concurrency
        var store = new InMemoryOrderStore();
        var repo = new OrderRepository(store);
        var service = new OrderService(_mockOrderValidator.Object, _mockILogger.Object, repo,
            _mockNotificationService.Object);

        _mockOrderValidator.Setup(v => v.IsValid(order.Id)).Returns(true);

        // Act
        var tasks = Enumerable.Range(0, 10).Select(async _ =>
        {
            try
            {
                await service.AddOrderAsync(order);
                Interlocked.Increment(ref successCount);
            }
            catch (OrderAlreadyExistsException)
            {
                Interlocked.Increment(ref failureCount);
            }
        });

        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(1, successCount);
        Assert.Equal(9, failureCount);
        _mockILogger.Verify(
            l => l.LogInfo(It.Is<string>(s => s.Contains($"Order with id {order.Id} added successfully."))),
            Times.Once);
        _mockILogger.Verify(
            l => l.LogError(It.Is<string>(s => s.Contains("already exists") || s.Contains("Failed to add")),
                It.IsAny<OrderAlreadyExistsException>()),
            Times.Exactly(9));
        _mockNotificationService.Verify(n => n.Send(It.IsAny<string>()), Times.Once);
    }
}