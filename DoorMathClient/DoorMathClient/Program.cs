using System;
using DoorMathLibrary;  // Подключение библиотеки

namespace DoorMathClient
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== ИСПОЛЬЗОВАНИЕ БИБЛИОТЕКИ DoorMathLibrary ===\n");

            // Демонстрация работы методов
            Console.WriteLine("1. Базовые математические операции:");
            Console.WriteLine($"  10 + 5 = {Calculator.Add(10, 5)}");
            Console.WriteLine($"  10 - 5 = {Calculator.Subtract(10, 5)}");
            Console.WriteLine($"  10 * 5 = {Calculator.Multiply(10, 5)}");
            Console.WriteLine($"  10 / 3 = {Calculator.Divide(10, 3):F2}");

            Console.WriteLine("\n2. Расчёт площади:");
            Console.WriteLine($"  Прямоугольник 5×3: {Calculator.RectangleArea(5, 3):F2} кв.ед.");
            Console.WriteLine($"  Круг радиусом 4: {Calculator.CircleArea(4):F2} кв.ед.");

            Console.WriteLine("\n3. Возведение в степень:");
            Console.WriteLine($"  2^10 = {Calculator.Power(2, 10)}");
            Console.WriteLine($"  3^4 = {Calculator.Power(3, 4)}");

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}