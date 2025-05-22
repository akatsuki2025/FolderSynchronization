using static FolderSynchronization.Helpers.ArgsValidationHelper;
using static FolderSynchronization.Helpers.FolderCopyingHelper;
using static FolderSynchronization.Helpers.SynchronizationHelper;
using static FolderSynchronization.Helpers.CleanupHelper;

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
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: FoderSynchronization <sourceFolderPath> <destinationFolderPath> <synchronizationIntervalSeconds>");
            return;
        }

        string sourcePath = args[0];
        string destinationParentPath = args[1];

        // Validate synchronization interval (in seconds) and assign it to syncIntervalSeconds
        if (!ValidateSyncInterval(args[2], out int syncIntervalSeconds))
            return;

        // Validate source and destination folder path
        if (!ValidateDirectory(sourcePath, "Source directory", out string sourceFolderPath) ||
                !ValidateDirectory(destinationParentPath, "Source directory", out string destinationFolderPath))
            return;

        // Get a source folder name
        string folderName = Path.GetFileName(sourceFolderPath.TrimEnd(Path.DirectorySeparatorChar));
        
        // Create a path to the replica folder in a following way: destinationFolderPath / folderName + "_copy"
        string destinationPath = Path.Combine(destinationFolderPath, folderName + "_copy");

        // PR: to sprawdzenie to raczej już w samej funkcji copy folder bym dał, i tak już tam masz create directory, które powinno być zaraz po Directory.Exists
        // Check if the folder already exists
        if (Directory.Exists(destinationPath))
        {
            Console.WriteLine($"Warning: The destination directory {destinationPath} aready exists and files may be overwritten.");
        }
        /*
        // Copy the folder
        try
        {
            CopyFolder(sourceFolderPath, destinationPath);
            Console.WriteLine($"The folder was successfully copied to {destinationPath}.");
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Error while copyting: {ex.Message}");
            PromptAndDeleteIncompleteCopy(destinationPath);
        }
        */

        while (true)
        {
            try
            {
                if (IsSyncNeeded(sourceFolderPath, destinationPath))
                {
                    CopyFolder(sourceFolderPath, destinationPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during synchronization: {ex.Message}");
                PromptAndDeleteIncompleteCopy(destinationPath);
            }

            Thread.Sleep(syncIntervalSeconds * 1000);
        }
    }
}