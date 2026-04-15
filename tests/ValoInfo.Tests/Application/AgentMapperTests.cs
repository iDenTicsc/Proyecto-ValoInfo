using FluentAssertions;
using Xunit;
using ValoInfo.Application.Mappings;
using ValoInfo.Tests.Common;

namespace ValoInfo.Tests.Application;

public class AgentMapperTests
{
    [Fact]
    public void ToResponse_MapsAllFields_Correctly()
    {
        var agent = TestDataBuilder.BuildAgent();

        var dto = AgentMapper.ToResponse(agent);

        dto.Nombre.Should().Be(agent.Nombre);
        dto.Rol.Should().Be(agent.Rol);
        dto.Biografia.Should().Be(agent.Biografia);
        dto.Imagen.Should().Be(agent.Imagen);
        dto.Habilidades.Should().HaveCount(agent.Habilidades.Count);
    }

    [Fact]
    public void ToResponse_MapsSkills_Correctly()
    {
        var agent = TestDataBuilder.BuildAgent();
        var primeraHabilidad = agent.Habilidades[0];

        var dto = AgentMapper.ToResponse(agent);
        var habilidadDto = dto.Habilidades[0];

        habilidadDto.Keybind.Should().Be(primeraHabilidad.Keybind);
        habilidadDto.NombreHabilidad.Should().Be(primeraHabilidad.NombreHabilidad);
        habilidadDto.Descripcion.Should().Be(primeraHabilidad.Descripcion);
        habilidadDto.Logo.Should().Be(primeraHabilidad.Logo);
        habilidadDto.Video.Should().Be(primeraHabilidad.Video);
    }

    [Fact]
    public void ToResponse_MapsEmptySkills_Correctly()
    {
        var agent = TestDataBuilder.BuildAgent();
        agent.Habilidades.Clear();

        var dto = AgentMapper.ToResponse(agent);

        dto.Habilidades.Should().BeEmpty();
    }

    [Fact]
    public void ToResponse_DoesNotMapId()
    {
        var agent = TestDataBuilder.BuildAgent("phoenix");

        var dto = AgentMapper.ToResponse(agent);

        // AgenteResponse no tiene campo Id
        dto.GetType().GetProperty("Id").Should().BeNull();
    }
}
