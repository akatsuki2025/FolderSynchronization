using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSynchronization.Exceptions;
using System.IO.Abstractions;

namespace FolderSynchronization.Validators
{
    public class AccessValidator : IAccessValidator
    {
        private readonly IFileSystem _fileSystem;

        public AccessValidator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public void ValidateReadAccess(string path)
        {
            try
            {
                _fileSystem.Directory.GetFiles(path);
            }
            catch (Exception ex) 
            {
                throw new ValidationException($"No read access: {ex.Message}");
            }
        }

        public void ValidateWriteAccess(string path)
        {
            try
            {
                string testFile = _fileSystem.Path.Combine(path, "write_test.tmp");
                _fileSystem.File.WriteAllText(testFile, string.Empty);
                _fileSystem.File.Delete(testFile);
            }
            catch (Exception ex)
            {
                throw new ValidationException($"No write access: {ex.Message}");
            }
        }
    }
}
