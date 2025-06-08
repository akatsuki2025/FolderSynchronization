using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSynchronization.Exceptions;
using Serilog;

namespace FolderSynchronization.Validators
{
    public static class DirectoryValidator
    {
        public static void ValidateSourceDirectory(string normalizedSourcePath)
        {
            try
            { 
                AccessValidator.ValidateReadAccess(normalizedSourcePath);
            }
            catch (ValidationException ex)
            {
                Log.Error(ex, "Source folder validation failed: {Error}", ex.Message);
                throw;
            }
        }

        public static void ValidateDestinationDirectory(string normalizedDestinationPath, string normalizedSourcePath)
        {
            try
            {
                FolderRelationshipValidator.ValidateSourceAndDestinationRelationship(normalizedSourcePath, normalizedDestinationPath);
                string? destinationParentPath = Path.GetDirectoryName(normalizedDestinationPath);
                AccessValidator.ValidateReadAccess(destinationParentPath);
            }
            catch (ValidationException ex)
            {
                Log.Error(ex, "Destination folder validation failed: {Error}", ex.Message);
                throw;
            }
        }

        public static void ValidateLogDirectory(string normalizedLogPath)
        {
            try
            {
                if (!Directory.Exists(normalizedLogPath))
                {
                    try
                    {
                        Directory.CreateDirectory(normalizedLogPath);
                        Console.WriteLine($"Created log directory: {normalizedLogPath}");
                    }
                    catch (Exception ex)
                    {
                        throw new ValidationException($"Cannot create log directory: {ex.Message}");
                    }
                }

                AccessValidator.ValidateWriteAccess(normalizedLogPath);
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"Log folder validation failed: {ex.Message}");
                throw;
            }
        }
    }
}
