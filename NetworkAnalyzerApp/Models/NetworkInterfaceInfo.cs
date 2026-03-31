namespace NetworkAnalyzerApp.Models;

public class NetworkInterfaceInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IpAddress { get; set; } = "-";
    public string SubnetMask { get; set; } = "-";
    public string MacAddress { get; set; } = "-";
    public string Status { get; set; } = "-";
    public string Speed { get; set; } = "-";
    public string InterfaceType { get; set; } = "-";

    public string DisplayName => $"{Name} ({IpAddress})";
}