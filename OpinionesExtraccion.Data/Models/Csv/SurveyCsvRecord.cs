namespace OpinionesExtraccion.Data.Models.Csv;

/// <summary>
/// Registro extraído desde el archivo CSV de encuestas internas.
/// </summary>
public class SurveyCsvRecord
{
    public int IdOpinion { get; set; }
    public int? IdCliente { get; set; }
    public int IdProducto { get; set; }
    public DateTime Fecha { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public string Clasificacion { get; set; } = string.Empty;
    public int? PuntajeSatisfaccion { get; set; }
    public string Fuente { get; set; } = string.Empty;
}