using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleTaskManager
{
    /// <summary>
    /// Модель задачи
    /// </summary>
    class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public override string ToString()
        {
            string status = IsCompleted ? "✅" : "⬜";
            return $"{status} #{Id}: {Title}";
        }

        public string GetFullInfo()
        {
            string status = IsCompleted ? "Выполнена" : "В работе";
            string completed = IsCompleted ? $"Завершена: {CompletedAt:dd.MM.yyyy HH:mm}" : "Не завершена";
            return $"#{Id} | {Title}\n" +
                   $"   Описание: {Description}\n" +
                   $"   Статус: {status}\n" +
                   $"   Создана: {CreatedAt:dd.MM.yyyy HH:mm}\n" +
                   $"   {completed}";
        }
    }

    class Program
    {
        // Список задач
        static List<TaskItem> tasks = new List<TaskItem>();
        static int nextId = 1;
        static string dataFile = "tasks.txt";

        // =========================================================
        // ГЛАВНЫЙ МЕТОД (ТОЧКА ВХОДА)
        // =========================================================

        static void Main()
        {
            // Загрузка задач из файла
            LoadTasks();

            Console.WriteLine("╔═══════════════════════════════════════════════════╗");
            Console.WriteLine("║             📋 МЕНЕДЖЕР ЗАДАЧ                    ║");
            Console.WriteLine("║                                                   ║");
            Console.WriteLine("║  Версия 1.0     |     Всего задач: {0,-3}        ║", tasks.Count);
            Console.WriteLine("╚═══════════════════════════════════════════════════╝");

            // Главный цикл
            while (true)
            {
                Console.Clear();
                ShowMenu();

                Console.Write("\nВыберите действие: ");
                string choice = Console.ReadLine();
                Console.Clear();

                switch (choice)
                {
                    case "1": ShowTasks(); break;
                    case "2": AddTask(); break;
                    case "3": CompleteTask(); break;
                    case "4": DeleteTask(); break;
                    case "5": ShowStats(); break;
                    case "6": ShowTaskDetails(); break;
                    case "7": ClearAllTasks(); break;
                    case "8": SaveAndExit(); return;
                    default:
                        Console.WriteLine("❌ Неверный выбор! Попробуйте снова.");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        // =========================================================
        // 1. МЕНЮ
        // =========================================================

        static void ShowMenu()
        {
            
            Console.WriteLine("ГЛАВНОЕ МЕНЮ                   ");
            Console.WriteLine("1. Показать все задачи                       ");
            Console.WriteLine("2. Добавить задачу                           ");
            Console.WriteLine("3. Выполнить задачу                          ");
            Console.WriteLine("4. Удалить задачу                            ");
            Console.WriteLine("5. Статистика                                ");
            Console.WriteLine("6. Детали задачи                             ");
            Console.WriteLine("7. Очистить все задачи                      ");
            Console.WriteLine("8. Выход и сохранение                       ");
            Console.WriteLine($"Всего задач: {tasks.Count,-30} ");
            Console.WriteLine($"Выполнено: {tasks.Count(t => t.IsCompleted),-30} ");
            Console.WriteLine($"В работе:  {tasks.Count(t => !t.IsCompleted),-30} ");
        }

        // =========================================================
        // 2. ПОКАЗАТЬ ВСЕ ЗАДАЧИ
        // =========================================================

        static void ShowTasks()
        {
            Console.WriteLine("📋 СПИСОК ЗАДАЧ");
            Console.WriteLine(new string('─', 50));

            if (tasks.Count == 0)
            {
                Console.WriteLine("   Задач пока нет!");
                return;
            }

            // Группировка по статусу
            var pending = tasks.Where(t => !t.IsCompleted).ToList();
            var completed = tasks.Where(t => t.IsCompleted).ToList();

            if (pending.Any())
            {
                Console.WriteLine("\n⏳ В РАБОТЕ:");
                foreach (var task in pending)
                {
                    Console.WriteLine($"   {task}");
                }
            }

            if (completed.Any())
            {
                Console.WriteLine("\n✅ ВЫПОЛНЕНО:");
                foreach (var task in completed)
                {
                    Console.WriteLine($"   {task}");
                }
            }

            Console.WriteLine("\n" + new string('─', 50));
            Console.WriteLine($"📊 ИТОГО: {tasks.Count} задач (выполнено: {completed.Count}, в работе: {pending.Count})");
        }

        // =========================================================
        // 3. ДОБАВИТЬ ЗАДАЧУ
        // =========================================================

        static void AddTask()
        {
            Console.WriteLine("➕ ДОБАВЛЕНИЕ ЗАДАЧИ");
            Console.WriteLine(new string('─', 50));

            Console.Write("Название: ");
            string title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("❌ Название не может быть пустым!");
                return;
            }

            Console.Write("Описание (необязательно): ");
            string description = Console.ReadLine();

            var task = new TaskItem
            {
                Id = nextId++,
                Title = title,
                Description = description ?? "",
                IsCompleted = false,
                CreatedAt = DateTime.Now,
                CompletedAt = null
            };

            tasks.Add(task);
            SaveTasks();

            Console.WriteLine($"\n✅ Задача \"{title}\" добавлена! (ID: {task.Id})");
        }

        // =========================================================
        // 4. ВЫПОЛНИТЬ ЗАДАЧУ
        // =========================================================

        static void CompleteTask()
        {
            ShowTasks();

            if (tasks.Count == 0)
                return;

            Console.Write("\nВведите ID выполненной задачи: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ Неверный ID! Введите число.");
                return;
            }

            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                Console.WriteLine($"❌ Задача с ID {id} не найдена!");
                return;
            }

            if (task.IsCompleted)
            {
                Console.WriteLine($"⚠️ Задача \"{task.Title}\" уже выполнена!");
                return;
            }

            task.IsCompleted = true;
            task.CompletedAt = DateTime.Now;
            SaveTasks();

            Console.WriteLine($"✅ Задача \"{task.Title}\" выполнена! ({task.CompletedAt:dd.MM.yyyy HH:mm})");
        }

        // =========================================================
        // 5. УДАЛИТЬ ЗАДАЧУ
        // =========================================================

        static void DeleteTask()
        {
            ShowTasks();

            if (tasks.Count == 0)
                return;

            Console.Write("\nВведите ID задачи для удаления: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ Неверный ID! Введите число.");
                return;
            }

            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                Console.WriteLine($"❌ Задача с ID {id} не найдена!");
                return;
            }

            Console.Write($"Вы уверены, что хотите удалить задачу \"{task.Title}\"? (да/нет): ");
            string confirm = Console.ReadLine();

            if (confirm?.ToLower() == "да" || confirm?.ToLower() == "yes" || confirm?.ToLower() == "д")
            {
                tasks.Remove(task);
                SaveTasks();
                Console.WriteLine($"🗑 Задача \"{task.Title}\" удалена!");
            }
            else
            {
                Console.WriteLine("❌ Удаление отменено.");
            }
        }

        // =========================================================
        // 6. СТАТИСТИКА
        // =========================================================

        static void ShowStats()
        {
            Console.WriteLine("📊 СТАТИСТИКА");
            Console.WriteLine(new string('─', 50));

            int total = tasks.Count;
            int completed = tasks.Count(t => t.IsCompleted);
            int pending = total - completed;

            Console.WriteLine($"\n📌 ОБЩАЯ СТАТИСТИКА:");
            Console.WriteLine($"   Всего задач: {total}");
            Console.WriteLine($"   Выполнено: {completed} ({(total > 0 ? completed * 100 / total : 0)}%)");
            Console.WriteLine($"   В работе: {pending} ({(total > 0 ? pending * 100 / total : 0)}%)");

            if (total > 0)
            {
                // Среднее время выполнения
                var completedTasks = tasks.Where(t => t.IsCompleted && t.CompletedAt.HasValue).ToList();
                if (completedTasks.Any())
                {
                    double avgMinutes = completedTasks.Average(t =>
                        (t.CompletedAt.Value - t.CreatedAt).TotalMinutes);
                    Console.WriteLine($"\n⏱ СРЕДНЕЕ ВРЕМЯ ВЫПОЛНЕНИЯ:");
                    Console.WriteLine($"   {avgMinutes:F1} минут ({avgMinutes / 60:F1} часов)");
                }

                // Самая старая задача
                var oldest = tasks.OrderBy(t => t.CreatedAt).First();
                Console.WriteLine($"\n📅 САМАЯ СТАРАЯ ЗАДАЧА:");
                Console.WriteLine($"   \"{oldest.Title}\" (создана: {oldest.CreatedAt:dd.MM.yyyy})");

                // Самая новая задача
                var newest = tasks.OrderByDescending(t => t.CreatedAt).First();
                Console.WriteLine($"\n🆕 САМАЯ НОВАЯ ЗАДАЧА:");
                Console.WriteLine($"   \"{newest.Title}\" (создана: {newest.CreatedAt:dd.MM.yyyy})");
            }

            // Прогресс-бар
            Console.WriteLine($"\n📊 ПРОГРЕСС:");
            int progress = total > 0 ? completed * 100 / total : 0;
            Console.WriteLine(GetProgressBar(progress / 100.0));
            Console.WriteLine($"   {progress}% выполнено");
        }

        // =========================================================
        // 7. ДЕТАЛИ ЗАДАЧИ
        // =========================================================

        static void ShowTaskDetails()
        {
            ShowTasks();

            if (tasks.Count == 0)
                return;

            Console.Write("\nВведите ID задачи для просмотра деталей: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ Неверный ID! Введите число.");
                return;
            }

            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                Console.WriteLine($"❌ Задача с ID {id} не найдена!");
                return;
            }

            Console.WriteLine("\n" + new string('─', 50));
            Console.WriteLine("🔍 ДЕТАЛИ ЗАДАЧИ");
            Console.WriteLine(new string('─', 50));
            Console.WriteLine(task.GetFullInfo());
            Console.WriteLine(new string('─', 50));
        }

        // =========================================================
        // 8. ОЧИСТИТЬ ВСЕ ЗАДАЧИ
        // =========================================================

        static void ClearAllTasks()
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("📭 Список задач уже пуст.");
                return;
            }

            Console.WriteLine($"⚠️ ВНИМАНИЕ! Вы собираетесь удалить ВСЕ {tasks.Count} задач.");
            Console.Write("Вы уверены? (да/нет): ");
            string confirm = Console.ReadLine();

            if (confirm?.ToLower() == "да" || confirm?.ToLower() == "yes" || confirm?.ToLower() == "д")
            {
                tasks.Clear();
                nextId = 1;
                SaveTasks();
                Console.WriteLine("🧹 Все задачи удалены!");
            }
            else
            {
                Console.WriteLine("❌ Очистка отменена.");
            }
        }

        // =========================================================
        // 9. СОХРАНЕНИЕ И ВЫХОД
        // =========================================================

        static void SaveAndExit()
        {
            SaveTasks();
            Console.WriteLine("💾 Данные сохранены.");
            Console.WriteLine("👋 До свидания!");
        }

        // =========================================================
        // 10. РАБОТА С ФАЙЛОМ
        // =========================================================

        static void SaveTasks()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(dataFile))
                {
                    foreach (var task in tasks)
                    {
                        string line = string.Join("|",
                            task.Id,
                            task.Title,
                            task.Description,
                            task.IsCompleted,
                            task.CreatedAt.ToString("o"),
                            task.CompletedAt?.ToString("o") ?? ""
                        );
                        sw.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при сохранении: {ex.Message}");
            }
        }

        static void LoadTasks()
        {
            if (!File.Exists(dataFile))
                return;

            try
            {
                tasks.Clear();
                nextId = 1;

                using (StreamReader sr = new StreamReader(dataFile))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 6)
                        {
                            var task = new TaskItem
                            {
                                Id = int.Parse(parts[0]),
                                Title = parts[1],
                                Description = parts[2],
                                IsCompleted = bool.Parse(parts[3]),
                                CreatedAt = DateTime.Parse(parts[4]),
                                CompletedAt = !string.IsNullOrEmpty(parts[5]) ? DateTime.Parse(parts[5]) : (DateTime?)null
                            };

                            tasks.Add(task);

                            if (task.Id >= nextId)
                                nextId = task.Id + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при загрузке: {ex.Message}");
            }
        }

        // =========================================================
        // 11. ВСПОМОГАТЕЛЬНЫЙ МЕТОД (ПРОГРЕСС-БАР)
        // =========================================================

        static string GetProgressBar(double percent, int width = 30)
        {
            int filled = (int)(percent * width);
            int empty = width - filled;
            return $"[{"█".PadRight(filled, '█')}{"░".PadRight(empty, '░')}]";
        }
    }
}