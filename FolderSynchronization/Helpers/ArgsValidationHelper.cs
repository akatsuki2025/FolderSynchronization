using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FolderSynchronization.Helpers
{
    public static class ArgsValidationHelper
    {
        public static bool ValidateDirectory(string path, string pathName, out string fullPath)
        {
            fullPath = null;

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine($"Error: {pathName} cannot be empty or shitespce.");
                return false;
            }

            path = path.Trim();

            if (Regex.IsMatch(path, @"^[A-Za-z]:\s*$"))
            {
                Console.WriteLine($"Error: {pathName} '{path}' is not a valid folder path.");
                return false;
            }

            try
            {
                fullPath = Path.GetFullPath(path);
                if (fullPath.Equals(Path.GetPathRoot(pathName), StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Error: {pathName} '{path}' is a drive root. Copying an entire drive is not allowed.");
                    return false;
                }
            }
            catch
            {
                Console.WriteLine($"Error: {pathName} '{path}' is not a valid path.");
                return false;
            }

            if (!Directory.Exists(fullPath))
            {
                Console.WriteLine($"Error: {pathName} '{path} does not exist.");
                return false;
            }

            return true;
        }

        public static bool ValidateSyncInterval(string input, out int syncIntervalSeconds)
        {
            if (!int.TryParse(input, out syncIntervalSeconds) || syncIntervalSeconds < 0)
            {
                Console.WriteLine("Error: Synchronization interval must be a non-negative integer (seconds).");
                return false;
            }
            return true;
        }

        public static bool ValidateLogFilePath(string logFilePath)
        {
            if (string.IsNullOrWhiteSpace(logFilePath)) 
            {
                Console.WriteLine("Error: Log file path cannot be empty or whitespace.");
                return false;
            }

            try
            {
                string fullLogPath = Path.GetFullPath(logFilePath);
                string logFolder = Path.GetDirectoryName(fullLogPath);

                if (string.IsNullOrWhiteSpace(logFolder))
                {
                    Console.WriteLine("Error: Log file must include a valid directory");
                    return false;
                }

                return true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error: Invalid log file path {logFilePath}: {ex.Message}");
                return false;
            }
        }
    }
}
