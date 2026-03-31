using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NetworkAnalyzerApp.Models;

namespace NetworkAnalyzerApp.Services;

public class UrlAnalysisService
{
    public async Task<UrlAnalysisResult> AnalyzeAsync(string input)
    {
        var result = new UrlAnalysisResult
        {
            OriginalInput = input
        };

        var normalizedInput = NormalizeInput(input);

        if (!Uri.TryCreate(normalizedInput, UriKind.Absolute, out var uri))
        {
            result.IsSuccess = false;
            result.ErrorMessage = "Не удалось распознать введенный URL/URI.";
            return result;
        }

        result.IsSuccess = true;
        result.NormalizedUrl = uri.AbsoluteUri;
        result.Scheme = uri.Scheme;
        result.Host = uri.Host;
        result.Port = uri.IsDefaultPort ? "Стандартный" : uri.Port.ToString();
        result.Path = string.IsNullOrWhiteSpace(uri.AbsolutePath) ? "/" : uri.AbsolutePath;
        result.Query = string.IsNullOrWhiteSpace(uri.Query) ? "-" : uri.Query;
        result.Fragment = string.IsNullOrWhiteSpace(uri.Fragment) ? "-" : uri.Fragment;

        result.PingStatus = await GetPingStatusAsync(uri.Host);
        result.DnsInfo = await GetDnsInfoAsync(uri.Host);
        result.AddressType = await GetAddressTypeAsync(uri.Host);

        return result;
    }

    private static string NormalizeInput(string input)
    {
        if (input.Contains("://"))
        {
            return input;
        }

        return $"https://{input}";
    }

    private static async Task<string> GetPingStatusAsync(string host)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(host, 2000);

            return reply.Status switch
            {
                IPStatus.Success => $"Доступен, время = {reply.RoundtripTime} мс",
                _ => $"Недоступен ({reply.Status})"
            };
        }
        catch (Exception ex)
        {
            return $"Ошибка ping: {ex.Message}";
        }
    }

    private static async Task<string> GetDnsInfoAsync(string host)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(host);

            if (addresses.Length == 0)
            {
                return "DNS-записи не найдены.";
            }

            return string.Join(Environment.NewLine, addresses.Select(address => address.ToString()));
        }
        catch (Exception ex)
        {
            return $"Ошибка DNS: {ex.Message}";
        }
    }

    private static async Task<string> GetAddressTypeAsync(string host)
    {
        try
        {
            IPAddress? ipAddress = null;

            if (IPAddress.TryParse(host, out var parsedAddress))
            {
                ipAddress = parsedAddress;
            }
            else
            {
                var dnsAddresses = await Dns.GetHostAddressesAsync(host);
                ipAddress = dnsAddresses.FirstOrDefault();
            }

            if (ipAddress is null)
            {
                return "Не удалось определить адрес.";
            }

            if (IPAddress.IsLoopback(ipAddress))
            {
                return $"Loopback ({ipAddress})";
            }

            if (IsPrivateOrLocal(ipAddress))
            {
                return $"Локальный/частный ({ipAddress})";
            }

            return $"Публичный ({ipAddress})";
        }
        catch (Exception ex)
        {
            return $"Ошибка определения типа адреса: {ex.Message}";
        }
    }

    private static bool IsPrivateOrLocal(IPAddress address)
    {
        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
            var bytes = address.GetAddressBytes();

            if (bytes[0] == 10)
            {
                return true;
            }

            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
            {
                return true;
            }

            if (bytes[0] == 192 && bytes[1] == 168)
            {
                return true;
            }

            if (bytes[0] == 169 && bytes[1] == 254)
            {
                return true;
            }

            return false;
        }

        if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            if (address.IsIPv6LinkLocal || address.IsIPv6SiteLocal)
            {
                return true;
            }

            var bytes = address.GetAddressBytes();
            return bytes[0] is 0xFC or 0xFD;
        }

        return false;
    }
}