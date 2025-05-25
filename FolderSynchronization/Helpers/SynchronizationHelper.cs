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
        public static void SynchronizeFolder(string sourceFolder, string destinationFolder)
        {
            try
            {
                // Create the destination folder if it doesn't exist
                EnsureDirectoryExists(destinationFolder);

                // Process all files in the source directory
                SynchronizeFiles(sourceFolder, destinationFolder);

                // Process all subdirectories
                SynchronizeSubdirectories(sourceFolder, destinationFolder);
                
                // Remove files and directories from destination folder that don't exist in source folder
                RemoveExtraFiles(sourceFolder, destinationFolder);
                RemoveExtraDirectories(sourceFolder, destinationFolder);
            }
            catch (Exception ex) 
            {
                LoggerHelper.Log($"Error dusing synchronization: {ex.Message}");
                throw;
            }
        }

        private static void EnsureDirectoryExists(string destinationFolder)
        {
            try
            {
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                    LoggerHelper.Log($"Created directory: {destinationFolder}");
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Error while creating directory '{destinationFolder}': {ex.Message}");
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
                            !CheckFileMd5(sourceFile, destinationFilePath))
                        {
                            needsCopy = true;
                        }
                    }

                    // Copy files if needed
                    if (needsCopy)
                    {
                        File.Copy(sourceFile, destinationFilePath, overwrite: true);
                        // Set the same last write time to avoid unnecessary future copies
                        File.SetLastWriteTimeUtc(destinationFilePath, File.GetLastWriteTimeUtc(sourceFile));
                        LoggerHelper.Log($"Successfully copied file '{fileName}'");
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Log($"Error processing file {sourceFile}: {ex.Message}");
                }
            }
        }

        private static bool CheckFileMd5(string file1, string file2)
        {
            using var md5 = MD5.Create();

            using var stream1 = File.OpenRead(file1);
            using var stream2 = File.OpenRead(file2);

            var hash1 = md5.ComputeHash(stream1);
            var hash2 = md5.ComputeHash(stream2);

            return StructuralComparisons.StructuralEqualityComparer.Equals(hash1, hash2);
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
                    LoggerHelper.Log($"Error processing subdirectory '{sourceSubfolder}': {ex.Message}");
                    throw;
                }
            }
        }

        private static void RemoveExtraFiles(string sourceFolder, string destinationFolder)
        {
            try
            {
                string[] sourceFiles = Directory.GetFiles(sourceFolder).Select(Path.GetFileName).ToArray();
                string[] destinationFiles = Directory.GetFiles(destinationFolder).Select(Path.GetFileName).ToArray();

                // Find files in destination folder that don't exist in source folder
                foreach (string destinationFileName in destinationFiles)
                {
                    if (!sourceFiles.Contains(destinationFileName))
                    {
                        string fileToDelete = Path.Combine(destinationFolder, destinationFileName);
                        try
                        {
                            File.Delete(fileToDelete);
                            LoggerHelper.Log($"Deleted extra file: '{destinationFileName}' from destination folder");
                        }
                        catch (Exception ex)
                        {
                            LoggerHelper.Log($"Error: Failed to delete file '{destinationFileName}': {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Error removing extra files in '{destinationFolder}': {ex.Message}");
                throw;
            }
        }

        private static void RemoveExtraDirectories(string sourceFolder, string destinationFolder)
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
                            LoggerHelper.Log($"Deleted directory that does not exist in source directory: '{directoryNameToCheck}'");
                        }
                        catch (Exception ex)
                        {
                            LoggerHelper.Log($"Error: Failed to delete directory {directoryNameToCheck}: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Error removing extra directories in {destinationFolder}: {ex.Message}");
                throw;
            }
        }
    }
}
