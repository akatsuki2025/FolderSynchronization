using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FolderSynchronization.Helpers.DeleteHelper;



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
                        LoggerHelper.Log("Incomplete copy deleted.");
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Log($"Failed to delete incomplete copy: {ex.Message}");
                }
            }
            else
            {
                LoggerHelper.Log("Incomplete copy retained");
            }
        }
    }
}
