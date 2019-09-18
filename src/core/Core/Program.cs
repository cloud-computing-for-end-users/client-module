using System;
using System.IO;
using System.Runtime.CompilerServices;
using NLog;

namespace Core
{
    class Program
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            SetupNLog();
            logger.Info("Starting program");
            new CommsWrapper();
        }

        static void SetupNLog()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logFile = "log.txt";
            var rootFolder = System.AppDomain.CurrentDomain.BaseDirectory;

            if (File.Exists(Path.Combine(rootFolder, logFile)))
            {  
                File.Delete(Path.Combine(rootFolder, logFile));
            }
        
            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = logFile };

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;
        }
    }
}