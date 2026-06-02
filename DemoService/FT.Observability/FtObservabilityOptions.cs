namespace FT.Observability;

public sealed class FtObservabilityOptions
{
    public string ServiceName { get; set; } = "";
    public string ServiceVersion { get; set; } = FtObservabilityDefaults.DefaultServiceVersion;
    public string ActivitySourceName { get; set; } = "";
    public string OtlpEndpoint { get; set; } = FtObservabilityDefaults.DefaultOtlpEndpoint;
}