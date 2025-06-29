using Claims.Controllers;
using Claims.Models.DTO;
using Claims.Services.Claims;
using Claims.Services.Coverage;
using Claims.Services.Channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using Claims.Models.Channel;
using Claims.Models.Claim;
using Claims.Models.Cover;

namespace Claims.Tests;

public class ClaimsControllerTests
{
    private readonly Mock<ILogger<ClaimsController>> _logger;
    private readonly Mock<IClaimsService> _claimsService;
    private readonly Mock<ICoverService> _coverService;
    private readonly Mock<IChannelQueue> _channel;
    private readonly ClaimsController _claimsController;

    public ClaimsControllerTests()
    {
        _logger = new Mock<ILogger<ClaimsController>>();
        _claimsService = new Mock<IClaimsService>();
        _coverService = new Mock<ICoverService>();
        _channel = new Mock<IChannelQueue>();

        _claimsController = new ClaimsController(
            _logger.Object,
            _claimsService.Object,
            _coverService.Object,
            _channel.Object
        );
    }

    [Fact]
    public async Task GetAsync_ReturnsOk_WhenClaimsExist()
    {
        // Arrange
        var claims = new List<Claim> {
            new()
            {
                Id = "1",
                CoverId = "C1", Name = "Test1",
                Created = DateOnly.FromDateTime(DateTime.UtcNow),
                Type = ClaimType.Collision, DamageCost = 100
            },
            new()
            {
                Id = "2",
                CoverId = "C2", Name = "Test2",
                Created = DateOnly.FromDateTime(DateTime.UtcNow),
                Type = ClaimType.BadWeather, DamageCost = 500
            }
        };
        _claimsService.Setup(s => s.GetClaimsAsync()).ReturnsAsync(claims);

        // Act
        var result = await _claimsController.GetAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<IEnumerable<Claim>>(okResult.Value, false);

        Assert.Equal(claims, okResult.Value);
    }

    [Fact]
    public async Task GetAsync_ReturnsNoContent_WhenNoClaimsExist()
    {
        // Arrange
        _claimsService.Setup(s => s.GetClaimsAsync()).ReturnsAsync([]);

        // Act
        var result = await _claimsController.GetAsync();

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNotFound_WhenCoverNotFound()
    {
        // Arrange
        var claimDto = new ClaimDto
        {
            CoverId = "C1",
            Created = DateOnly.FromDateTime(DateTime.UtcNow),
            Name = "Test",
            DamageCost = 100,
            Type = ClaimType.Collision
        };

        _coverService.Setup(s => s.GetCoverAsync(claimDto.CoverId)).ReturnsAsync((Cover?)null);

        // Act
        var result = await _claimsController.CreateAsync(claimDto);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);

        Assert.Contains("Cover with ID C1 not found.", notFound?.Value?.ToString());
    }

    [Fact]
    public async Task CreateAsync_ReturnsBadRequest_WhenClaimDateInvalid()
    {
        // Arrange
        var claimDto = new ClaimDto
        {
            CoverId = "Cov1",
            Created = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
            Name = "Test1",
            DamageCost = 100,
            Type = ClaimType.Collision
        };

        var cover = new Cover
        {
            Id = "Cov1",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            Type = CoverType.Yacht,
            Premium = 100
        };

        _coverService.Setup(s => s.GetCoverAsync(claimDto.CoverId)).ReturnsAsync(cover);

        // Act
        var result = await _claimsController.CreateAsync(claimDto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Claim date is not within the cover period", badRequest?.Value?.ToString());
    }

    [Fact]
    public async Task CreateAsync_ReturnsOk_WhenClaimIsValid()
    {
        // Arrange
        var claimDto = new ClaimDto
        {
            CoverId = "C1",
            Created = DateOnly.FromDateTime(DateTime.UtcNow),
            Name = "Test",
            DamageCost = 100,
            Type = ClaimType.Collision
        };

        var cover = new Cover
        {
            Id = "C1",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Type = CoverType.Yacht,
            Premium = 100
        };

        var claim = new Claim
        {
            Id = "C1",
            CoverId = "C1",
            Name = "Test",
            Created = claimDto.Created.Value,
            Type = claimDto.Type,
            DamageCost = claimDto.DamageCost
        };

        _coverService.Setup(s => s.GetCoverAsync(claimDto.CoverId)).ReturnsAsync(cover);
        _claimsService.Setup(s => s.AddItemAsync(claimDto)).ReturnsAsync(claim);
        _channel.Setup(s => s.EnqueueAsync(It.IsAny<ChannelRequest>())).Returns(ValueTask.CompletedTask);

        // Act
        var result = await _claimsController.CreateAsync(claimDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(claim, okResult.Value);
    }

    [Fact]
    public async Task DeleteAsync_CallsDeleteAndEnqueue()
    {
        // Arrange
        var id = "cl1";
        _channel.Setup(s => s.EnqueueAsync(It.IsAny<ChannelRequest>())).Returns(ValueTask.CompletedTask).Verifiable();
        _claimsService.Setup(s => s.DeleteItemAsync(id)).Returns(Task.CompletedTask).Verifiable();

        // Act
        await _claimsController.DeleteAsync(id);

        // Assert
        _channel.Verify(s => s.EnqueueAsync(It.Is<ChannelRequest>(r => r.Id == id && r.HttpRequestType == "DELETE" && r.type == "CLAIM")), Times.Once);
        _claimsService.Verify(s => s.DeleteItemAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ById_ReturnsOk_WhenClaimExists()
    {
        // Arrange
        var id = "cl1";
        var claim = new Claim
        {
            Id = id,
            CoverId = "C1",
            Name = "Test",
            Created = DateOnly.FromDateTime(DateTime.UtcNow),
            Type = ClaimType.Collision,
            DamageCost = 100
        };
        _claimsService.Setup(s => s.GetClaimAsync(id)).ReturnsAsync(claim);

        // Act
        var result = await _claimsController.GetAsync(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(claim, okResult.Value);
    }

    [Fact]
    public async Task GetAsync_ById_ReturnsNoContent_WhenClaimNotFound()
    {
        // Arrange
        var id = "cl1";
        _claimsService.Setup(s => s.GetClaimAsync(id)).ReturnsAsync((Claim?)null);

        // Act
        var result = await _claimsController.GetAsync(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
