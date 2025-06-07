using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderSynchronization.Configuration
{
    public static class LoggerConfiguration
    {
        public static void ConfigureSerilog(string logDirectory)
        {
            string logFile = Path.Combine(logDirectory, "log_.txt");

            Log.Logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFile, 
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: null,
                    fileSizeLimitBytes: 1024 * 1024 * 100) // Limit each file to 100MB
                .CreateLogger();

            Log.Information("Log started at {DateTime}", DateTime.Now);
        }
    }
}
