using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace FT.Observability;

public static class HostBuilderExtensions
{
    public static HostApplicationBuilder UseFtObservabilityLogging(
        this HostApplicationBuilder builder,
        IConfigurationSection configurationSection)
    {
        var options = new FtObservabilityOptions();
        configurationSection.Bind(options);

        ValidateLoggingOptions(options);

        Log.Logger = CreateLogger(
            builder.Configuration,
            options,
            builder.Environment.EnvironmentName);

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger, dispose: true);

        return builder;
    }

    public static WebApplicationBuilder UseFtObservabilityLogging(
        this WebApplicationBuilder builder,
        IConfigurationSection configurationSection)
    {
        var options = new FtObservabilityOptions();
        configurationSection.Bind(options);

        ValidateLoggingOptions(options);

        Log.Logger = CreateLogger(
            builder.Configuration,
            options,
            builder.Environment.EnvironmentName);

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger, dispose: true);

        return builder;
    }

    private static Serilog.Core.Logger CreateLogger(
        IConfiguration configuration,
        FtObservabilityOptions options,
        string environmentName)
    {
        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.OpenTelemetry(otel =>
            {
                otel.Endpoint = options.OtlpEndpoint;
                otel.Protocol = OtlpProtocol.Grpc;

                otel.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = options.ServiceName,
                    ["service.version"] = options.ServiceVersion,
                    ["deployment.environment"] = environmentName
                };
            })
            .CreateLogger();
    }

    private static void ValidateLoggingOptions(FtObservabilityOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ServiceName))
            throw new ArgumentException("ServiceName is required.", nameof(options));

        if (string.IsNullOrWhiteSpace(options.ServiceVersion))
            throw new ArgumentException("ServiceVersion is required.", nameof(options));

        if (string.IsNullOrWhiteSpace(options.OtlpEndpoint))
            throw new ArgumentException("OtlpEndpoint is required.", nameof(options));

        if (!Uri.TryCreate(options.OtlpEndpoint, UriKind.Absolute, out _))
            throw new ArgumentException("OtlpEndpoint must be a valid absolute URI.", nameof(options));
    }
}