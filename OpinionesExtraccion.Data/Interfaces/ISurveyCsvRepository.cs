using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpinionesExtraccion.Data.Models.Csv;

namespace OpinionesExtraccion.Data.Interfaces;

public interface ISurveyCsvRepository
{
    Task<IReadOnlyList<SurveyCsvRecord>> GetSurveysAsync(CancellationToken cancellationToken = default);
}