using System;
using System.IO;

namespace ClientUNO
{
    public static class Logger
    {
        private static string _fileName;

        public static void Initialize(string clientName)
        {
            Directory.CreateDirectory("Logs");

            _fileName = Path.Combine("Logs", $"log_{clientName}.txt");

            File.WriteAllText(
                _fileName,
                $"=== Session started {DateTime.Now:dd.MM.yyyy HH:mm:ss} ==={Environment.NewLine}");
        }

        public static void Log(string text)
        {
            Write("INFO", text);
        }

        public static void Error(string text)
        {
            Write("ERROR", text);
        }

        public static void Error(Exception ex)
        {
            Write("ERROR", ex.ToString());
        }

        private static void Write(string type, string text)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                Directory.CreateDirectory("Logs");
                _fileName = Path.Combine("Logs", "log.txt");
            }

            File.AppendAllText(
                _fileName,
                $"[{DateTime.Now:HH:mm:ss}] {type}: {text}{Environment.NewLine}");
        }
    }
}
