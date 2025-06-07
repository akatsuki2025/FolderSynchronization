using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;

namespace FolderSynchronization.Validators
{
    public static class PathValidator
    {
        public static bool IsEmptyOrWhitespace(string path, string pathDescription)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine($"Error: {pathDescription} cannot be empty or whitespace.");
                return true;
            }
            return false;
        }

        public static bool IsDriveLetterWithColonOnly(string path, string pathDescription)
        {
            if (Regex.IsMatch(path, @"^[A-Za-z]:\s*$"))
            {
                Console.WriteLine($"Error: {pathDescription} '{path}' is not a valid folder path.");
                return true;
            }
            return false;
        }

        public static bool ResolveAndValidateFullPath(string directoryPath, string directoryDescription, out string resolvedFullPath)
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
    }
}
