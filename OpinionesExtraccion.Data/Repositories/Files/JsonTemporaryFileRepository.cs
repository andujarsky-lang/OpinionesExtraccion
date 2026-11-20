using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OpinionesExtraccion.Data.Interfaces;

namespace OpinionesExtraccion.Data.Repositories.Files;

public class JsonTemporaryFileRepository : ITemporaryFileRepository
{
    private readonly string _directory;

    public JsonTemporaryFileRepository(string directory)
    {
        _directory = directory;
        Directory.CreateDirectory(_directory);
    }

    public async Task SaveJsonAsync<T>(string fileName, T data, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_directory, fileName);
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, data, new JsonSerializerOptions { WriteIndented = true }, cancellationToken);
    }
}