using FT.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.UseFtObservabilityLogging(
    builder.Configuration.GetSection("FtObservability"));

builder.Services.AddFtObservabilityTracing(
    builder.Configuration.GetSection("FtObservability"));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/demo", (
    ILogger<Program> logger,
    FtObservabilityProvider observability) =>
{
    using var activity =
        observability.ActivitySource.StartActivity("demo.request");

    activity?.SetTag("demo.type", "manual-test");
    activity?.SetTag("demo.service", "DemoService");

    logger.LogInformation("Demo endpoint called");

    return Results.Ok(new
    {
        message = "FT.Observability demo executed",
        traceId = activity?.TraceId.ToString()
    });
});

app.Run();