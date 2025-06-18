using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Serilog;


namespace FolderSynchronization.Helpers
{
    public static class SynchronizationHelper
    {
        public static void SynchronizeFolder(string sourceFolder, string destinationFolder)
        {
            try
            {
                // Create the destination folder if it doesn't exist
                DirectoryOperations.EnsureDirectoryExists(destinationFolder);

                // Process all files in the source directory
                SynchronizeFiles(sourceFolder, destinationFolder);

                // Process all subdirectories
                SynchronizeSubdirectories(sourceFolder, destinationFolder);
                
                // Remove files and directories from destination folder that don't exist in source folder
                FileOperations.RemoveExtraFiles(sourceFolder, destinationFolder);
                DirectoryOperations.RemoveExtraDirectories(sourceFolder, destinationFolder);
            }
            catch (Exception ex) 
            {
                Log.Error(ex, $"Error dusing synchronization");
                throw;
            }
        }

        private static void SynchronizeFiles(string sourceFolder, string destinationFolder)
        {
            var sourceFiles = Directory.GetFiles(sourceFolder);

            // Check if files in source folder exists in destination folder
            foreach (var sourceFile in sourceFiles)
            {
                try
                {
                    string fileName = Path.GetFileName(sourceFile);
                    string destinationFilePath = Path.Combine(destinationFolder, fileName);

                    bool needsCopy = false;

                    // Check if the file exists
                    if (!File.Exists(destinationFilePath))
                    {
                        needsCopy = true;
                    }
                    else
                    {
                        // Check if metadata is the same (size and timestamp)
                        var sourceInfo = new FileInfo(sourceFile);
                        var destinationInfo = new FileInfo(destinationFilePath);

                        if (sourceInfo.Length != destinationInfo.Length ||
                            sourceInfo.LastWriteTimeUtc != destinationInfo.LastWriteTimeUtc ||
                            !FileComparison.CheckFileMd5(sourceFile, destinationFilePath))
                        {
                            needsCopy = true;
                        }
                    }

                    // Copy files if needed
                    if (needsCopy)
                    {
                        FileOperations.CopyFile(sourceFile, destinationFilePath, overwrite: true);
                        // Set the same last write time to avoid unnecessary future copies
                        File.SetLastWriteTimeUtc(destinationFilePath, File.GetLastWriteTimeUtc(sourceFile));
                        Log.Information("Successfully copied file {fileName}", fileName);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex,"Error processing file {sourceFile}", sourceFile);
                }
            }
        }

        private static void SynchronizeSubdirectories(string sourceFolder, string destinationFolder)
        {
            foreach (var sourceSubfolder in Directory.GetDirectories(sourceFolder))
            {
                try
                {
                    string subfolderName = Path.GetFileName(sourceSubfolder);
                    string destinationSubfolderPath = Path.Combine(destinationFolder, subfolderName);

                    // Recursively suchronize the subdirectories
                    SynchronizeFolder(sourceSubfolder, destinationSubfolderPath);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error processing subdirectory {sourceSubfolder}", sourceSubfolder);
                    throw;
                }
            }
        }
    }
}
