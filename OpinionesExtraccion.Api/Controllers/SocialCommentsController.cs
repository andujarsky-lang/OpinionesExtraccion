using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OpinionesExtraccion.Data.Models.Api;

namespace OpinionesExtraccion.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SocialCommentsController : ControllerBase
{
    private readonly string _connectionString;

    public SocialCommentsController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AnalisisOpinionesDb")
            ?? "Server=.\\SQLEXPRESS;Database=AnalisisOpinionesdb;Integrated Security=True;TrustServerCertificate=True;";
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SocialCommentRecord>>> Get()
    {
        var result = new List<SocialCommentRecord>();

        const string sql = @"
            SELECT o.IdOpinion, o.IdCliente, o.IdProducto, fd.IdFuente, o.Fecha, o.Comentario
            FROM Opiniones o
            INNER JOIN SocialComments sc ON o.IdOpinion = sc.IdOpinion
            LEFT JOIN FuentesDatos fd ON o.IdFuente = fd.IdFuente";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new SocialCommentRecord
            {
                IdOpinion = reader.GetInt32(0),
                IdCliente = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                IdProducto = reader.GetInt32(2),
                Fuente = reader.IsDBNull(3) ? "Social" : reader.GetString(3),
                Fecha = reader.GetDateTime(4),
                Comentario = reader.GetString(5)
            });
        }

        return Ok(result);
    }
}