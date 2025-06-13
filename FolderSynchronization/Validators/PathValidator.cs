using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;
using FolderSynchronization.Exceptions;

namespace FolderSynchronization.Validators
{
    public static class PathValidator
    {
        public static string ValidatePath(string path, string pathDescription, bool allowDriveRoot)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ValidationException($"{pathDescription} cannot be empty or whitespace");

            string trimmedPath = path.Trim();

            if (Regex.IsMatch(trimmedPath, @"^\s*[A-Za-z](:)?\s*$"))
                throw new ValidationException($"{pathDescription} '{path}' cannot be just a drive letter");

            const string invalidChars = "\"<>|*?";

            if (trimmedPath.IndexOfAny(invalidChars.ToCharArray()) >= 0)
                throw new ValidationException($"Invalid {pathDescription}: The path contains invalid characters.");

            try
            {
                string fullPath = Path.GetFullPath(trimmedPath);

                if (!allowDriveRoot && fullPath.Equals(Path.GetPathRoot(path), StringComparison.OrdinalIgnoreCase))
                    throw new ValidationException($"{pathDescription} '{path}' cannot be a drive root");

                return fullPath;
            }
            catch (Exception ex) 
            {
                throw new ValidationException($"Invalid {pathDescription}: {ex.Message}");
            }
        }
    }
}
