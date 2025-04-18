using System;
using System.IO;
using System.Text;

namespace ReadingLargeTextFiles.Core
{
    /// <summary>
    /// 日志管理类，用于统一管理应用程序的日志输出
    /// </summary>
    public static class LogManager
    {
        // 日志级别
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        // 当前日志级别
        private static LogLevel currentLogLevel = LogLevel.Error;

        // 日志文件路径
        private static string logFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "logs",
            $"log_{DateTime.Now:yyyyMMdd}.txt");

        // 是否启用文件日志
        private static bool enableFileLog = true;

        // 是否启用控制台日志
        private static bool enableConsoleLog = true;

        // 日志事件
        public static event EventHandler<LogEventArgs> LogAdded;

        /// <summary>
        /// 日志事件参数
        /// </summary>
        public class LogEventArgs : EventArgs
        {
            public string Message { get; }
            public LogLevel Level { get; }
            public DateTime Timestamp { get; }

            public LogEventArgs(string message, LogLevel level)
            {
                Message = message;
                Level = level;
                Timestamp = DateTime.Now;
            }
        }

        /// <summary>
        /// 初始化日志管理器
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="enableFile">是否启用文件日志</param>
        /// <param name="enableConsole">是否启用控制台日志</param>
        public static void Initialize(LogLevel level = LogLevel.Info, bool enableFile = false, bool enableConsole = true)
        {
            currentLogLevel = level;
            enableFileLog = enableFile;
            enableConsoleLog = enableConsole;

            if (enableFileLog)
            {
                // 确保日志目录存在
                string logDir = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                // 写入日志头
                string header = $"=== 日志开始 {DateTime.Now} ===\r\n";
                File.AppendAllText(logFilePath, header, Encoding.UTF8);
            }
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void Debug(string message)
        {
            if (currentLogLevel <= LogLevel.Debug)
            {
                Log(message, LogLevel.Debug);
            }
        }

        /// <summary>
        /// 记录一般信息
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void Info(string message)
        {
            if (currentLogLevel <= LogLevel.Info)
            {
                Log(message, LogLevel.Info);
            }
        }

        /// <summary>
        /// 记录警告信息
        /// </summary>
        /// <param name="message">日志消息</param>
        public static void Warning(string message)
        {
            if (currentLogLevel <= LogLevel.Warning)
            {
                Log(message, LogLevel.Warning);
            }
        }

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="ex">异常对象</param>
        public static void Error(string message, Exception ex = null)
        {
            if (currentLogLevel <= LogLevel.Error)
            {
                string logMessage = message;
                if (ex != null)
                {
                    logMessage += $"\r\n异常: {ex.Message}\r\n堆栈: {ex.StackTrace}";
                }
                Log(logMessage, LogLevel.Error);
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="level">日志级别</param>
        private static void Log(string message, LogLevel level)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logMessage = $"[{timestamp}] [{level}] {message}";

            // 输出到控制台
            if (enableConsoleLog)
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                switch (level)
                {
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case LogLevel.Info:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }

                Console.WriteLine(logMessage);
                Console.ForegroundColor = originalColor;
            }

            // 写入日志文件
            if (enableFileLog)
            {
                try
                {
                    File.AppendAllText(logFilePath, logMessage + "\r\n", Encoding.UTF8);
                }
                catch
                {
                    // 忽略写入日志文件时的错误
                }
            }

            // 触发日志事件
            LogAdded?.Invoke(null, new LogEventArgs(message, level));
        }
    }
}
