using FolderSynchronization.Validators;
using FolderSynchronization.Helpers;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

class Program
{
    private const int EXPECTED_ARGUMENT_COUNT = 4;

    static void Main(string[] args)
    {
        if (args.Length != EXPECTED_ARGUMENT_COUNT)
        {
            Console.WriteLine("Usage: FolderSynchronization <sourceFolderPath> <destinationFolderPath> <synchronizationIntervalSeconds> <logFolderPath>");
            return;
        }

        string sourcePathInput = args[0];
        string destinationParentPathInput = args[1];
        string synchronizationIntervalInput = args[2];
        string logPathInput = args[3];

        try
        {
            // 1. Validate log path and set up the logger
            string normalizedLogPath = PathValidator.ValidatePath(logPathInput, "Log directory", allowDriveRoot: true);
            DirectoryValidator.ValidateLogDirectory(normalizedLogPath);
            
            try
            {
                FolderSynchronization.Configuration.LoggerConfiguration.ConfigureSerilog(normalizedLogPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to configure logger: {ex.Message}");
                return;
            }

            // 2. Validate source folder and get normalized path
            string normalizedSourcePath = PathValidator.ValidatePath(sourcePathInput, "Source directory", allowDriveRoot: false);
            DirectoryValidator.ValidateSourceDirectory(normalizedSourcePath);

            // 3. Validate destination folder and get normalized path
            string normalizedDestinationPath = PathValidator.ValidatePath(destinationParentPathInput, "Destination directory", allowDriveRoot: false);
            string sourceFolderName = Path.GetFileName(normalizedSourcePath.TrimEnd(Path.DirectorySeparatorChar));
            string replicaFolderPath = Path.Combine(normalizedDestinationPath, sourceFolderName + "_copy");
            DirectoryValidator.ValidateDestinationDirectory(replicaFolderPath, normalizedSourcePath);

            // 4. Validate log folder is not inside synchronized folders
            FolderRelationshipValidator.ValidateLogFolderRelationship(
                normalizedLogPath,
                normalizedSourcePath,
                replicaFolderPath);

            // 5. Validate synchronization interval (in seconds) and assign it to syncIntervalSeconds
            int synchronizationIntervalSeconds = SynchronizationIntervalValidator.ValidateSynchronizationInterval(synchronizationIntervalInput);

            FolderSynchronization.Services.FolderSynchronizer.RunPeriodicSynchronization(normalizedSourcePath, replicaFolderPath, synchronizationIntervalSeconds);
        }
        catch (ValidationException ex)
        {
            if (Log.Logger != null)
                Log.Error(ex, "Application stopped due to validation error");
            else
                Console.WriteLine($"Validation failed: {ex.Message}");
            return;
        }
        catch (Exception ex)
        {
            if (Log.Logger != null)
                Log.Error(ex, "Application stopped due to unexpected error");
            else
                Console.WriteLine($"Unexpected error: {ex.Message}");
            return;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}