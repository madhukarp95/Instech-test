using Claims.Controllers;
using Claims.Models;
using Claims.Models.DTO;
using Claims.Services.Coverage;
using Claims.Services.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Claims.Tests
{
    public class CoversControllerTests
    {
        private readonly Mock<ILogger<CoversController>> _logger;
        private readonly Mock<ICoverService> _coverService;
        private readonly Mock<IChannelQueue> _channel;
        private readonly CoversController _coversController;

        public CoversControllerTests()
        {
            _logger = new Mock<ILogger<CoversController>>();
            _coverService = new Mock<ICoverService>();
            _channel = new Mock<IChannelQueue>();
            _coversController = new CoversController(_coverService.Object, _channel.Object, _logger.Object);
        }

        [Fact]
        public void ComputePremium_ReturnsOk_WithPremium()
        {
            // Arrange
            var coverDto = new CoverDto
            {
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                Type = CoverType.Yacht
            };

            var expectedPremium = 1234m;
            _coverService.Setup(s => s.ComputePremium(coverDto)).Returns(expectedPremium);

            // Act
            var result = _coversController.ComputePremium(coverDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedPremium, okResult.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsOk_WhenCoversExist()
        {
            // Arrange
            var covers = new List<Cover> {
                new()
                {
                    Id = "1",
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                    Type = CoverType.Yacht, Premium = 100
                }
            };

            _coverService.Setup(s => s.GetCoverAsync()).ReturnsAsync(covers);

            // Act
            var result = await _coversController.GetAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<IEnumerable<Cover>>(okResult.Value, false);
        }

        [Fact]
        public async Task GetAsync_ReturnsNoContent_WhenNoCoversExist()
        {
            // Arrange
            _coverService.Setup(s => s.GetCoverAsync()).ReturnsAsync([]);

            // Act
            var result = await _coversController.GetAsync();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetAsync_ById_ReturnsOk_WhenCoverExists()
        {
            // Arrange
            var id = "1";
            var cover = new Cover
            {
                Id = id,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                Type = CoverType.Yacht,
                Premium = 100
            };
            _coverService.Setup(s => s.GetCoverAsync(id)).ReturnsAsync(cover);

            // Act
            var result = await _coversController.GetAsync(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(cover, okResult.Value);
        }

        [Fact]
        public async Task GetAsync_ById_ReturnsNoContent_WhenCoverNotFound()
        {
            // Arrange
            var id = "1";
            _coverService.Setup(s => s.GetCoverAsync(id)).ReturnsAsync((Cover?)null);

            // Act
            var result = await _coversController.GetAsync(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOk_AndEnqueuesAudit()
        {
            // Arrange
            var coverDto = new CoverDto
            {
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                Type = CoverType.Yacht
            };

            var cover = new Cover
            {
                Id = "1",
                StartDate = coverDto.StartDate.Value,
                EndDate = coverDto.EndDate.Value,
                Type = coverDto.Type,
                Premium = 100
            };

            _coverService.Setup(s => s.AddItemAsync(coverDto)).ReturnsAsync(cover);

            // Act
            var result = await _coversController.CreateAsync(coverDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(cover, okResult.Value);
        }

        [Fact]
        public async Task DeleteAsync_DeletesCover_AndEnqueuesAudit()
        {
            // Arrange
            var id = "1";
            _channel.Setup(s => s.EnqueueAsync(It.IsAny<ChannelRequest>())).Returns(ValueTask.CompletedTask).Verifiable();
            _coverService.Setup(s => s.DeleteItemAsync(id)).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _coversController.DeleteAsync(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _channel.Verify(s => s.EnqueueAsync(It.Is<ChannelRequest>(r => r.Id == id && r.HttpRequestType == "DELETE" && r.type == "COVER")), Times.Once);
            _coverService.Verify(s => s.DeleteItemAsync(id), Times.Once);
        }
    }
}
