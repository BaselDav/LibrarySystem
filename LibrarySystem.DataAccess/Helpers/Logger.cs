using System;
using System.IO;


namespace LibrarySystem.DataAccess.Helpers
{
    public static class Logger
    {
        private static readonly string logFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LibrarySystem", "error-log.txt"
        );

        public static void Log(Exception ex)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

                using (var writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("=== ERROR ===");
                    writer.WriteLine($"Date: {DateTime.Now}");
                    writer.WriteLine($"Message: {ex.Message}");
                    writer.WriteLine($"StackTrace: {ex.StackTrace}");
                    writer.WriteLine();
                }
            }
            catch
            {
                
            }
        }
    }
}
