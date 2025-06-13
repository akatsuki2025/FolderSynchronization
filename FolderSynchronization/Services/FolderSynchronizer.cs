using Serilog;
using FolderSynchronization.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Services
{
    public static class FolderSynchronizer
    {
        private const int MILLISECONDS_PER_SECOND = 1000;

        public static void RunPeriodicSynchronization(string sourcePath, string replicaPath, int intervalInSeconds)
        {   while (true)
            {
                try
                {
                    Log.Information("Synchronizing folders at {DateTime}", DateTime.Now);

                    SynchronizationHelper.SynchronizeFolder(sourcePath, replicaPath);

                    Log.Information("Synchronization complete.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error during synchronization");
                    CleanupHelper.PromptAndDeleteIncompleteCopy(replicaPath);
                    return;
                }

                Log.Information("Next check scheduled in {synchronizationIntervalSeconds} seconds", intervalInSeconds);
                Log.Information("Listening for changes... Press Ctrl+C to exit");

                Thread.Sleep(intervalInSeconds * MILLISECONDS_PER_SECOND);
            }
        }
    }
}
