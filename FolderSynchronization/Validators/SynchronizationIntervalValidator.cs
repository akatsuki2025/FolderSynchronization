using FolderSynchronization.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using FolderSynchronization.Exceptions;

namespace FolderSynchronization.Validators
{
    public static class SynchronizationIntervalValidator
    {
        public static int ValidateSynchronizationInterval(string intervalInput)
        {
            if (!int.TryParse(intervalInput, out int synchronizationIntervalInSeconds))
                throw new ValidationException($"Synchronization interval must be a valid integer. Provided value: {intervalInput}");

            if (synchronizationIntervalInSeconds <= 0)
                throw new ValidationException($"Synchronization interval must be a positive integer (seconds). Provided value: {{intervalInput}}");

            return synchronizationIntervalInSeconds;
        }
    }
}
