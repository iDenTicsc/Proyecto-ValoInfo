using ValoInfo.Application.DTOs.Agents;
using ValoInfo.Domain.Entities;

namespace ValoInfo.Tests.Common;

public static class TestDataBuilder
{
    public static Agente BuildAgent(string id = "phoenix") => new()
    {
        Id = id,
        Nombre = "Phoenix",
        Rol = "Duelista",
        Biografia = "Agente de prueba",
        Imagen = "https://imagen.com/phoenix.png",
        Habilidades = new List<Habilidad>
        {
            new()
            {
                Keybind = "Q",
                NombreHabilidad = "Fuego Curvado",
                Descripcion = "Lanza una llama curva.",
                Logo = "https://logo.com/q.png",
                Video = "https://video.com/q.mp4"
            },
            new()
            {
                Keybind = "E",
                NombreHabilidad = "Linterna Caliente",
                Descripcion = "Crea una llama curativa.",
                Logo = "https://logo.com/e.png",
                Video = "https://video.com/e.mp4"
            }
        }
    };

    public static List<Agente> BuildAgentList(int count = 2)
    {
        return Enumerable.Range(1, count)
            .Select(i =>
            {
                var agent = BuildAgent($"agent-{i}");
                agent.Nombre = $"Agente {i}";
                return agent;
            })
            .ToList();
    }

    public static AgenteResponse BuildAgentDto() => new()
    {
        Nombre = "Phoenix",
        Rol = "Duelista",
        Biografia = "Agente de prueba",
        Imagen = "https://imagen.com/phoenix.png",
        Habilidades = new List<HabilidadResponse>
        {
            new()
            {
                Keybind = "Q",
                NombreHabilidad = "Fuego Curvado",
                Descripcion = "Lanza una llama curva.",
                Logo = "https://logo.com/q.png",
                Video = "https://video.com/q.mp4"
            }
        }
    };
}
