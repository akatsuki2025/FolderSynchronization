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
        public static void ConfigureSerilog(string logPath)
        {
            Log.Logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Log started at {DateTime}", DateTime.Now);
        }
    }
}
