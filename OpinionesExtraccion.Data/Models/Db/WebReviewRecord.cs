namespace OpinionesExtraccion.Data.Models.Db;
public class WebReviewRecord
{
    public int IdOpinion { get; set; }
    public int? IdCliente { get; set; }
    public int IdProducto { get; set; }
    public DateTime Fecha { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Fuente { get; set; } = "Web";
}