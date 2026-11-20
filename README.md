# OpinionesExtraccion - Proceso de Extracción ETL

Proceso de extracción (fase "E" del ETL) para el **Sistema de Análisis de Opiniones de Clientes**.
Consolida opiniones provenientes de encuestas internas (CSV), reseñas web (base de datos relacional)
y comentarios de redes sociales (API REST), dejándolas disponibles en archivos JSON temporales para su
posterior transformación y carga.

## Proyectos de la solución

| Proyecto | Responsabilidad |
|----------|-----------------|
| `OpinionesExtraccion.Api` | Expone `GET /api/socialcomments` desde SQL Server. |
| `OpinionesExtraccion.Data` | Modelos, interfaces y repositorios de acceso a datos. |
| `OpinionesExtraccion.Worker` | Coordina la extracción paralela y guarda archivos JSON en `Temp/`. |

## Arquitectura

La solución sigue una arquitectura en capas simplificada (Clean Architecture):

- **Fuentes**: archivo CSV, base de datos relacional y API REST.
- **Repositorios**: `SurveyCsvRepository`, `WebReviewRepository`, `SocialCommentApiRepository`, `JsonTemporaryFileRepository`.
- **Contratos**: `ISurveyCsvRepository`, `IWebReviewRepository`, `ISocialCommentApiRepository`, `ITemporaryFileRepository`.
- **Coordinador**: `Worker` ejecuta las tres extracciones en paralelo con `Task.WhenAll`.

Principios aplicados: polimorfismo, Open/Closed, Dependency Inversion, Single Responsibility.

## Tecnologías

- .NET 8/9, ASP.NET Core, Worker Service
- SQL Server (ADO.NET / `Microsoft.Data.SqlClient`)
- HttpClient, CsvHelper, System.Text.Json
- ILogger y Stopwatch (trazabilidad y medición)

## Requisitos previos

- .NET SDK 8 o superior
- SQL Server con la base `AnalisisOpinionesdb` poblada (ver proyecto `CargaOpiniones`)
- Archivo `surveys_part1.csv` en la ruta configurada

## Ejecución

### 1. Iniciar la API
```bash
cd OpinionesExtraccion.Api
dotnet run
```
La API queda disponible en `http://localhost:5000` (Swagger en `/swagger`).

### 2. Ejecutar el Worker
En otra terminal:
```bash
cd OpinionesExtraccion.Worker
dotnet run
```

El Worker extrae las tres fuentes en paralelo y genera:
- `Temp/surveys-csv.json` (encuestas)
- `Temp/webreviews-db.json` (reseñas web)
- `Temp/socialcomments-api.json` (comentarios de redes sociales)

## Configuración

Editar `OpinionesExtraccion.Worker/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "AnalisisOpinionesDb": "Server=.\\SQLEXPRESS;Database=AnalisisOpinionesdb;Integrated Security=True;TrustServerCertificate=True;"
  },
  "ApiSettings": { "BaseUrl": "http://localhost:5000" },
  "CsvSettings": { "SurveysPath": "../../Archivo CSV Análisis de Opiniones de Clientes-20261119/surveys_part1.csv" },
  "TemporaryFiles": { "Directory": "Temp" }
}
```

## Evidencia de ejecución

| Fuente | Extraídos | Duración | Archivo |
|--------|-----------|----------|---------|
| CSV (encuestas) | 500 | ~175 ms | `Temp/surveys-csv.json` |
| BD (reseñas web) | 200 | ~1131 ms | `Temp/webreviews-db.json` |
| API (redes sociales) | 200 | ~2000 ms | `Temp/socialcomments-api.json` |
| **Total** | **900** | **~3.3 s** | |

## Atributos de calidad

- **Rendimiento**: extracción paralela con `Task.WhenAll`, `SqlDataReader` y `HttpClient` asíncrono.
- **Escalabilidad**: fuentes configurables en `appsettings.json`; nuevas fuentes solo requieren implementar su interfaz.
- **Seguridad**: credenciales centralizadas en `appsettings.json`; API con redirección HTTPS.
- **Mantenibilidad**: separación en proyectos, inyección de dependencias, logging con `ILogger`.

## Repositorio

https://github.com/<tu-usuario>/OpinionesExtraccion.git
```