using System.Diagnostics;

namespace FT.Observability;

public sealed class FtObservabilityProvider
{
    public string ServiceName { get; }
    public string ServiceVersion { get; }
    public string ActivitySourceName { get; }
    public ActivitySource ActivitySource { get; }

    public FtObservabilityProvider(FtObservabilityOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ServiceName))
            throw new ArgumentException("ServiceName must be provided.", nameof(options));

        if (string.IsNullOrWhiteSpace(options.ActivitySourceName))
            throw new ArgumentException("ActivitySourceName must be provided.", nameof(options));

        ServiceName = options.ServiceName;
        ServiceVersion = options.ServiceVersion;
        ActivitySourceName = options.ActivitySourceName;
        ActivitySource = new ActivitySource(ActivitySourceName);
    }
}