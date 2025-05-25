using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Helpers
{
    internal class DeleteHelper
    {
        public static void DeleteDirectorySafe(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                LoggerHelper.Log($"Delete skipped: Directory {folderPath} does not exist");
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
                LoggerHelper.Log($"Successfully deleted directory: {folderPath}");
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Failed to delete directory {folderPath}: {ex.Message}");
                throw;
            }
        }
    }
}
