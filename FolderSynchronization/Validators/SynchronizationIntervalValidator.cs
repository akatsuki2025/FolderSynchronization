using FolderSynchronization.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Validators
{
    public static class SynchronizationIntervalValidator
    {
        public static bool ValidateSynchronizationInterval(string intervalInput, out int synchronizationIntervalInSeconds)
        {
            if (!int.TryParse(intervalInput, out synchronizationIntervalInSeconds) || synchronizationIntervalInSeconds <= 0)
            {
                LoggerHelper.Log("Error: Synchronization interval must be a non-negative integer (seconds).");
                return false;
            }
            return true;
        }
    }
}
