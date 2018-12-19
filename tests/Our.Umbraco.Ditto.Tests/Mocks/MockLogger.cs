using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Logging;

namespace Our.Umbraco.Ditto.Tests.Mocks
{
    internal class MockLogger : ILogger
    {
        private readonly List<LogMessage> _logMessages;

        public MockLogger()
        {
            _logMessages = new List<LogMessage>();
        }

        public IEnumerable<LogMessage> GetLogMessages()
        {
            return _logMessages;
        }

        public void ClearLogMessages()
        {
            _logMessages.Clear();
        }

        public bool IsEnabled(Type reporting, LogLevel level)
        {
            return true;
        }

        public void Fatal(Type reporting, Exception exception, string message)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Fatal, message, exception));
        }

        public void Fatal(Type reporting, Exception exception)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Fatal, exception));
        }

        public void Fatal(Type reporting, string message)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Fatal, message));
        }

        public void Fatal(Type reporting, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Fatal, messageTemplate, propertyValues, exception));
        }

        public void Fatal(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Fatal, messageTemplate, propertyValues));
        }

        public void Error(Type reporting, Exception exception, string message)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Error, message, exception));
        }

        public void Error(Type reporting, Exception exception)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Error, exception));
        }

        public void Error(Type reporting, string message)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Error, message));
        }

        public void Error(Type reporting, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Error, messageTemplate, propertyValues, exception));
        }

        public void Error(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Error, messageTemplate, propertyValues));
        }

        public void Warn(Type reporting, string message)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Warn, message));
        }

        public void Warn(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Warn, messageTemplate, propertyValues));
        }

        public void Warn(Type reporting, Exception exception, string message)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Warn, message));
        }

        public void Warn(Type reporting, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Warn, messageTemplate, propertyValues, exception));
        }

        public void Info(Type reporting, string message)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Info, message));
        }

        public void Info(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Info, messageTemplate, propertyValues));
        }

        public void Debug(Type reporting, string message)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Debug, message));
        }

        public void Debug(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Debug, messageTemplate, propertyValues));
        }

        public void Verbose(Type reporting, string message)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Verbose, message));
        }

        public void Verbose(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            _logMessages.Add(new LogMessage(reporting, LogMessageType.Verbose, messageTemplate, propertyValues));
        }
    }

    internal class LogMessage
    {
        public LogMessage(Type reporting, LogMessageType messageType, Exception exception = null)
        {
            CallingType = reporting;
            MessageType = messageType;
            Exception = exception;
        }

        public LogMessage(Type reporting, LogMessageType messageType, string message, Exception exception = null)
        {
            CallingType = reporting;
            Message = message;
            MessageType = messageType;
            Exception = exception;
        }

        public LogMessage(Type reporting, LogMessageType messageType, string messageFormat, object[] messageParams, Exception exception = null)
        {
            CallingType = reporting;
            Message = string.Format(messageFormat, messageParams);
            MessageType = messageType;
            Exception = exception;
        }

        public Type CallingType { get; }

        public LogMessageType MessageType { get; }

        public string Message { get; }

        public Exception Exception { get; }
    }

    internal enum LogMessageType
    {
        Fatal,
        Error,
        Warn,
        Info,
        Debug,
        Verbose
    }
}