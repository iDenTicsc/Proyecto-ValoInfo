using Microsoft.AspNetCore.Mvc;
using ValoInfo.Application.DTOs.Agents;
using ValoInfo.Application.Interfaces;
using ValoInfo.Application.Mappings;
using ValoInfo.Domain.Common;

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
        var data = agents.Select(AgentMapper.ToResponse).ToList();
        return Ok(ApiResponse<List<AgenteResponse>>.Ok(data));
    }

    [HttpGet("{id}")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAgentById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(ApiResponse<AgenteResponse>.BadRequest("El ID no puede estar vacío."));

        var agent = await _repo.GetAgentById(id);

        if (agent == null)
            return NotFound(ApiResponse<AgenteResponse>.NotFound("No se encontró un agente con el ID proporcionado."));

        var response = AgentMapper.ToResponse(agent);
        return Ok(ApiResponse<AgenteResponse>.Ok(response));
    }
}