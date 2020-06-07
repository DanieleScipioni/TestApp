using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace TestAppUWP.Logic.Logs
{
    internal class Writer
    {
        private const string FileNamePrefix = "SpeedyTouch.FullTrustProcess";
        private static string FileName => $"{FileNamePrefix}.{DateTime.Today:yyyyMMdd}.log";

        private static readonly ConcurrentDictionary<string, StreamWriter> StreamWritersByFileName =
            new ConcurrentDictionary<string, StreamWriter>();
        private readonly string _logDirPath;

        readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private DateTime _nextClean;

        internal Writer(string logDirPath)
        {
            _logDirPath = logDirPath;
            _nextClean = DateTime.Now.AddMinutes(1);
        }

        public async void Log(string loggerName, string message)
        {
            await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                CleanOldFiles();
                await GetStreamWriter()
                    .WriteLineAsync($"{DateTimeOffset.UtcNow.ToRoundTripString()}|{loggerName}|{message}")
                    .ConfigureAwait(false);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async void Flush()
        {
            await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                await GetStreamWriter().FlushAsync().ConfigureAwait(false);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private void CleanOldFiles()
        {
            DateTime dateTime = DateTime.Now;
            if (dateTime < _nextClean) return;

            try
            {
                DateTime threshold = DateTime.Today.AddDays(-1);
                var directoryInfo = new DirectoryInfo(_logDirPath);
                foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles($"{FileNamePrefix}*"))
                {
                    if (fileInfo.LastWriteTime.Date <= threshold) fileInfo.Delete();
                }
            }
            finally
            {
                _nextClean = DateTime.Today.AddDays(1);
            }
        }
        private StreamWriter GetStreamWriter()
        {
            return StreamWritersByFileName.GetOrAdd(FileName, fileName => new StreamWriter(File.Open(
                Path.Combine(_logDirPath, $"SpeedyTouch.FullTrustProcess.{DateTime.Today:yyyyMMdd}.log"),
                FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8, 1024));
        }
    }
}