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
            Console.WriteLine("Usage: FoderSynchronization <sourceFolderPath> <destinationFolderPath> <synchronizationIntervalSeconds> <logFilePath>");
            return;
        }

        string sourcePath = args[0];
        string destinationParentPath = args[1];
        string syncIntervalArg = args[2];
        string logFilePath = args[3];

        // Validate synchronization interval (in seconds) and assign it to syncIntervalSeconds
        if (!ValidateSyncInterval(syncIntervalArg, out int syncIntervalSeconds))
            return;

        // Validate source and destination folder path
        if (!ValidateDirectory(sourcePath, "Source directory", out string sourceFolderPath) ||
                !ValidateDirectory(destinationParentPath, "Source directory", out string destinationFolderPath))
            return;

        // Validate log file path
        if (!ValidateLogFilePath(logFilePath))
            return;

        LoggerHelper.Initialize(logFilePath);

        // Get a source folder name and create a path to the replica folder in a following way: destinationFolderPath / folderName + "_copy"
        string folderName = Path.GetFileName(sourceFolderPath.TrimEnd(Path.DirectorySeparatorChar));
        string destinationPath = Path.Combine(destinationFolderPath, folderName + "_copy");

        // PR: to sprawdzenie to raczej już w samej funkcji copy folder bym dał, i tak już tam masz create directory, które powinno być zaraz po Directory.Exists
        // Check if the folder already exists
        /*if (Directory.Exists(destinationPath))
        {
            Console.WriteLine($"Warning: The destination directory {destinationPath} aready exists and files may be overwritten.");
        }*/

        while (true)
        {
            try
            {
                if (IsSyncNeeded(sourceFolderPath, destinationPath))
                {
                    LoggerHelper.Log("Changes detected or initial copy required. Synchronizing...");
                    CopyFolder(sourceFolderPath, destinationPath);
                    LoggerHelper.Log("Synchronization complete.");
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Error during synchronization: {ex.Message}");
                PromptAndDeleteIncompleteCopy(destinationPath);
                return;
            }

            Thread.Sleep(syncIntervalSeconds * 1000);
        }
    }
}