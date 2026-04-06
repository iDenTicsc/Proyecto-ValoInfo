using Google.Cloud.Firestore;
using ValoInfo.Application.Interfaces;
using ValoInfo.Domain.Entities;

namespace ValoInfo.Infrastructure.Repositories;

public class FirestoreAgentRepository : IAgentRepository
{
    private readonly FirestoreDb _db;
    private readonly string _collection = "Agentes";

    public FirestoreAgentRepository(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<List<Agente>> GetAllAgentsAsync()
    {
        var snapshot = await _db.Collection(_collection).GetSnapshotAsync();
        return snapshot.Documents.Select(doc =>
        {
            var agent = doc.ConvertTo<Agente>();
            agent.Id = doc.Id;
            return agent;
        }).ToList();
    }

    public async Task<Agente?> GetAgentById(string id)
    {
        id = id.ToLowerInvariant();
        var doc = await _db.Collection(_collection).Document(id).GetSnapshotAsync();

        if(!doc.Exists) return null;

        var agent = doc.ConvertTo<Agente>();
        agent.Id = doc.Id;
        return agent;
    }

    public async Task CreateAgentAsync(Agente agent)
    {
        await _db.Collection(_collection).AddAsync(agent);
    }

    public async Task UpdateAgentAsync(Agente agent)
    {
        await _db.Collection(_collection).Document(agent.Id).SetAsync(agent, SetOptions.Overwrite);
    }

    public async Task DeleteAgentAsync(string id)
    {
        await _db.Collection(_collection).Document(id).DeleteAsync();
    }
}

