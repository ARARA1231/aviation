# Анализатор сетевых подключений

WPF-приложение для:
- просмотра сетевых интерфейсов компьютера;
- отображения IP, маски, MAC, состояния, скорости и типа интерфейса;
- анализа URL/URI;
- ping хоста;
- получения DNS-информации;
- определения типа адреса;
- сохранения истории проверенных URL.

## Запуск в VS Code

1. Установить .NET SDK 10 (или 9, если у тебя он уже стоит).
2. Установить расширения **C# Dev Kit** и **C#** в VS Code.
3. Открыть папку `NetworkAnalyzerSolution`.
4. В терминале выполнить:

```bash
dotnet restore
dotnet build .\NetworkAnalyzerSolution.sln
dotnet run --project .\NetworkAnalyzerApp\NetworkAnalyzerApp.csproj