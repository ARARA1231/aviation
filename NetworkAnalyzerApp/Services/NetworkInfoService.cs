using System.Net.NetworkInformation;
using System.Net.Sockets;
using NetworkAnalyzerApp.Models;

namespace NetworkAnalyzerApp.Services;

public class NetworkInfoService
{
    public List<NetworkInterfaceInfo> GetAllInterfaces()
    {
        var result = new List<NetworkInterfaceInfo>();

        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            var ipProperties = networkInterface.GetIPProperties();

            var ipv4Address = ipProperties.UnicastAddresses
                .FirstOrDefault(address => address.Address.AddressFamily == AddressFamily.InterNetwork);

            var ip = ipv4Address?.Address?.ToString() ?? "Нет IPv4";
            var mask = ipv4Address?.IPv4Mask?.ToString() ?? "-";

            result.Add(new NetworkInterfaceInfo
            {
                Name = networkInterface.Name,
                Description = networkInterface.Description,
                IpAddress = ip,
                SubnetMask = mask,
                MacAddress = FormatMacAddress(networkInterface.GetPhysicalAddress()),
                Status = networkInterface.OperationalStatus.ToString(),
                Speed = FormatSpeed(networkInterface.Speed),
                InterfaceType = networkInterface.NetworkInterfaceType.ToString()
            });
        }

        return result
            .OrderBy(item => item.Name)
            .ToList();
    }

    private static string FormatMacAddress(PhysicalAddress address)
    {
        var bytes = address.GetAddressBytes();

        if (bytes.Length == 0)
        {
            return "-";
        }

        return string.Join("-", bytes.Select(b => b.ToString("X2")));
    }

    private static string FormatSpeed(long bitsPerSecond)
    {
        if (bitsPerSecond <= 0)
        {
            return "-";
        }

        var megabitsPerSecond = bitsPerSecond / 1_000_000d;
        return $"{megabitsPerSecond:0.##} Мбит/с";
    }
}