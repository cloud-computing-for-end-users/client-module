using System;
using NLog;

namespace Core
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            try
            {   
                SetupNLog();
                Logger.Info("Starting program");
                CommsWrapper.Setup(localhost:false);
            }
            catch (Exception e)
            {
                Logger.Info("Exception from main follows:");
                Logger.Debug(e);
            }
        }

        private static void SetupNLog()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logFile = "client-module-log.txt";

            /*
            var rootFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists(Path.Combine(rootFolder, logFile)))
            {  
                File.Delete(Path.Combine(rootFolder, logFile));
            }
            */

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = logFile };

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;
        }
    }
}