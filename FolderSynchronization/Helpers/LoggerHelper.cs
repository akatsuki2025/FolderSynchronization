using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Helpers
{
    internal class LoggerHelper
    {
        private static string _logFilePath;
        private static bool _initialized = false;

        public static bool Initialize(string logPath)
        {
            // TODO: opcjonalnie blokowanie obiektu
            // TODO: opcjonalnie watcher na wypadek usunięcia pliku w trakcie działania programu
            if (_initialized)
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Logger already initialized.");
                return true;
            }

            try
            {
                // Check if the path is a directory
                if (Directory.Exists(logPath))
                {
                    // Create default log file name with timestamp
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string defaultLogFileName = $"log_{timestamp}.log";
                    _logFilePath = Path.Combine(logPath, defaultLogFileName);
                }

                else
                {
                    // Path is a file path
                    _logFilePath = logPath;
                }

                _initialized = true;
                File.AppendAllText(_logFilePath, $"Log started at {DateTime.Now}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize log file: {ex.Message}");
                _initialized = false;
                return false;
            }
        }

        public static void Log(string message)
        {
            string timestampedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            Console.WriteLine(timestampedMessage);

            if (_initialized)
            {
                try
                {
                    File.AppendAllText(_logFilePath, timestampedMessage + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write to log file: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Warning: Logger not initialized, message only written to console");
            }
        }
    }
}
