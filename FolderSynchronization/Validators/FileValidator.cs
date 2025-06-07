using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    Console.WriteLine($"Error: Cannot write to existing log file: {ex.Message}");
                    return false;
                }
            }
            return true;
        }
    }
}
