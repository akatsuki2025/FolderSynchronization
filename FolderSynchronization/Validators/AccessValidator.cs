using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSynchronization.Exceptions;

namespace FolderSynchronization.Validators
{
    public static class AccessValidator
    {
        public static void ValidateReadAccess(string path)
        {
            try
            {
                Directory.GetFiles(path);
            }
            catch (Exception ex) 
            {
                throw new ValidationException($"No read access: {ex.Message}");
            }
        }

        public static void ValidateWriteAccess(string path)
        {
            try
            {
                string testFile = Path.Combine(path, "write_test.tmp");
                File.WriteAllText(testFile, string.Empty);
                File.Delete(testFile);
            }
            catch (Exception ex)
            {
                throw new ValidationException($"No write acces: {ex.Message}");
            }
        }
    }
}
