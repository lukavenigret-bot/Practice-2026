using System;
using System.IO;

namespace FileSystemUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            // Определение пути (из аргументов или текущая директория)
            string path = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

            Console.WriteLine($"📁 СОДЕРЖИМОЕ ПАПКИ: {path}");
            Console.WriteLine(new string('─', 60));

            try
            {
                // 1. Вывод директорий
                Console.WriteLine("\n📂 ПАПКИ:");
                var dirs = Directory.GetDirectories(path);
                if (dirs.Length == 0)
                    Console.WriteLine("  (пусто)");
                else
                {
                    foreach (string dir in dirs)
                    {
                        var info = new DirectoryInfo(dir);
                        string size = GetDirectorySize(dir);
                        Console.WriteLine($"  📁 {info.Name,-30} {size,15}");
                    }
                }

                // 2. Вывод файлов
                Console.WriteLine("\n📄 ФАЙЛЫ:");
                var files = Directory.GetFiles(path);
                if (files.Length == 0)
                    Console.WriteLine("  (пусто)");
                else
                {
                    foreach (string file in files)
                    {
                        var info = new FileInfo(file);
                        string size = FormatFileSize(info.Length);
                        string modified = info.LastWriteTime.ToString("dd.MM.yyyy HH:mm");
                        Console.WriteLine($"  📄 {info.Name,-30} {size,12}  {modified,16}");
                    }
                }

                // 3. Статистика
                Console.WriteLine("\n" + new string('─', 60));
                Console.WriteLine($"📊 ИТОГО: {dirs.Length} папок, {files.Length} файлов");
                Console.WriteLine($"📊 Общий размер: {FormatFileSize(GetTotalSize(path))}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Получение размера папки
        /// </summary>
        static string GetDirectorySize(string path)
        {
            try
            {
                long size = 0;
                foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    size += new FileInfo(file).Length;
                }
                return FormatFileSize(size);
            }
            catch
            {
                return " (недоступно)";
            }
        }

        /// <summary>
        /// Получение общего размера
        /// </summary>
        static long GetTotalSize(string path)
        {
            long size = 0;
            foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                size += new FileInfo(file).Length;
            }
            return size;
        }

        /// <summary>
        /// Форматирование размера файла
        /// </summary>
        static string FormatFileSize(long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} Б";
            else if (bytes < 1024 * 1024)
                return $"{bytes / 1024.0:F1} КБ";
            else if (bytes < 1024 * 1024 * 1024)
                return $"{bytes / (1024.0 * 1024.0):F1} МБ";
            else
                return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} ГБ";
        }
    }
}