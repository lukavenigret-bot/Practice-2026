using System;

namespace DoorMathLibrary
{
    /// <summary>
    /// Класс-калькулятор для математических операций
    /// </summary>
    public class Calculator
    {
        /// <summary>
        /// Сложение двух чисел
        /// </summary>
        public static int Add(int a, int b) => a + b;

        /// <summary>
        /// Вычитание двух чисел
        /// </summary>
        public static int Subtract(int a, int b) => a - b;

        /// <summary>
        /// Умножение двух чисел
        /// </summary>
        public static int Multiply(int a, int b) => a * b;

        /// <summary>
        /// Деление двух чисел
        /// </summary>
        public static double Divide(int a, int b)
        {
            if (b == 0)
                throw new DivideByZeroException("Деление на ноль невозможно");
            return (double)a / b;
        }

        /// <summary>
        /// Возведение в степень
        /// </summary>
        public static double Power(double a, double b) => Math.Pow(a, b);

        /// <summary>
        /// Расчёт площади прямоугольника
        /// </summary>
        public static double RectangleArea(double width, double height) => width * height;

        /// <summary>
        /// Расчёт площади круга
        /// </summary>
        public static double CircleArea(double radius) => Math.PI * radius * radius;
    }
}