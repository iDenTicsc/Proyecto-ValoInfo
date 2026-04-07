using Microsoft.AspNetCore.Mvc;
using ValoInfo.Application.Interfaces;
using ValoInfo.Application.Mappings;

namespace ValoInfo.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/agentes")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class AgentsController : ControllerBase
{
    private readonly IAgentRepository _repo;

    public AgentsController(IAgentRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAll()
    {
        var agents = await _repo.GetAllAgentsAsync();
        var response = agents.Select(AgentMapper.ToResponse).ToList();
        return Ok(response);
    }

    [HttpGet("{id}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAgentById(string id)
    {
        var agent = await _repo.GetAgentById(id);

        if (agent == null) return NotFound("No encontrado mi papacho");

        var response = AgentMapper.ToResponse(agent);
        return Ok(response);
    }
}