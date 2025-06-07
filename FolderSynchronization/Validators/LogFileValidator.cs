using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Validators
{
    public static class LogFileValidator
    {
        public static bool ValidateLogPath(string logPathInput, out string resolvedLogPath)
        {
            resolvedLogPath = string.Empty;

            if (PathValidator.IsEmptyOrWhitespace(logPathInput, "Log file path"))
            {
                return false;
            }

            try
            {
                // Check if the path is a directory
                if (DirectoryValidator.ValidateDirectory(logPathInput, "Log directory", out resolvedLogPath, suppressError: true))
                {
                    return true;
                }

                // Check if logPathInput is a valid file (.txt or .log)
                string fullLogFilePath = Path.GetFullPath(logPathInput);

                if (ValidateLogFileExtension(fullLogFilePath))
                {
                    if (!ValidateLogDirectoryPath(fullLogFilePath))
                    {
                        return false;
                    }

                    if (!EnsureLogDirectoryExists(fullLogFilePath))
                    {
                        return false;
                    }

                    if (!FileValidator.VerifyFileWriteAccess(fullLogFilePath))
                    {
                        return false;
                    }

                    resolvedLogPath = fullLogFilePath;
                    return true;
                }
                else
                {
                    Console.WriteLine("Error: Log file must have .log or .txt extension");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid log file path: {ex.Message}");
                return false;
            }
        }

        private static bool ValidateLogFileExtension(string logFilePath)
        {
            string extension = Path.GetExtension(logFilePath).ToLower();
            if (string.IsNullOrWhiteSpace(extension) || extension != ".log" && extension != ".txt")
            {
                Console.WriteLine("Error: Log file must have .log or .txt extension");
                return false;
            }
            return true;
        }

        private static bool ValidateLogDirectoryPath(string fullLogFilePath)
        {
            string? logDirectoryPath = Path.GetDirectoryName(fullLogFilePath);

            if (string.IsNullOrWhiteSpace(logDirectoryPath))
            {
                Console.WriteLine("Error: Invalid log file path - missing directory");
                return false;
            }
            return true;
        }

        private static bool EnsureLogDirectoryExists(string fullLogFilePath)
        {
            string? logDirectoryPath = Path.GetDirectoryName(fullLogFilePath);

            if (!Directory.Exists(logDirectoryPath))
            {
                try
                {
                    Directory.CreateDirectory(logDirectoryPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating log directory: {ex.Message}");
                    return false;
                }
            }
            return true;
        }
    }
}
