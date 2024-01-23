using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecool.MarsExploration.MapExplorer.Logger
{
    internal class ConsoleLogger : ILogger
    {
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
            Console.WriteLine(entry);
        }

        private string CreateEntry(string message, string type) => $"[{DateTime.Now}][{type}] - {message}";
    }
}
