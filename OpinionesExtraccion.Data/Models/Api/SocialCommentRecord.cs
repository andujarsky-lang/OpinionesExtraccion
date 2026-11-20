namespace OpinionesExtraccion.Data.Models.Api;

public class SocialCommentRecord
{
    public int IdOpinion { get; set; }
    public int? IdCliente { get; set; }
    public int IdProducto { get; set; }
    public string Fuente { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Comentario { get; set; } = string.Empty;
}