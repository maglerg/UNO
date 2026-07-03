using System;
using System.IO;

namespace ClientUNO
{
    public static class Logger
    {
        private static readonly string path = "log.txt";

        public static void Log(string message)
        {
            File.AppendAllText(path,
                $"[{DateTime.Now:HH:mm:ss}] {message}\n");
        }

        public static void Error(string message)
        {
            File.AppendAllText(path,
                $"[{DateTime.Now:HH:mm:ss}] ERROR: {message}\n");
        }
    }
}
