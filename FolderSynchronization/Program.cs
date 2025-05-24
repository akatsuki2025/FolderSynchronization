using static FolderSynchronization.Helpers.ArgsValidationHelper;
using static FolderSynchronization.Helpers.FolderCopyingHelper;
using static FolderSynchronization.Helpers.SynchronizationHelper;
using static FolderSynchronization.Helpers.CleanupHelper;
using FolderSynchronization.Helpers;

class Program
{
    static void Main(string[] args)
    {
        /* To consider:
         * permissions
         * folders like C
         * check if there is enough space on disk
         * spacja na końcu "C: "
         */

        // Validate provided arguments
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: FolderSynchronization <sourceFolderPath> <destinationFolderPath> <synchronizationIntervalSeconds> <logFilePath>");
            return;
        }

        string sourcePathInput = args[0];
        string destinationParentPathInput = args[1];
        string synchronizationIntervalInput = args[2];
        string logFilePathInput = args[3];

        // Validate synchronization interval (in seconds) and assign it to syncIntervalSeconds
        if (!ValidateSynchronizationInterval(synchronizationIntervalInput, out int synchronizationIntervalSeconds))
            return;

        // Validate source and destination folder path
        if (!ValidateDirectory(sourcePathInput, "Source directory", out string validatedSourcePath) ||
                !ValidateDirectory(destinationParentPathInput, "Destination parent directory", out string validatedDestinationParentPath))
            return;

        // Validate log file path
        if (!ValidateLogPath(logFilePathInput, out string resolvedLogPath))
            return;

        LoggerHelper.Initialize(resolvedLogPath);

        // Get a source folder name and create a path to the replica folder in a following way: destinationFolderPath / folderName + "_copy"
        string sourceFolderName = Path.GetFileName(validatedSourcePath.TrimEnd(Path.DirectorySeparatorChar));
        string replicaFolderPath = Path.Combine(validatedDestinationParentPath, sourceFolderName + "_copy");

        while (true)
        {
            try
            {
                if (IsSyncNeeded(validatedSourcePath, replicaFolderPath))
                {
                    LoggerHelper.Log("Changes detected or initial copy required. Synchronizing...");
                    CopyFolder(validatedSourcePath, replicaFolderPath);
                    LoggerHelper.Log("Synchronization complete.");
                }
                else
                {
                    Console.WriteLine("No changes detected. Folders are already synchronized.");
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Error during synchronization: {ex.Message}");
                PromptAndDeleteIncompleteCopy(replicaFolderPath);
                return;
            }

            LoggerHelper.Log($"Next check scheduled in {synchronizationIntervalSeconds} seconds");
            LoggerHelper.Log("Listening for changes... Press Ctrl+C to exit");

            Thread.Sleep(synchronizationIntervalSeconds * 1000);
        }
    }
}