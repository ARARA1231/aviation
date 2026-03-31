namespace NetworkAnalyzerApp.Models;

public class UrlAnalysisResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public string OriginalInput { get; set; } = string.Empty;
    public string NormalizedUrl { get; set; } = string.Empty;
    public string Scheme { get; set; } = "-";
    public string Host { get; set; } = "-";
    public string Port { get; set; } = "-";
    public string Path { get; set; } = "-";
    public string Query { get; set; } = "-";
    public string Fragment { get; set; } = "-";
    public string PingStatus { get; set; } = "-";
    public string DnsInfo { get; set; } = "-";
    public string AddressType { get; set; } = "-";
}