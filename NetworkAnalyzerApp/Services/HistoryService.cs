using System.IO;
using System.Text.Json;
using NetworkAnalyzerApp.Models;

namespace NetworkAnalyzerApp.Services;

public class HistoryService
{
    private readonly string _historyFilePath;

    public HistoryService()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NetworkAnalyzerApp");

        Directory.CreateDirectory(folder);
        _historyFilePath = Path.Combine(folder, "url_history.json");
    }

    public List<UrlHistoryItem> LoadHistory()
    {
        try
        {
            if (!File.Exists(_historyFilePath))
            {
                return new List<UrlHistoryItem>();
            }

            var json = File.ReadAllText(_historyFilePath);
            var items = JsonSerializer.Deserialize<List<UrlHistoryItem>>(json);

            return items?
                .OrderByDescending(item => item.CheckedAt)
                .ToList()
                ?? new List<UrlHistoryItem>();
        }
        catch
        {
            return new List<UrlHistoryItem>();
        }
    }

    public void AddToHistory(string url)
    {
        var history = LoadHistory();

        history.Insert(0, new UrlHistoryItem
        {
            Url = url,
            CheckedAt = DateTime.Now
        });

        history = history
            .GroupBy(item => item.Url, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.OrderByDescending(item => item.CheckedAt).First())
            .OrderByDescending(item => item.CheckedAt)
            .Take(50)
            .ToList();

        SaveHistory(history);
    }

    public void ClearHistory()
    {
        SaveHistory(new List<UrlHistoryItem>());
    }

    private void SaveHistory(List<UrlHistoryItem> items)
    {
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_historyFilePath, json);
    }
}