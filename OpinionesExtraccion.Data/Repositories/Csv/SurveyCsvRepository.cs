using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using OpinionesExtraccion.Data.Interfaces;
using OpinionesExtraccion.Data.Models.Csv;

namespace OpinionesExtraccion.Data.Repositories.Csv;

public class SurveyCsvRepository : ISurveyCsvRepository
{
    private readonly string _filePath;

    public SurveyCsvRepository(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<IReadOnlyList<SurveyCsvRecord>> GetSurveysAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<SurveyCsvRecord>();

        if (!File.Exists(_filePath))
            return result;

        using var reader = new StreamReader(_filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        });

        var records = csv.GetRecordsAsync<SurveyCsvRecord>(cancellationToken);
        await foreach (var record in records.WithCancellation(cancellationToken))
        {
            result.Add(record);
        }

        return result;
    }
}