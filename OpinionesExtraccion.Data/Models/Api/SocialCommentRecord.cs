namespace OpinionesExtraccion.Data.Models.Api;

/// <summary>
/// Registro extraído desde la API REST (comentarios de redes sociales).
/// </summary>
public class SocialCommentRecord
{
    public int IdOpinion { get; set; }
    public int? IdCliente { get; set; }
    public int IdProducto { get; set; }
    public string Fuente { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Comentario { get; set; } = string.Empty;
}