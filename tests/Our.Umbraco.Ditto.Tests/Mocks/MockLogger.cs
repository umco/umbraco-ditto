using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Ditto.Tests.Mocks
{
    internal class MockLogger : ILogger
    {
        private readonly List<LogMessage> logMessages;

        public MockLogger()
        {
            this.logMessages = new List<LogMessage>();
        }

        public void Error(Type callingType, string message, Exception exception)
        {
            logMessages.Add(new LogMessage { MessageType = LogMessageType.Error, CallingType = callingType, Message = message, Exception = exception });
        }

        public void Warn(Type callingType, string message, params Func<object>[] formatItems)
        {
            logMessages.Add(new LogMessage { MessageType = LogMessageType.Warn, CallingType = callingType, Message = string.Format(message, formatItems.Select(x => x.Invoke()).ToArray()) });
        }

        public void WarnWithException(Type callingType, string message, Exception e, params Func<object>[] formatItems)
        {
            logMessages.Add(new LogMessage { MessageType = LogMessageType.Warn, CallingType = callingType, Message = string.Format(message, formatItems.Select(x => x.Invoke()).ToArray()), Exception = e });
        }

        public void Info(Type callingType, Func<string> generateMessage)
        {
            logMessages.Add(new LogMessage { MessageType = LogMessageType.Info, CallingType = callingType, Message = generateMessage.Invoke() });
        }

        public void Info(Type callingType, string generateMessageFormat, params Func<object>[] formatItems)
        {
            logMessages.Add(new LogMessage { MessageType = LogMessageType.Info, CallingType = callingType, Message = string.Format(generateMessageFormat, formatItems.Select(x => x.Invoke()).ToArray()) });
        }

        public void Debug(Type callingType, Func<string> generateMessage)
        {
            logMessages.Add(new LogMessage { MessageType = LogMessageType.Debug, CallingType = callingType, Message = generateMessage.Invoke() });
        }

        public void Debug(Type callingType, string generateMessageFormat, params Func<object>[] formatItems)
        {
            logMessages.Add(new LogMessage { MessageType = LogMessageType.Info, CallingType = callingType, Message = string.Format(generateMessageFormat, formatItems.Select(x => x.Invoke()).ToArray()) });
        }

        public IEnumerable<LogMessage> GetLogMessages()
        {
            return this.logMessages;
        }

        public void ClearLogMessages()
        {
            this.logMessages.Clear();
        }
    }

    internal class LogMessage
    {
        public Type CallingType { get; set; }

        public LogMessageType MessageType { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }

    internal enum LogMessageType
    {
        Error,
        Warn,
        Info,
        Debug
    }
}