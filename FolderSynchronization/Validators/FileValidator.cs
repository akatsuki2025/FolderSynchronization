using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace FolderSynchronization.Validators
{
    public static class FileValidator
    {
        public static bool VerifyFileWriteAccess(string fullLogFilePath)
        {
            if (File.Exists(fullLogFilePath))
            {
                try
                {
                    // Try to open the file for append to validate access
                    using (FileStream fs = new FileStream(fullLogFilePath, FileMode.Append))
                    {
                        // Testing permissions
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Cannot write to existing log file {fullLogFilePath}", fullLogFilePath);
                    return false;
                }
            }
            return true;
        }
    }
}
