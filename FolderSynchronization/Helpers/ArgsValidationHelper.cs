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
        public static bool ValidateDirectory(string directoryPath, string directoryDescription, out string resolvedFullPath)
        {
            resolvedFullPath = string.Empty; 

            if (IsEmptyOrWhitespace(directoryPath, directoryDescription))
            {
                return false;
            }

            directoryPath = directoryPath.Trim();

            if (IsDriveLetterWithColonOnly(directoryPath, directoryDescription))
            {
                return false;
            }

            if (!ResolveAndValidateFullPath(directoryPath, directoryDescription, out resolvedFullPath))
            {
                return false;
            }

            if (!ConfirmDirectoryExists(resolvedFullPath, directoryPath, directoryDescription))
            {
                return false;
            }

            return true;
        }

        public static bool ValidateSynchronizationInterval(string intervalInput, out int synchronizationIntervalInSeconds)
        {
            if (!int.TryParse(intervalInput, out synchronizationIntervalInSeconds) || synchronizationIntervalInSeconds < 0)
            {
                Console.WriteLine("Error: Synchronization interval must be a non-negative integer (seconds).");
                return false;
            }
            return true;
        }

        public static bool ValidateLogPath(string logPathInput, out string resolvedLogPath)
        {
            resolvedLogPath = string.Empty;

            // Check if logPathInput is a valid directory
            if (ValidateDirectory(logPathInput, "Log directory", out string validatedLogDirectory))
            {
                resolvedLogPath = validatedLogDirectory;
                return true;
            }

            // Check if logPathInput is a valid file (.txt or .log)
            try
            {
                
                if (string.IsNullOrWhiteSpace(logPathInput))
                {
                    Console.WriteLine("Error: Log file path cannot be empty or whitespace.");
                    return false;
                }

                string fullLogPath = Path.GetFullPath(logPathInput);

                // Check file extension
                string? extension = Path.GetExtension(fullLogPath).ToLower();
                if (string.IsNullOrWhiteSpace(extension) || (extension != ".log" && extension != ".txt"))
                {
                    Console.WriteLine("Error: Log file must have .log or .txt extension");
                    return false;
                }

                // Check if directory exists or can be created
                string? logDirectoryPath = Path.GetDirectoryName(fullLogPath);
                if (string.IsNullOrWhiteSpace(logDirectoryPath))
                {
                     Console.WriteLine("Error: Invalid log file path - missing directory");
                     return false;
                }

                // Check persmissions - try to create directory if it doesn't exist
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

                // Check if the file existis and can be appended to
                if (File.Exists(fullLogPath))
                {
                    try
                    {
                        // Try to open the file for append to validate access
                        using (FileStream fs = new FileStream(fullLogPath, FileMode.Append))
                        {
                            // Testing permissions
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: Cannot write to existing log file: {ex.Message}");
                        return false;
                    }
                }

                resolvedLogPath = fullLogPath;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid log file path: {ex.Message}");
                return false;
            }
        }

        private static bool IsEmptyOrWhitespace(string path, string pathDescription)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine($"Error: {pathDescription} cannot be empty or whitespace.");
                return true;
            }
            return false;
        }

        private static bool IsDriveLetterWithColonOnly(string path, string pathDescription)
        {
            if (Regex.IsMatch(path, @"^[A-Za-z]:\s*$"))
            {
                Console.WriteLine($"Error: {pathDescription} '{path}' is not a valid folder path.");
                return true;
            }
            return false;
        }

        private static bool ResolveAndValidateFullPath(string directoryPath, string directoryDescription,out string resolvedFullPath)
        {
            resolvedFullPath = string.Empty;

            try
            {
                resolvedFullPath = Path.GetFullPath(directoryPath);

                // Check if the path is a drive root (e.g. "C:\")
                if (resolvedFullPath.Equals(Path.GetPathRoot(directoryPath), StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Error: {directoryDescription} '{directoryPath}' is a drive root. Copying an entire drive is not allowed.");
                    return false;
                }

                return true;
            }
            catch
            {
                Console.WriteLine($"Error: {directoryDescription} '{directoryPath}' is not a valid path.");
                return false;
            }
        }

        private static bool ConfirmDirectoryExists(string resolvedPath, string originalPath, string directoryDescription)
        {
            if (!Directory.Exists(resolvedPath))
            {
                Console.WriteLine($"Error: {directoryDescription} '{originalPath} does not exist.");
                return false;
            }
            return true;
        }
    }
}
