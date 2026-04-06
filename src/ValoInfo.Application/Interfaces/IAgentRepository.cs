using ValoInfo.Domain.Entities;

namespace ValoInfo.Application.Interfaces;

public interface IAgentRepository
{
    Task<List<Agente>> GetAllAgentsAsync();
    Task<Agente?> GetAgentById(string id);
    Task CreateAgentAsync(Agente agente);
    Task UpdateAgentAsync(Agente agente);
    Task DeleteAgentAsync(string id);
}