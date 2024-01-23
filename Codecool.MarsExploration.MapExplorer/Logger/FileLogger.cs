using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.Logger
{
    internal class FileLogger : ILogger
    {
        private readonly object _lockObject = new object();
        private static readonly string WorkDir = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _filePath = Path.Combine(WorkDir, "Resources", "FileLogger.txt");
        public void LogError(string message)
        {
            LogMessage(message, "ERROR");
        }

        public void LogInfo(string message)
        {
            LogMessage(message, "INFO");
        }

        private void LogMessage(string message, string type)
        {
            string entry = CreateEntry(message, type);

            lock (_lockObject)
            {
                using (StreamWriter sw = new StreamWriter(_filePath, true))
                {
                    sw.WriteLine(entry);
                }
        }
    }

        private string CreateEntry(string message, string type) => $"[{DateTime.Now}][{type}] - {message}";
    }
}
