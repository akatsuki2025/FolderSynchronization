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
            Directory.CreateDirectory(destinationFolder);

            foreach (string filePath in Directory.GetFiles(sourceFolder))
            {
                string fileName = Path.GetFileName(filePath);
                string destinationFilePath = Path.Combine(destinationFolder, fileName);
                File.Copy(filePath, destinationFilePath, overwrite: true);
            }

            foreach (string folderPath in Directory.GetDirectories(sourceFolder))
            {
                string folderName = Path.GetFileName(folderPath);
                string destinationFolderName = Path.Combine(destinationFolder, folderName);
                CopyFolder(folderPath, destinationFolderName);
            }
        }
    }
}
