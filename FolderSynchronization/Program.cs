using FolderSynchronization.Validators;
using FolderSynchronization.Helpers;
using Serilog;

class Program
{
    static void Main(string[] args)
    {
        // Validate provided arguments
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: FolderSynchronization <sourceFolderPath> <destinationFolderPath> <synchronizationIntervalSeconds> <logFolderPath>");
            return;
        }

        string sourcePathInput = args[0];
        string destinationParentPathInput = args[1];
        string synchronizationIntervalInput = args[2];
        string logPathInput = args[3];

        // Validate log file path and initialize the logger
        if (!LogFileValidator.ValidateLogPath(logPathInput, out string resolvedLogPath))
        {
            Console.WriteLine("Invalid log path. Exiting.");
            return;
        }
        try
        {
            FolderSynchronization.Configuration.LoggerConfiguration.ConfigureSerilog(resolvedLogPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to configure logging: " + ex.Message);
            return;
        }

        // Validate synchronization interval (in seconds) and assign it to syncIntervalSeconds
        if (!SynchronizationIntervalValidator.ValidateSynchronizationInterval(synchronizationIntervalInput, out int synchronizationIntervalSeconds))
            return;

        // Validate source and destination folder path
        if (!DirectoryValidator.ValidateDirectory(sourcePathInput, "Source directory", out string validatedSourcePath) ||
                !DirectoryValidator.ValidateDirectory(destinationParentPathInput, "Destination parent directory", out string validatedDestinationParentPath))
            return;

        // Get a source folder name and create a path to the replica folder in a following way: destinationFolderPath / folderName + "_copy"
        string sourceFolderName = Path.GetFileName(validatedSourcePath.TrimEnd(Path.DirectorySeparatorChar));
        string replicaFolderPath = Path.Combine(validatedDestinationParentPath, sourceFolderName + "_copy");

        try
        {
            while (true)
            {
                try
                {
                    Log.Information("Synchronizing folders at {DateTime}", DateTime.Now);

                    SynchronizationHelper.SynchronizeFolder(validatedSourcePath, replicaFolderPath);

                    Log.Information("Synchronization complete.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error during synchronization");
                    CleanupHelper.PromptAndDeleteIncompleteCopy(replicaFolderPath);
                    return;
                }

                Log.Information("Next check scheduled in {synchronizationIntervalSeconds} seconds", synchronizationIntervalSeconds);
                Log.Information("Listening for changes... Press Ctrl+C to exit");

                Thread.Sleep(synchronizationIntervalSeconds * 1000);
            }
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}