using Moq;
using OrderProcessingTask.Core.Infrastructure.Logging;
using OrderProcessingTask.Core.Infrastructure.Repositories;
using OrderProcessingTask.Core.Services;
using OrderProcessingTask.Core.Services.Validators;

namespace OrderProcessingTask.Core.UnitTests;

public class ProcessOrderTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IOrderValidator> _mockOrderValidator;
    private readonly Mock<ILogger> _mockILogger;
    private readonly OrderService _sut;

    public ProcessOrderTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockOrderValidator = new Mock<IOrderValidator>();
        _mockILogger = new Mock<ILogger>();
        _sut = new OrderService(_mockOrderValidator.Object, _mockILogger.Object, _mockOrderRepository.Object);
    }

    [Fact]
    public async Task ProcessOrderAsync_ShouldLogSuccess_WhenOrderExists()
    {
        // Arrange
        var orderId = 1;
        _mockOrderValidator.Setup(v => v.IsValid(orderId)).Returns(true);
        _mockOrderRepository.Setup(r => r.GetOrderAsync(orderId)).ReturnsAsync("Laptop");

        // Act
        await _sut.ProcessOrderAsync(orderId);

        // Assert
        _mockOrderRepository.Verify(r => r.GetOrderAsync(orderId), Times.Once);
        _mockILogger.Verify(
            l => l.LogInfo(It.Is<string>(s => s.Contains($"Order id: {orderId} processed successfully."))), Times.Once);
    }

    [Fact]
    public async Task ProcessOrderAsync_ShouldLogError_WhenIdIsInvalid()
    {
        // Arrange
        var orderId = -1;
        _mockOrderValidator.Setup(v => v.IsValid(orderId)).Returns(false);

        // Act
        await _sut.ProcessOrderAsync(orderId);

        // Assert
        _mockOrderRepository.Verify(r => r.GetOrderAsync(It.IsAny<int>()), Times.Never);
        _mockILogger.Verify(
            l => l.LogError(It.Is<string>(s => s.Contains($"Order id: {orderId} is invalid.")),
                It.IsAny<ArgumentException>()), Times.Once);
    }

    [Fact]
    public async Task ProcessOrderAsync_ShouldLogError_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = 1;
        _mockOrderValidator.Setup(v => v.IsValid(orderId)).Returns(true);
        _mockOrderRepository.Setup(r => r.GetOrderAsync(orderId)).ThrowsAsync(new KeyNotFoundException());

        // Act
        await _sut.ProcessOrderAsync(orderId);

        // Assert
        _mockILogger.Verify(
            l => l.LogError(It.Is<string>(s => s.Contains($"Order id: {orderId} not found.")),
                It.IsAny<KeyNotFoundException>()), Times.Once);
    }

    [Fact]
    public async Task ProcessOrderAsync_ShouldHandleMultipleParallelCalls()
    {
        // Arrange
        _mockOrderValidator.Setup(v => v.IsValid(It.IsAny<int>())).Returns(true);
        _mockOrderRepository.Setup(r => r.GetOrderAsync(It.IsAny<int>()))
            .Returns(async () =>
            {
                await Task.Delay(50);
                return "Item";
            });

        // Act
        var tasks = Enumerable.Range(1, 10).Select(id => _sut.ProcessOrderAsync(id));
        await Task.WhenAll(tasks);

        // Assert
        _mockOrderRepository.Verify(r => r.GetOrderAsync(It.IsAny<int>()), Times.Exactly(10));
        _mockILogger.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("processed successfully"))),
            Times.Exactly(10));
    }
}