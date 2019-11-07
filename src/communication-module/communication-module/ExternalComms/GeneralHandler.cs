using System;
using System.Threading;
using Core.Exceptions;
using Newtonsoft.Json;

namespace Core.ExternalComms
{
    internal class GeneralHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal static T PollVariableFor10Seconds<T>(ref T obj)
        {
            try
            {
                Logger.Info("Start of polling, polling for object type: " + typeof(T));
                var counter = 0;
                while (null == obj && counter < 100)
                {
                    Thread.Sleep(100);
                    counter++;
                }

                if (null == obj)
                {
                    const string message = "Polling finished after 10 seconds without result";
                    Logger.Info(message);
                    throw new PollingException(message, typeof(T).Name);
                }

                Logger.Info("Polling finished before 10 seconds ran out, returning " + typeof(T).Name);
                return obj;
            }
            catch (Exception e)
            {
                Logger.Debug("Got exception in PollVariableFor10Seconds");
                Logger.Debug(e);
                throw new Exception("Exception my man");
            }
        }

        internal static string ReturnAsJSON<T>(T obj)
        {
            Logger.Info("Returning " + obj.GetType().Name + " as JSON");
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }
}
