using System.Threading;
using System.Threading.Tasks;

namespace OpinionesExtraccion.Data.Interfaces;

public interface ITemporaryFileRepository
{
    Task SaveJsonAsync<T>(string fileName, T data, CancellationToken cancellationToken = default);
}