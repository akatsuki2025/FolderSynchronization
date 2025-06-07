using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Validators
{
    public static class DirectoryValidator
    {
        public static bool ValidateDirectory(string directoryPath, string directoryDescription, out string resolvedFullPath, bool suppressError = false)
        {
            resolvedFullPath = string.Empty;

            if (PathValidator.IsEmptyOrWhitespace(directoryPath, directoryDescription))
            {
                return false;
            }

            directoryPath = directoryPath.Trim();

            if (PathValidator.IsDriveLetterWithColonOnly(directoryPath, directoryDescription))
            {
                return false;
            }

            if (!PathValidator.ResolveAndValidateFullPath(directoryPath, directoryDescription, out resolvedFullPath))
            {
                return false;
            }

            if (!ConfirmDirectoryExists(resolvedFullPath, directoryPath, directoryDescription, suppressError))
            {
                return false;
            }

            return true;
        }

        public static bool ConfirmDirectoryExists(string resolvedPath, string originalPath, string directoryDescription, bool suppressError = false)
        {
            if (!Directory.Exists(resolvedPath))
            {
                if (!suppressError)
                {
                    Console.WriteLine($"Error: {directoryDescription} '{originalPath}' does not exist.");
                }
                return false;
            }
            return true;
        }
    }
}
