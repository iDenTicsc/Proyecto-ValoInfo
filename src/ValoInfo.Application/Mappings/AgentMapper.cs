using ValoInfo.Application.DTOs.Agents;
using ValoInfo.Domain.Entities;

namespace ValoInfo.Application.Mappings;

public static class AgentMapper
{
    public static AgenteResponse ToResponse(Agente agent)
    {
        return new AgenteResponse
        {
            Nombre = agent.Nombre,
            Rol = agent.Rol,
            Biografia = agent.Biografia,
            Habilidades = agent.Habilidades
                .Select(h => new HabilidadResponse
                {
                    NombreHabilidad = h.NombreHabilidad,
                    Keybind = h.Keybind,
                    Descripcion = h.Descripcion,
                    Logo = h.Logo,
                    Video = h.Video
                })
            .ToList(),
            Imagen = agent.Imagen
        };
    }
}