using System;

namespace LowLevelDemo
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== ДЕМОНСТРАЦИЯ РАБОТЫ С УКАЗАТЕЛЯМИ ===\n");

            // =====================================================
            // 1. БАЗОВАЯ РАБОТА С УКАЗАТЕЛЯМИ
            // =====================================================

            unsafe
            {
                // Объявление переменной
                int number = 42;
                Console.WriteLine($"Исходное значение: {number}");

                // Создание указателя на переменную
                int* pointer = &number;

                // Вывод адреса и значения через указатель
                Console.WriteLine($"Адрес переменной: {(long)pointer:X}");
                Console.WriteLine($"Значение через указатель: {*pointer}");

                // Изменение значения через указатель
                *pointer = 100;
                Console.WriteLine($"Новое значение (изменено через указатель): {number}");

                Console.WriteLine("\n" + new string('─', 50) + "\n");
            }

            // =====================================================
            // 2. РАБОТА С МАССИВАМИ ЧЕРЕЗ УКАЗАТЕЛИ
            // =====================================================

            unsafe
            {
                // Создание массива в стеке (быстрое выделение)
                int* array = stackalloc int[5];

                // Заполнение массива через указатели
                for (int i = 0; i < 5; i++)
                {
                    array[i] = (i + 1) * 10;
                }

                Console.WriteLine("Массив, созданный в стеке:");
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"  array[{i}] = {array[i]} (адрес: {(long)&array[i]:X})");
                }

                Console.WriteLine("\n" + new string('─', 50) + "\n");
            }

            // =====================================================
            // 3. АРИФМЕТИКА УКАЗАТЕЛЕЙ
            // =====================================================

            unsafe
            {
                int[] numbers = { 10, 20, 30, 40, 50 };

                // Фиксация массива в памяти (для работы с указателем)
                fixed (int* ptr = numbers)
                {
                    Console.WriteLine("Арифметика указателей:");

                    // Перемещение по массиву с помощью указателя
                    for (int i = 0; i < numbers.Length; i++)
                    {
                        int* current = ptr + i;
                        Console.WriteLine($"  Элемент {i}: значение = {*current}, адрес = {(long)current:X}");
                    }

                    Console.WriteLine();

                    // Демонстрация инкремента указателя
                    int* ptr2 = ptr;
                    Console.WriteLine($"Начальный адрес: {(long)ptr2:X}, значение: {*ptr2}");

                    ptr2++; // Переход к следующему элементу (сдвиг на 4 байта)
                    Console.WriteLine($"После ptr2++: {(long)ptr2:X}, значение: {*ptr2}");

                    ptr2 += 2; // Переход на 2 элемента вперёд
                    Console.WriteLine($"После ptr2 += 2: {(long)ptr2:X}, значение: {*ptr2}");
                }

                Console.WriteLine("\n" + new string('─', 50) + "\n");
            }

            // =====================================================
            // 4. РАБОТА СО СТРУКТУРАМИ ЧЕРЕЗ УКАЗАТЕЛИ
            // =====================================================

            unsafe
            {
                // Создание структуры
                DoorData door = new DoorData
                {
                    Id = 1,
                    Width = 800,
                    Height = 2000,
                    Price = 15000.0
                };

                Console.WriteLine("Структура до использования указателей:");
                Console.WriteLine($"  Id: {door.Id}");
                Console.WriteLine($"  Ширина: {door.Width} мм");
                Console.WriteLine($"  Высота: {door.Height} мм");
                Console.WriteLine($"  Цена: {door.Price:N0} ₽");

                // Указатель на структуру
                DoorData* doorPtr = &door;

                // Изменение полей через указатель
                doorPtr->Width = 900;
                doorPtr->Height = 2100;
                doorPtr->Price = 25000.0;

                Console.WriteLine("\nСтруктура после изменения через указатель:");
                Console.WriteLine($"  Id: {doorPtr->Id}");
                Console.WriteLine($"  Ширина: {doorPtr->Width} мм");
                Console.WriteLine($"  Высота: {doorPtr->Height} мм");
                Console.WriteLine($"  Цена: {doorPtr->Price:N0} ₽");
            }
        }

        /// <summary>
        /// Структура для демонстрации работы с указателями
        /// </summary>
        unsafe struct DoorData
        {
            public int Id;
            public int Width;
            public int Height;
            public double Price;
        }
    }
}