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

            if (PathValidator.IsEmptyOrWhitespace(logPathInput, "Log directory path"))
            {
                return false;
            }

            try
            {
                resolvedLogPath = Path.GetFullPath(logPathInput);

                if (!DirectoryValidator.ValidateDirectory(logPathInput, "Log directory", out resolvedLogPath, suppressError: true))
                {
                    Console.WriteLine($"Invalid log directory path {logPathInput}");
                    return false;
                }

                return true;   
     
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating log directory: {ex.Message}");
                return false;
            }
        }
    }
}
