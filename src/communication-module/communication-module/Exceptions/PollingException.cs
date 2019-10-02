using System;

namespace Core.Exceptions
{
    class PollingException : Exception
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public string VariablePolled { get; set; }

        public PollingException(string variablePolled)
        {
            VariablePolled = variablePolled;
        }

        public PollingException(string message, string variablePolled) : base(message)
        {
            VariablePolled = variablePolled;
        }

        public PollingException(string message, string variablePolled, Exception inner) : base(message, inner)
        {
            VariablePolled = variablePolled;
        }

        public string Handle()
        {
            Logger.Debug(this);
            return "(PollingException) Failed to get " + VariablePolled;
        }
    }
}
