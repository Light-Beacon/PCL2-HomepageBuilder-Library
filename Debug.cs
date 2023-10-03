using System;
using System.Diagnostics;
using System.IO;
using static System.Diagnostics.Debug;

namespace PageBuilder
{
    enum Logtype
    {
        UI = 1,
        Process = 2,
        Thread = 3,
        ThreadCreated = 301,
        ThreadDeleted = 302,
        IO = 4,
        Read = 401,
        Write = 402,
        Success = 501,
        Warn = 502,
        Fail = 503,
        Error = 504,

    }
    public static partial class Debug
    {
        static string LogPath = System.AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "Log.txt";
        //static StreamWriter streamWriter;
        static Stopwatch sw = new Stopwatch();
        static bool enabled = false;
        static short Level = 2;
        static string RunTime => sw.ElapsedMilliseconds.ToString().PadLeft(5, '0');
        static Debug()
        {
            File.Delete(LogPath);
        }
        public static void StartLog(short level)
        {
            Level = level;
            sw = new Stopwatch();
            enabled = true;
            sw.Start();
            //streamWriter = new StreamWriter(LogPath);
            Log("[Debug] 启动日志记录", 15);
        }
        public static void StopLog()
        {
            enabled = false;
            sw.Stop();
            //streamWriter.Close();
        }
        static string Now => DateTime.Now.ToString("hh:mm:ss:ff");

#if Client
        public static void Log(string log, short logLevel, int code = 0, string detil = null)
        {
            string logMessage = string.Empty;
            if (logLevel >= Level || code != 0)
            {
                logMessage = $"[Info|{logLevel:X}] {Now}|{RunTime} {log}";
                if (code != 0)
                {
                    logMessage = $"[Warn|{logLevel:X}] {Now}|{RunTime} [{code:X}] {log}";
                }
                Log(logMessage);
            }

        }

        public static void LogError(System.Exception ex, string message = null, short logLevel = 8)
        {

            if (logLevel >= Level && enabled)
            {
                string logMessage = $"[Error|{logLevel:X}] {Now}|{RunTime} [{ex:X}] {message} {ex.Message}";
                Log(logMessage);
                Fail(logMessage);
            }
        }

        public static void LogWarn(System.Exception ex, string message = null, short logLevel = 8)
        {

            if (logLevel >= Level && enabled)
            {
                string logMessage = $"[Warn|{logLevel:X}] {Now}|{RunTime} [{ex:X}] {message} {ex.Message}";
                Log(logMessage);
            }
        }

        public delegate void LogHandler(string log);
        public static event LogHandler OnLog;

        private static void Log(string message)
        {
            WriteLine(message);
            //streamWriter.WriteLine(message);
            if (OnLog != null)
                OnLog(message);
        }
#elif Runner
        public static void LogError(System.Exception ex, string message = null, short logLevel = 8)
        {
            Console.Error.WriteLine(message ?? ex.ToString());
        }
        public static void Log(string log, short logLevel, int code = 0, string detil = null)
        {
            string logMessage = string.Empty;
            if (logLevel >= Level || code != 0)
            {
                logMessage = $"{log}";
                if (code != 0)
                {
                    logMessage = $"[CODE:{code:X}]{log}";
                }
                Console.WriteLine(logMessage);
            }
        }
        public static void Log(Exception ex, string message = null, short logLevel = 8)
        {

            if (logLevel >= Level && enabled)
            {
                string logMessage = $"[CODE:{ex:X}] {message} {ex.Message}";
                Console.WriteLine(logMessage);
            }
        }
#else
        public static void LogError(System.Exception ex, string message = null, short logLevel = 8)
        {
            if (logLevel >= Level && enabled)
            {
                string logMessage = $"[Error|{logLevel:X}] {Now}|{RunTime} [{ex:X}] {message} {ex.Message}";
                ServerLog(logMessage, NHPS2.LogType.Warning);
                Fail(logMessage);
            }
        }

        public static void Log(string log, short logLevel, int code = 0, string detil = null)
        {
            string logMessage = string.Empty;
            if (logLevel >= Level || code != 0)
            {
                logMessage = $"{log}";
                var type = NHPS2.LogType.Info;
                if (code != 0)
                {
                    logMessage = $"[CODE:{code:X}]{log}";
                    type = NHPS2.LogType.Warning;
                }
                ServerLog(logMessage, type);
            }
        }

        public static void Log(Exception ex, string message = null, short logLevel = 8)
        {

            if (logLevel >= Level && enabled)
            {
                string logMessage = $"[CODE:{ex:X}] {message} {ex.Message}";
                ServerLog(logMessage, NHPS2.LogType.Error);
            }
        }

        private static void ServerLog(string message, NHPS2.LogType type)
        {
            NHPS2.Debug.Log(message, type);
        }
#endif
    }
}