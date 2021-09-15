// Лабораторная 2 Задание 3
// Дана функция y = f(x), принимающая на отрезке [a,b] только положительные значения.
// Необходимо найти приближенное значение площади криволинейной трапеции, ограниченной
// кривой y = f(x), прямыми x = a, x = b и осью абсцисс, разбив отрезок [a,b] на n элементов.
// Для подсчета площади каждого элемента криволинейной трапеции запустить отдельный поток,
// вычисляющий площадь этого элемента трапеции как площадь криволинейной трапеции, ограниченной
// кривой y = f(x), прямыми x = xi, x = xi+1, где i – номер элемента,  и осью абсцисс,
// методом прямоугольников, разбивая отрезок [xi, xi+1] на m участков.
// Пояснение: делим исходную “фигуру” на n частей, каждую из этих n частей делим на m частей.
// Каждую из m частей считать методом прямоугольников.

using System;
using System.Threading;


namespace Lab2_Task3
{
    class Program
    {
        static object locker = new object();

        static float sum = 0;

        static int n = 10; // Количество потоков
        static int m = 100; // Количество сегментов для потока

        static float a = 0;  // Левая граница
        static float b = 10; // Правая граница
        
        static void Main(string[] args)
        {
            Thread[] threads = new Thread[n];
            // Создаем потоки для обработки части строки
            for (int i = 0; i < n; i++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(PartIntegral);
                Thread thread = new Thread(start);
                thread.Name = "Поток №" + i.ToString();
                threads[i] = thread;
            }

            float delta = (b - a) / n;

            // Запускаем потоки
            for (int i = 0; i < n; i++)
            {
                float left = a + i * delta;
                float right = a + (i + 1) * delta;
                threads[i].Start(new Argument(left, right));
            }
            // Ждем завершения каждого потока
            for (int i = 0; i < n; i++)
            {
                threads[i].Join();
            }

            Console.WriteLine("Функция " + Function.Name + " Диапазон [" + a.ToString() + "; " + b.ToString() + "]");
            Console.WriteLine("Приближенное значение интеграла равно " + Math.Round(sum, 3).ToString());
        }

        public static float RectangleMethod(float left, float right)
        {
            // Функция вычисляет площадь сегмента с помощью метода прямоугольников
            return new Function((left + right) / 2).Calculate() * (right - left);
        }

        public static void PartIntegral(object argument)
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " запустился");

            Argument arg = (Argument)argument;
            float delta = (arg.b - arg.a) / m;

            for(int i = 0; i < m; i++)
            {
                float left = arg.a + i * delta;
                float right = arg.a + (i + 1) * delta;

                float segment = RectangleMethod(left, right);

                lock(locker)
                {
                    sum += segment;
                }

            }

            Console.WriteLine(Convert.ToString(id) + " завершился");
        }
    }

    class Argument
    {
        public float a;
        public float b;

        public Argument(float a, float b)
        {
            this.a = a;
            this.b = b;
        }
    }

    class Function
    {
        public static String Name = "f(x)=x";
        public float x;
        
        public Function(float x)
        {
            this.x = x;
        }

        public float Calculate()
        {
            return x;
        }
    }
}
