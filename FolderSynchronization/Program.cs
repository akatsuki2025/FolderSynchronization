using static FolderSynchronization.Helpers.PathValidationHelper;
using static FolderSynchronization.Helpers.FolderCopyingHelper;
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
        int syncIntervalSeconds = 0;

        // Validate synchronization interval (in seconds) and assign it to syncIntervalSeconds
        if (args.Length == 3 && (!int.TryParse(args[2], out syncIntervalSeconds) || syncIntervalSeconds <= 0) )
        {
            Console.WriteLine("Error: Synchronization interval must be a positive integer (seconds).");
            return;
        }

        // Validate source and destination folder path
        if (!ValidateDirectory(sourcePath, "Source directory", out string sourceFolderPath) ||
                !ValidateDirectory(destinationParentPath, "Source directory", out string destinationFolderPath))
            return;

        // Get folder name
        string folderName = Path.GetFileName(sourceFolderPath.TrimEnd(Path.DirectorySeparatorChar));
        
        // Create a path to the replica folder in a following way: destinationFolderPath / folderName + "_copy"
        string destinationPath = Path.Combine(destinationFolderPath, folderName + "_copy");

        // Check if the foder already exists
        if (Directory.Exists(destinationPath))
        {
            Console.WriteLine($"Warning: The destination directory {destinationPath} aready exists and files may be overwritten.");
        }

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
    }
}