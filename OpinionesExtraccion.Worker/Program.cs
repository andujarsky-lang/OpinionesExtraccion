using System.IO;
using Microsoft.Extensions.Hosting;
using OpinionesExtraccion.Data.Interfaces;
using OpinionesExtraccion.Data.Repositories.Api;
using OpinionesExtraccion.Data.Repositories.Csv;
using OpinionesExtraccion.Data.Repositories.Db;
using OpinionesExtraccion.Data.Repositories.Files;
using OpinionesExtraccion.Worker;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("AnalisisOpinionesDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("No se encontro la cadena de conexion AnalisisOpinionesDb.");
}

builder.Services.AddSingleton<IWebReviewRepository>(_ =>
    new WebReviewRepository(connectionString));

var apiBaseUrl = configuration["ApiSettings:BaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException("No se encontro ApiSettings:BaseUrl.");
}

builder.Services.AddHttpClient<ISocialCommentApiRepository, SocialCommentApiRepository>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
});

var csvRelativePath = configuration["CsvSettings:SurveysPath"];
if (string.IsNullOrWhiteSpace(csvRelativePath))
{
    throw new InvalidOperationException("No se encontro CsvSettings:SurveysPath.");
}

builder.Services.AddSingleton<ISurveyCsvRepository>(serviceProvider =>
{
    var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
    var fullPath = Path.Combine(environment.ContentRootPath, csvRelativePath);
    return new SurveyCsvRepository(fullPath);
});

var tempDirectory = configuration["TemporaryFiles:Directory"];
if (string.IsNullOrWhiteSpace(tempDirectory))
{
    throw new InvalidOperationException("No se encontro TemporaryFiles:Directory.");
}

builder.Services.AddSingleton<ITemporaryFileRepository>(serviceProvider =>
{
    var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
    var fullPath = Path.Combine(environment.ContentRootPath, tempDirectory);
    return new JsonTemporaryFileRepository(fullPath);
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();