using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpinionesExtraccion.Data.Interfaces;
using OpinionesExtraccion.Data.Models.Api;
using OpinionesExtraccion.Data.Models.Csv;
using OpinionesExtraccion.Data.Models.Db;

namespace OpinionesExtraccion.Worker;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ISurveyCsvRepository _surveyCsvRepository;
    private readonly IWebReviewRepository _webReviewRepository;
    private readonly ISocialCommentApiRepository _socialCommentApiRepository;
    private readonly ITemporaryFileRepository _temporaryFileRepository;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public Worker(
        ILogger<Worker> logger,
        ISurveyCsvRepository surveyCsvRepository,
        IWebReviewRepository webReviewRepository,
        ISocialCommentApiRepository socialCommentApiRepository,
        ITemporaryFileRepository temporaryFileRepository,
        IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _surveyCsvRepository = surveyCsvRepository;
        _webReviewRepository = webReviewRepository;
        _socialCommentApiRepository = socialCommentApiRepository;
        _temporaryFileRepository = temporaryFileRepository;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var totalStopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Iniciando las extracciones en paralelo.");

            await Task.WhenAll(
                ExtractSurveysAsync(stoppingToken),
                ExtractWebReviewsAsync(stoppingToken),
                ExtractSocialCommentsAsync(stoppingToken));

            totalStopwatch.Stop();

            _logger.LogInformation("Todas las extracciones finalizaron correctamente.");
            _logger.LogInformation("Tiempo total: {ElapsedMilliseconds} ms", totalStopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning("El proceso de extraccion fue cancelado.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ocurrio un error durante el proceso de extraccion.");
        }
        finally
        {
            _logger.LogInformation("El proceso finalizara en 10 segundos.");

            await Task.Delay(TimeSpan.FromSeconds(10), CancellationToken.None);

            _applicationLifetime.StopApplication();
        }
    }

    private async Task ExtractSurveysAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Iniciando la extraccion de encuestas desde CSV.");

        var surveys = await _surveyCsvRepository.GetSurveysAsync(cancellationToken);

        await _temporaryFileRepository.SaveJsonAsync("surveys-csv.json", surveys, cancellationToken);

        stopwatch.Stop();

        _logger.LogInformation("Las encuestas fueron guardadas en Temp/surveys-csv.json.");
        _logger.LogInformation("Encuestas extraidas: {RecordCount}", surveys.Count);
        _logger.LogInformation("Tiempo de extraccion de encuestas: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
    }

    private async Task ExtractWebReviewsAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Iniciando la extraccion de resenas web desde la BD relacional.");

        var reviews = await _webReviewRepository.GetWebReviewsAsync(cancellationToken);

        await _temporaryFileRepository.SaveJsonAsync("webreviews-db.json", reviews, cancellationToken);

        stopwatch.Stop();

        _logger.LogInformation("Las resenas web fueron guardadas en Temp/webreviews-db.json.");
        _logger.LogInformation("Resenas web extraidas: {RecordCount}", reviews.Count);
        _logger.LogInformation("Tiempo de extraccion de resenas web: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
    }

    private async Task ExtractSocialCommentsAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Iniciando la extraccion de comentarios desde la API REST.");

        var comments = await _socialCommentApiRepository.GetSocialCommentsAsync(cancellationToken);

        await _temporaryFileRepository.SaveJsonAsync("socialcomments-api.json", comments, cancellationToken);

        stopwatch.Stop();

        _logger.LogInformation("Los comentarios fueron guardados en Temp/socialcomments-api.json.");
        _logger.LogInformation("Comentarios extraidos desde la API: {RecordCount}", comments.Count);
        _logger.LogInformation("Tiempo de extraccion desde la API: {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
    }
}