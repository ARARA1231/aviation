using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using NetworkAnalyzerApp.Models;
using NetworkAnalyzerApp.Services;

namespace NetworkAnalyzerApp;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly NetworkInfoService _networkInfoService = new();
    private readonly UrlAnalysisService _urlAnalysisService = new();
    private readonly HistoryService _historyService = new();

    private NetworkInterfaceInfo? _selectedNetworkInterface;
    private string _urlInput = string.Empty;
    private string _analysisOutput = "Здесь появятся результаты анализа URL.";

    public ObservableCollection<NetworkInterfaceInfo> NetworkInterfaces { get; } = new();
    public ObservableCollection<UrlHistoryItem> UrlHistory { get; } = new();

    public NetworkInterfaceInfo? SelectedNetworkInterface
    {
        get => _selectedNetworkInterface;
        set
        {
            _selectedNetworkInterface = value;
            OnPropertyChanged();
        }
    }

    public string UrlInput
    {
        get => _urlInput;
        set
        {
            _urlInput = value;
            OnPropertyChanged();
        }
    }

    public string AnalysisOutput
    {
        get => _analysisOutput;
        set
        {
            _analysisOutput = value;
            OnPropertyChanged();
        }
    }

    public MainWindow()
    {
        DataContext = this;

        LoadInterfaces();
        LoadHistory();
    }

    private void LoadInterfaces()
    {
        NetworkInterfaces.Clear();

        foreach (var item in _networkInfoService.GetAllInterfaces())
        {
            NetworkInterfaces.Add(item);
        }

        SelectedNetworkInterface = NetworkInterfaces.FirstOrDefault();
    }

    private void LoadHistory()
    {
        UrlHistory.Clear();

        foreach (var item in _historyService.LoadHistory())
        {
            UrlHistory.Add(item);
        }
    }

    private async void AnalyzeUrlButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UrlInput))
        {
            MessageBox.Show("Введите URL или доменное имя.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        AnalysisOutput = "Идет анализ...";

        var result = await _urlAnalysisService.AnalyzeAsync(UrlInput.Trim());
        AnalysisOutput = BuildResultText(result);

        if (result.IsSuccess)
        {
            _historyService.AddToHistory(UrlInput.Trim());
            LoadHistory();
        }
    }

    private void RefreshInterfacesButton_Click(object sender, RoutedEventArgs e)
    {
        LoadInterfaces();
        MessageBox.Show("Список сетевых интерфейсов обновлен.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        _historyService.ClearHistory();
        LoadHistory();
        MessageBox.Show("История очищена.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async void HistoryListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not System.Windows.Controls.ListBox listBox)
        {
            return;
        }

        if (listBox.SelectedItem is not UrlHistoryItem item)
        {
            return;
        }

        UrlInput = item.Url;

        var result = await _urlAnalysisService.AnalyzeAsync(item.Url);
        AnalysisOutput = BuildResultText(result);
    }

    private static string BuildResultText(UrlAnalysisResult result)
    {
        if (!result.IsSuccess)
        {
            return $"Ошибка: {result.ErrorMessage}";
        }

        return
$@"Исходный ввод: {result.OriginalInput}
Нормализованный URL: {result.NormalizedUrl}

==== Компоненты URL ====
Схема (протокол): {result.Scheme}
Хост: {result.Host}
Порт: {result.Port}
Путь: {result.Path}
Параметры запроса: {result.Query}
Фрагмент: {result.Fragment}

==== Проверка доступности ====
Ping: {result.PingStatus}

==== DNS-информация ====
{result.DnsInfo}

==== Тип адреса ====
{result.AddressType}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}