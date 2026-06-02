# FT.Observability

FT.Observability er et internt NuGet-package til .NET-services, der standardiserer opsætning af logging og tracing via OpenTelemetry.

Formålet er at gøre det nemt at sende logs og traces til en fælles observability pipeline (OpenTelemetry Collector → Elasticsearch → Kibana).

## Installation

```bash
dotnet add package FT.Observability
```

## Configuration

```json
"FtObservability": {
  "ServiceName": "my-service",
  "ServiceVersion": "1.0.0",
  "ActivitySourceName": "FT.Observability.MyService",
  "OtlpEndpoint": "http://localhost:4317"
}
```

## Setup

Logging:

```csharp
builder.UseFtObservabilityLogging(
    builder.Configuration.GetSection("FtObservability"));
```

Tracing:

```csharp
builder.Services.AddFtObservabilityTracing(
    builder.Configuration.GetSection("FtObservability"));
```

## Usage

Tracing:

```csharp
using var activity = _observability.ActivitySource.StartActivity(
    "myservice.operation",
    ActivityKind.Internal);

activity?.SetTag("example.id", id);
```

Logging:

```csharp
_logger.LogInformation("Processing item {Id}", id);
```

Logs skrevet inden for et span vil automatisk blive korreleret med traces via TraceId og SpanId.

## Result

- Logs i Kibana (`ft-observability-logs`)
- Traces i Kibana (`ft-observability-traces`)
- Correlation mellem logs og traces

## Notes

Metrics er ikke inkluderet i denne version.