using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolderSynchronization.Exceptions;

namespace FolderSynchronization.Validators
{
    public static class FolderRelationshipValidator
    {
        public static void ValidateSourceAndDestinationRelationship(string sourcePath, string destinationPath)
        {
            string? destinationParentPath = Path.GetDirectoryName(destinationPath);

            if (destinationParentPath is not null)
                ValidateNotSameFolder(sourcePath, destinationParentPath, "Source", "Destination");

            ValidateNotNestedInside(sourcePath, destinationPath, "Source", "Destination");
            ValidateNotNestedInside(destinationPath, sourcePath, "Destination", "Source");
        }

        public static void ValidateLogFolderRelationship(string logPath, string sourcePath, string destinationPath)
        {
            ValidateNotNestedInside(logPath, sourcePath, "Log", "Source");
            ValidateNotNestedInside(logPath, destinationPath, "Log", "Destination");
        }

        private static void ValidateNotSameFolder(string path1, string path2, string folder1Name, string folder2Name)
        {
            string fullPath1 = Path.GetFullPath(path1);
            string fullPath2 = Path.GetFullPath(path2);

            if (fullPath1.Equals(fullPath2, StringComparison.OrdinalIgnoreCase))
                throw new ValidationException($"{folder1Name} and {folder2Name} folders cannot be the same");
        }

        private static void ValidateNotNestedInside(string pathToCheck, string containerPath, string folderToCheckName, string containerName)
        {
            var pathToCheckFull = Path.GetFullPath(pathToCheck) + Path.DirectorySeparatorChar;
            var containerPathFull = Path.GetFullPath(containerPath) + Path.DirectorySeparatorChar;

            if (pathToCheckFull.StartsWith(containerPathFull, StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException($"{folderToCheckName} folder cannot be inside {containerName} folder");
            }
        }


    }
}
