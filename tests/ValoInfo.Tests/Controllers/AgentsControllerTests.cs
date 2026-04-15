using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using ValoInfo.Api.Controllers;
using ValoInfo.Application.DTOs.Agents;
using ValoInfo.Application.Interfaces;
using ValoInfo.Domain.Common;
using ValoInfo.Tests.Common;

namespace ValoInfo.Tests.Controllers;

public class AgentsControllerTests
{
    private readonly Mock<IAgentRepository> _repoMock;
    private readonly AgentsController _controller;

    public AgentsControllerTests()
    {
        _repoMock = new Mock<IAgentRepository>();
        _controller = new AgentsController(_repoMock.Object);
    }

    // --- GetAll ---

    [Fact]
    public async Task GetAll_ReturnsOk_WhenAgentsExist()
    {
        _repoMock.Setup(r => r.GetAllAgentsAsync())
            .ReturnsAsync(TestDataBuilder.BuildAgentList(2));

        var result = await _controller.GetAll();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<ApiResponse<List<AgenteResponse>>>().Subject;
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(200);
        response.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithEmptyList()
    {
        _repoMock.Setup(r => r.GetAllAgentsAsync())
            .ReturnsAsync(new List<Domain.Entities.Agente>());

        var result = await _controller.GetAll();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<ApiResponse<List<AgenteResponse>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_Returns500_WhenRepositoryThrows()
    {
        _repoMock.Setup(r => r.GetAllAgentsAsync())
            .ThrowsAsync(new Exception("Error de conexión"));

        var act = async () => await _controller.GetAll();

        await act.Should().ThrowAsync<Exception>();
    }

    // --- GetAgentById ---

    [Fact]
    public async Task GetAgentById_ReturnsOk_WhenAgentFound()
    {
        var agent = TestDataBuilder.BuildAgent("phoenix");
        _repoMock.Setup(r => r.GetAgentById("phoenix")).ReturnsAsync(agent);

        var result = await _controller.GetAgentById("phoenix");

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<ApiResponse<AgenteResponse>>().Subject;
        response.Success.Should().BeTrue();
        response.StatusCode.Should().Be(200);
        response.Data!.Nombre.Should().Be("Phoenix");
    }

    [Fact]
    public async Task GetAgentById_Returns404_WhenAgentNotFound()
    {
        _repoMock.Setup(r => r.GetAgentById("desconocido")).ReturnsAsync((Domain.Entities.Agente?)null);

        var result = await _controller.GetAgentById("desconocido");

        var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = notFound.Value.Should().BeOfType<ApiResponse<AgenteResponse>>().Subject;
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(404);
        response.Data.Should().BeNull();
        response.Message.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetAgentById_Returns400_WhenIdIsEmpty(string id)
    {
        var result = await _controller.GetAgentById(id);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var response = badRequest.Value.Should().BeOfType<ApiResponse<AgenteResponse>>().Subject;
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(400);
    }
}
