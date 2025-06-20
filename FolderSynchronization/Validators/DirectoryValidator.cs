using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSynchronization.Exceptions;
using Serilog;

namespace FolderSynchronization.Validators
{
    public static class DirectoryValidator
    {
        public static void ValidateSourceDirectory(string normalizedSourcePath, IAccessValidator accessValidator)
        {
            try
            { 
                accessValidator.ValidateReadAccess(normalizedSourcePath);
            }
            catch (ValidationException ex)
            {
                Log.Error(ex, "Source folder validation failed: {Error}", ex.Message);
                throw;
            }
        }

        public static void ValidateDestinationDirectory(string normalizedDestinationPath, string normalizedSourcePath, IAccessValidator accessValidator)
        {
            try
            {
                FolderRelationshipValidator.ValidateSourceAndDestinationRelationship(normalizedSourcePath, normalizedDestinationPath);
                string? destinationParentPath = Path.GetDirectoryName(normalizedDestinationPath);
                accessValidator.ValidateReadAccess(destinationParentPath);
            }
            catch (ValidationException ex)
            {
                Log.Error(ex, "Destination folder validation failed: {Error}", ex.Message);
                throw;
            }
        }

        public static void ValidateLogDirectory(string normalizedLogPath, IAccessValidator accessValidator, IFileSystem fileSystem)
        {
            try
            {
                if (!fileSystem.Directory.Exists(normalizedLogPath))
                {
                    try
                    {
                        fileSystem.Directory.CreateDirectory(normalizedLogPath);
                        Console.WriteLine($"Created log directory: {normalizedLogPath}");
                    }
                    catch (Exception ex)
                    {
                        throw new ValidationException($"Cannot create log directory: {ex.Message}");
                    }
                }

                accessValidator.ValidateWriteAccess(normalizedLogPath);
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"Log folder validation failed: {ex.Message}");
                throw;
            }
        }
    }
}
