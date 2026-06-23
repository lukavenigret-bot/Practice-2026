using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingDemo
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== ДЕМОНСТРАЦИЯ ПОТОКОВ ===\n");

            // =====================================================
            // 1. Ручное создание потоков (Thread)
            // =====================================================

            Console.WriteLine("1. РУЧНОЕ СОЗДАНИЕ ПОТОКОВ (Thread):");

            Thread thread1 = new Thread(() => DoWork("Поток A", 200));
            Thread thread2 = new Thread(() => DoWork("Поток B", 150));

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Console.WriteLine("   Все потоки завершены\n");

            // =====================================================
            // 2. Использование Task (TPL)
            // =====================================================

            Console.WriteLine("2. ИСПОЛЬЗОВАНИЕ TASK (TPL):");

            Task task1 = Task.Run(() => DoWork("Задача 1", 180));
            Task task2 = Task.Run(() => DoWork("Задача 2", 120));

            Task.WaitAll(task1, task2);
            Console.WriteLine("   Все задачи завершены\n");

            // =====================================================
            // 3. Параллельный цикл (Parallel.For)
            // =====================================================

            Console.WriteLine("3. ПАРАЛЛЕЛЬНЫЙ ЦИКЛ (Parallel.For):");

            Parallel.For(0, 10, i =>
            {
                Console.WriteLine($"   Итерация {i} в потоке {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(50);
            });

            Console.WriteLine("\n   Параллельный цикл завершён\n");

            // =====================================================
            // 4. Демонстрация гонки данных (Race Condition)
            // =====================================================

            Console.WriteLine("4. ДЕМОНСТРАЦИЯ ГОНКИ ДАННЫХ:");

            int counter = 0;
            Parallel.For(0, 1000, i =>
            {
                // Небезопасное увеличение счётчика
                counter++;
            });

            Console.WriteLine($"   Ожидаемое значение: 1000");
            Console.WriteLine($"   Фактическое значение: {counter} (без синхронизации)");

            // С синхронизацией
            int safeCounter = 0;
            object lockObject = new object();
            Parallel.For(0, 1000, i =>
            {
                lock (lockObject)
                {
                    safeCounter++;
                }
            });

            Console.WriteLine($"   Синхронизированное значение: {safeCounter}\n");

            // =====================================================
            // 5. Демонстрация параллельного расчёта
            // =====================================================

            Console.WriteLine("5. ПАРАЛЛЕЛЬНЫЙ РАСЧЁТ (сумма квадратов):");

            int[] numbers = new int[1000000];
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = i + 1;
            }

            // Последовательный расчёт
            var sw = System.Diagnostics.Stopwatch.StartNew();
            long sum = 0;
            foreach (int n in numbers)
            {
                sum += n * n;
            }
            sw.Stop();
            Console.WriteLine($"   Последовательно: {sum:N0} за {sw.ElapsedMilliseconds} мс");

            // Параллельный расчёт
            sw.Restart();
            long parallelSum = 0;
            object sumLock = new object();
            Parallel.For(0, numbers.Length, i =>
            {
                long square = (long)numbers[i] * numbers[i];
                lock (sumLock)
                {
                    parallelSum += square;
                }
            });
            sw.Stop();
            Console.WriteLine($"   Параллельно: {parallelSum:N0} за {sw.ElapsedMilliseconds} мс");

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Метод, выполняемый в потоке
        /// </summary>
        static void DoWork(string name, int delay)
        {
            for (int i = 1; i <= 3; i++)
            {
                Console.WriteLine($"   {name}: шаг {i} (поток {Thread.CurrentThread.ManagedThreadId})");
                Thread.Sleep(delay);
            }
        }
    }
}