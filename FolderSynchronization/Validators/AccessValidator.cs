using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSynchronization.Exceptions;
using System.IO.Abstractions;

namespace FolderSynchronization.Validators
{
    public static class AccessValidator
    {
        private static readonly IFileSystem _defaultFileSystem;
        private static IFileSystem _currentFileSystem;

        static AccessValidator()
        {
            _defaultFileSystem = new FileSystem();
            _currentFileSystem = _defaultFileSystem;
        }

        private static IFileSystem FileSystem
        {
            get => _currentFileSystem ?? _defaultFileSystem;
        }

        public static void SetFileSystem(IFileSystem fileSystem)
        {
            _currentFileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public static void ResetFileSystem()
        {
            _currentFileSystem = _defaultFileSystem;
        }

        public static void ValidateReadAccess(string path)
        {
            try
            {
                FileSystem.Directory.GetFiles(path);
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
                FileSystem.File.WriteAllText(testFile, string.Empty);
                FileSystem.File.Delete(testFile);
            }
            catch (Exception ex)
            {
                throw new ValidationException($"No write access: {ex.Message}");
            }
        }
    }
}
