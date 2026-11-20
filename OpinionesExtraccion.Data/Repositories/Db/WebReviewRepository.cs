using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using OpinionesExtraccion.Data.Interfaces;
using OpinionesExtraccion.Data.Models.Db;

namespace OpinionesExtraccion.Data.Repositories.Db;

public class WebReviewRepository : IWebReviewRepository
{
    private readonly string _connectionString;

    public WebReviewRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<WebReviewRecord>> GetWebReviewsAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<WebReviewRecord>();

        const string sql = @"
            SELECT o.IdOpinion, o.IdCliente, o.IdProducto, o.Fecha, o.Comentario, wr.Rating
            FROM Opiniones o
            INNER JOIN WebReviews wr ON o.IdOpinion = wr.IdOpinion";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new WebReviewRecord
            {
                IdOpinion = reader.GetInt32(0),
                IdCliente = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                IdProducto = reader.GetInt32(2),
                Fecha = reader.GetDateTime(3),
                Comentario = reader.GetString(4),
                Rating = reader.GetInt32(5)
            });
        }

        return result;
    }
}