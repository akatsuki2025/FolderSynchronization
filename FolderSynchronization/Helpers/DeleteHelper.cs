using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace FolderSynchronization.Helpers
{
    internal class DeleteHelper
    {
        public static void DeleteDirectorySafe(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Log.Information("Delete skipped: Directory {folderPath} does not exist", folderPath);
                return;
            }

            try
            {
                // Remove read-only attributes from all files
                var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }

                // Delete directory recursively
                Directory.Delete(folderPath, recursive: true);
                Log.Information("Successfully deleted directory: {folderPath}", folderPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete directory {folderPath}", folderPath);
                throw;
            }
        }
    }
}
