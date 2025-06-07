using FolderSynchronization.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace FolderSynchronization.Validators
{
    public static class SynchronizationIntervalValidator
    {
        public static bool ValidateSynchronizationInterval(string intervalInput, out int synchronizationIntervalInSeconds)
        {
            if (!int.TryParse(intervalInput, out synchronizationIntervalInSeconds) || synchronizationIntervalInSeconds <= 0)
            {
                Log.Error("Synchronization interval must be a positive integer (seconds). Provided value: {intervalInput}", intervalInput);
                return false;
            }
            return true;
        }
    }
}
