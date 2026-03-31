namespace NetworkAnalyzerApp.Models;

public class UrlHistoryItem
{
    public string Url { get; set; } = string.Empty;
    public DateTime CheckedAt { get; set; }

    public string DisplayText => $"{CheckedAt:dd.MM.yyyy HH:mm:ss} — {Url}";
}