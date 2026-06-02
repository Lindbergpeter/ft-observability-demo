using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FT.Observability;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFtObservabilityTracing(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        var options = new FtObservabilityOptions();
        configurationSection.Bind(options);

        return services.AddFtObservabilityTracing(options);
    }

    public static IServiceCollection AddFtObservabilityTracing(
        this IServiceCollection services,
        Action<FtObservabilityOptions> configure)
    {
        var options = new FtObservabilityOptions();
        configure(options);

        return services.AddFtObservabilityTracing(options);
    }

    private static IServiceCollection AddFtObservabilityTracing(
        this IServiceCollection services,
        FtObservabilityOptions options)
    {
        ValidateOptions(options);

        services.AddSingleton(options);
        services.AddSingleton<FtObservabilityProvider>();

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetSampler(new AlwaysOnSampler())
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                serviceName: options.ServiceName,
                                serviceVersion: options.ServiceVersion))
                    .AddSource(options.ActivitySourceName)
                    .AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = new Uri(options.OtlpEndpoint);
                        exporter.Protocol = OtlpExportProtocol.Grpc;
                    });
            });

        return services;
    }

    private static void ValidateOptions(FtObservabilityOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ServiceName))
            throw new ArgumentException("ServiceName is required.", nameof(options));

        if (string.IsNullOrWhiteSpace(options.ServiceVersion))
            throw new ArgumentException("ServiceVersion is required.", nameof(options));

        if (string.IsNullOrWhiteSpace(options.ActivitySourceName))
            throw new ArgumentException("ActivitySourceName is required.", nameof(options));

        if (string.IsNullOrWhiteSpace(options.OtlpEndpoint))
            throw new ArgumentException("OtlpEndpoint is required.", nameof(options));

        if (!Uri.TryCreate(options.OtlpEndpoint, UriKind.Absolute, out _))
            throw new ArgumentException("OtlpEndpoint must be a valid absolute URI.", nameof(options));
    }
}