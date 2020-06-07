using System;
using System.Collections.Concurrent;
using System.IO;

namespace TestAppUWP.Logic.Logs
{
    public static class LogFactory
    {
        private static readonly ConcurrentDictionary<string, Logger> LoggersByName =
            new ConcurrentDictionary<string, Logger>();
        private static Writer _writer;

        public static void Init(string logDirPath)
        {
            Directory.CreateDirectory(logDirPath);
            _writer = new Writer(logDirPath);
        }

        public static Logger GetLogger(string name)
        {
            return LoggersByName.GetOrAdd(name, requestedName => new Logger(_writer, requestedName));
        }
    }

    public class Logger
    {
        private readonly Writer _writer;
        private readonly string _loggerName;
        
        internal Logger(Writer writer, string loggerName)
        {
            _writer = writer;
            _loggerName = loggerName;
        }

        public void Log(string message)
        {
            _writer.Log(_loggerName, message);
        }

        public void Flush()
        {
            _writer.Flush();
        }
    }

    internal static class DateTimeExtension
    {
        public static string ToRoundTripString(this DateTimeOffset dateTime)
        {
            return dateTime.ToString("o");
        }
    }
}