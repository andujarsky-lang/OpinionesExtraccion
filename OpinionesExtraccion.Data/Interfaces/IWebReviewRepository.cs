using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpinionesExtraccion.Data.Models.Db;

namespace OpinionesExtraccion.Data.Interfaces;

public interface IWebReviewRepository
{
    Task<IReadOnlyList<WebReviewRecord>> GetWebReviewsAsync(CancellationToken cancellationToken = default);
}