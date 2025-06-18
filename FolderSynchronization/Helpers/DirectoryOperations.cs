using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Helpers
{
    internal static class DirectoryOperations
    {
        public static void EnsureDirectoryExists(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Log.Information("Created directory: {path}", path);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while creating directory {path}", path);
                throw;
            }
        }

        public static void RemoveExtraDirectories(string sourceFolder, string destinationFolder)
        {
            try
            {
                string[] sourceDirectoryNames = Directory.GetDirectories(sourceFolder)
                                                        .Select(fullPath => Path.GetFileName(fullPath))
                                                        .ToArray();
                string[] destinationDirectoryNames = Directory.GetDirectories(destinationFolder)
                                                        .Select(fullPath => Path.GetFileName(fullPath))
                                                        .ToArray();

                // Find directories in destination directory that don't exist in source directory
                foreach (string directoryNameToCheck in destinationDirectoryNames)
                {
                    if (!sourceDirectoryNames.Contains(directoryNameToCheck))
                    {
                        string directoryPathToDelete = Path.Combine(destinationFolder, directoryNameToCheck);
                        try
                        {
                            DeleteHelper.DeleteDirectorySafe(directoryPathToDelete);
                            Log.Information("Deleted directory that does not exist in source directory: {directoryNameToCheck}", directoryNameToCheck);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Failed to delete directory {directoryNameToCheck}", directoryNameToCheck);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error removing extra directories in {destinationFolder}", destinationFolder);
                throw;
            }
        }
    }
}
