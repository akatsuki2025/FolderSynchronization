using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Helpers
{
    public static class FolderCopyingHelper
    {
        public static void CopyFolder(string sourceFolder, string destinationFolder)
        {
            try
            {
                Directory.CreateDirectory(destinationFolder);
                LoggerHelper.Log($"Successfully created folder: {destinationFolder}");
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Error: Failed to create folder {destinationFolder}: {ex.Message}");
                throw;
            }

            foreach (string filePath in Directory.GetFiles(sourceFolder))
            {
                string fileName = Path.GetFileName(filePath);
                string destinationFilePath = Path.Combine(destinationFolder, fileName);
                try
                {
                    File.Copy(filePath, destinationFilePath, overwrite: true);
                    LoggerHelper.Log($"Successfully copied file: {fileName}");
                }
                catch (Exception ex)
                {
                    LoggerHelper.Log($"Error: Failed to copy file {fileName}: {ex.Message}");
                    throw;
                }
            }

            foreach (string folderPath in Directory.GetDirectories(sourceFolder))
            {
                string folderName = Path.GetFileName(folderPath);
                string destinationFolderName = Path.Combine(destinationFolder, folderName);

                try
                {
                    CopyFolder(folderPath, destinationFolderName);
                }
                catch (Exception ex)
                {
                    LoggerHelper.Log($"Error: Failed to process subfolder {folderName}: {ex.Message}");
                }
            }
        }
    }
}
