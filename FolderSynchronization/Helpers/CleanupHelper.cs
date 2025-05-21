using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Helpers
{
    static public class CleanupHelper
    {
        public static void PromptAndDeleteIncompleteCopy(string folderPath)
        {
            Console.WriteLine("Delete incomplete copy? (y/n)");
            string answer = Console.ReadLine();
            if (!string.IsNullOrEmpty(answer) && answer.Trim().ToLower() == "y")
            {
                try
                {
                    if (Directory.Exists(folderPath))
                    {
                        DeleteDirectorySafe(folderPath);
                        Console.WriteLine("Incomplete copy deleted.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete incomplete copy: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Incomplete copy retained");
            }
        }

        private static void DeleteDirectorySafe(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return;

            // Remove read-only attributes from all files
            var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            // Delete directory recursively
            Directory.Delete(folderPath, recursive: true);
        }
    }
}
