using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Helpers
{
    public static class SynchronizationHelper
    {
        public static bool IsSyncNeeded(string sourceFolder, string destinationFolder)
        {
            // Check if destination folder already exists
            if (!Directory.Exists(destinationFolder)) 
                return true;
            
            var sourceFiles = Directory.GetFiles(sourceFolder);
            var destinationFiles = Directory.GetFiles(destinationFolder);

            // Check if files in source folder exists in destination folder
            foreach (var sourceFile in sourceFiles)
            {
                string fileName = Path.GetFileName(sourceFile);
                string destinationFilePath = Path.Combine(destinationFolder, fileName);

                // Check if the file exists
                if (!File.Exists(destinationFilePath))
                {
                    return true;
                }

                // Check if metadata is the same (size and timestamp)
                var sourceInfo = new FileInfo(sourceFile);
                var destinationInfo = new FileInfo(destinationFilePath);

                if (sourceInfo.Length != destinationInfo.Length ||
                    sourceInfo.LastWriteTimeUtc != destinationInfo.LastWriteTimeUtc)
                {
                    return true;
                }

                // TODO: check md5



            }

            // Return false if no differences were found
            return false;
        }
    }
}
