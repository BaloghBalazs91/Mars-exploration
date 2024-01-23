namespace Codecool.MarsExploration.MapExplorer.Logger;

public interface ILogger
{
    void LogInfo(string message);

    void LogError(string message);

    string CreateEntry(string message, string type) => $"[{DateTime.Now}][{type}] - {message}";

}
