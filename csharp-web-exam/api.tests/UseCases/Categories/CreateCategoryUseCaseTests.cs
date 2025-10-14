using api.Application.DTOs;
using api.Application.Interfaces;
using api.Application.UseCases.Categories;
using Moq;
using Xunit;

namespace api.tests.UseCases.Categories;

public class CreateCategoryUseCaseTests
{
    private readonly Mock<ICategoryService> _mockService;
    private readonly CreateCategoryUseCase _useCase;

    public CreateCategoryUseCaseTests()
    {
        _mockService = new Mock<ICategoryService>();
        _useCase = new CreateCategoryUseCase(_mockService.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidDto_ReturnsCreatedCategory()
    {
        // Arrange
        var createDto = new CreateCategoryDto { Name = "Sports" };
        var createdDto = new CategoryDto { Id = 1, Name = "Sports" };
        _mockService.Setup(s => s.CreateCategoryAsync(createDto)).ReturnsAsync(createdDto);

        // Act
        var result = await _useCase.ExecuteAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Sports", result.Name);
        _mockService.Verify(s => s.CreateCategoryAsync(createDto), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_NullDto_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _useCase.ExecuteAsync(null!));
    }
}
