using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Helpers
{
    internal static class FileOperations
    {
        private const long PROGRESS_REPORTING_THRESHOLD = 100 * 1024 * 1024; // 100MB
        private const long PROGRESS_LOG_INTERVAL = 10 * 1024 * 1024;         // 10MB
        private const double BYTES_TO_MB = 1024.0 * 1024.0;                  // MB conversion
        private const int DEFAULT_BUFFER_SIZE = 81920;                       // 80KB - default FileStream buffer

        public static void CopyFile(string sourceFile, string destinationFile, bool overwrite)
        {
            var fileInfo = new FileInfo(sourceFile);

            if (fileInfo.Length < PROGRESS_REPORTING_THRESHOLD)
            {
                File.Copy(sourceFile, destinationFile, overwrite);
                Log.Debug("Copied file {file} ({size}MB) to {destination}",
                    Path.GetFileName(sourceFile),
                    Math.Round(fileInfo.Length / BYTES_TO_MB, 2),
                    Path.GetFileName(destinationFile));
                return;
            }

            using (var source = File.OpenRead(sourceFile))
            using (var destination = File.Create(destinationFile))
            {
                var buffer = new byte[DEFAULT_BUFFER_SIZE];
                long totalBytesCopied = 0;
                long lastReportedProgress = 0;
                int bytesRead;

                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    destination.Write(buffer, 0, bytesRead);
                    totalBytesCopied += bytesRead;

                    if (totalBytesCopied - lastReportedProgress >= PROGRESS_LOG_INTERVAL)
                    {
                        lastReportedProgress = (totalBytesCopied / PROGRESS_LOG_INTERVAL) * PROGRESS_LOG_INTERVAL;

                        Log.Information("Copying large file {file}: {copied}MB of {total}MB ({percentage}%)",
                            Path.GetFileName(sourceFile),
                            Math.Round(totalBytesCopied / BYTES_TO_MB, 2),
                            Math.Round(fileInfo.Length / BYTES_TO_MB, 2),
                            Math.Round((totalBytesCopied * 100.0) / fileInfo.Length, 1));
                    }
                }
            }
        }

        public static void RemoveExtraFiles(string sourceFolder, string destinationFolder)
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
                            Log.Information("Deleted extra file: {destinationFileName} from destination folder", destinationFileName);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Failed to delete file {destinationFileName}", destinationFileName);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error removing extra files in {destinationFolder}", destinationFolder);
                throw;
            }
        }
    }
}
