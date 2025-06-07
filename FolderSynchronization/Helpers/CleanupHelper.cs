using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using static FolderSynchronization.Helpers.DeleteHelper;



namespace FolderSynchronization.Helpers
{
    static public class CleanupHelper
    {
        public static void PromptAndDeleteIncompleteCopy(string folderPath)
        {
            Log.Information("Delete incomplete copy? (y/n)");
            string? answer = Console.ReadLine();

            Log.Information("User response to deletion prompt: '{Answer}'", answer);
            
            if (!string.IsNullOrEmpty(answer) && answer.Trim().ToLower() == "y")
            {
                try
                {
                    if (Directory.Exists(folderPath))
                    {
                        DeleteDirectorySafe(folderPath);
                        Log.Information("Incomplete copy deleted: {folderPath}", folderPath);
                    }
                    else 
                    {
                        Log.Warning("Directory not found: {folderPath}", folderPath);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to delete incomplete copy: {folderPath}", folderPath);
                }
            }
            else
            {
                Log.Information("User chose not to delete incomplete copyt: {folderPath}", folderPath);
            }
        }
    }
}
