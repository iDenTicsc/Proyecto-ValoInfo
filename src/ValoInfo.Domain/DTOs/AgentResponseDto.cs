using Google.Cloud.Firestore;

namespace ValoInfo.Application.DTOs.Agents;

[FirestoreData]
public class AgenteResponse
{
    [FirestoreProperty("nombre")]
    public string Nombre { get; set; } = string.Empty;
    [FirestoreProperty("rol")]
    public string Rol { get; set; } = string.Empty;
    [FirestoreProperty("biografia")]
    public string Biografia { get; set; } = string.Empty;
    [FirestoreProperty("habilidades")]
    public List<HabilidadResponse> Habilidades { get; set; } = new List<HabilidadResponse>();
    [FirestoreProperty("imagen")]
    public string Imagen { get; set; } = string.Empty;
}

[FirestoreData]
public class HabilidadResponse
{
    [FirestoreProperty("keybind")]
    public string Keybind { get; set; } = string.Empty;
    [FirestoreProperty("nombre_habilidad")]
    public string NombreHabilidad { get; set; } = string.Empty;
    [FirestoreProperty("descripcion")]
    public string Descripcion { get; set; } = string.Empty;
    [FirestoreProperty("logo")]
    public string Logo { get; set; } = string.Empty;
    [FirestoreProperty("video")]
    public string Video { get; set; } = string.Empty;
}