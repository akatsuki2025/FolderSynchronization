using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

                if (!CheckFileMd5(sourceFile, destinationFilePath))
                {
                    return true; 
                }

                // Check subfolders
                var sourceSubfolders = Directory.GetDirectories(sourceFolder);
                var destinationSubfolders = Directory.GetDirectories(destinationFolder);

                // Check if number of immediate subfolders is the same
                if (sourceSubfolders.Length != destinationSubfolders.Length)
                {
                    return true;
                }

                // Check if subfolders vary in content
                foreach (var sourceSubfolder in sourceSubfolders)
                {
                    string subfolderName = Path.GetFileName(sourceSubfolder);
                    string destinationSubfolderPath = Path.Combine(destinationFolder, subfolderName);

                    if (IsSyncNeeded(sourceSubfolder, destinationSubfolderPath))
                    {
                        return true;
                    }
                }
            }

            // Return false if no differences were found
            return false;
        }

        public static bool CheckFileMd5(string file1, string file2)
        {
            using var md5 = MD5.Create();

            using var stream1 = File.OpenRead(file1);
            using var stream2 = File.OpenRead(file2);

            var hash1 = md5.ComputeHash(stream1);
            var hash2 = md5.ComputeHash(stream2);

            return StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2);
        }
    }
}
