// Лабораторная 3 Задание 1
// Написать программу для нахождения произведения векторов, значения которых вводятся пользователем.
// Каждое скалярное произведение должно обрабатываться отдельным потоком.

using System.Threading;
using System;

namespace Lab3_Task1
{
    class Program
    {
        static int n;
        static int sum = 0;
        static Mutex mutexObj = new Mutex();
        static void Main(string[] args)
        {
            int[] vector1 = null;
            int[] vector2 = null;
            int[] vector3 = null;

            Console.WriteLine("Введите длину для векторов");
            n = Int32.Parse(Console.ReadLine());

            // Заполнение 1-ого вектора
            Console.WriteLine("Первый вектор");
            vector1 = new int[n];
            for (int i = 1; i <= n; i++)
            {
                Console.WriteLine("Введите значение " + i.ToString() + "-ой координаты");
                vector1[i - 1] = Int32.Parse(Console.ReadLine());
            }

            Console.WriteLine();

            // Заполнение 2-ого вектора
            Console.WriteLine("Второй вектор");
            vector2 = new int[n];
            for (int i = 1; i <= n; i++)
            {
                Console.WriteLine("Введите значение " + i.ToString() + "-ой координаты");
                vector2[i - 1] = Int32.Parse(Console.ReadLine());
            }

            Console.WriteLine();

            Console.WriteLine("Будет ли третий вектор? y/n");
            char answer = (char)Console.ReadLine()[0];
            if (answer == 'y')
            {
                Console.WriteLine();
                // Заполнение 3-ого вектора
                Console.WriteLine("Третий вектор");
                vector3 = new int[n];
                for (int i = 1; i <= n; i++)
                {
                    Console.WriteLine("Введите значение " + i.ToString() + "-ой координаты");
                    vector3[i - 1] = Int32.Parse(Console.ReadLine());
                }
            }
            
            // Подготавливаем потоки
            Thread[] threads = new Thread[n];
            for (int i = 0; i < n; i ++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(ScalarMultPart);
                Thread thread = new Thread(start);
                thread.Name = "Поток №" + i.ToString();
                threads[i] = thread;
            }

            // Запукаем потоки
            for (int i = 0; i < n; i++)
            {
                threads[i].Start(new Argument(vector1[i], vector2[i]));
            }

            // Дожидаемся завершения всех потоков
            for (int i = 0; i < n; i++)
                threads[i].Join();

            Console.WriteLine();

            // Выводим ответ
            Console.WriteLine("Вектор1: " + GetStringVectorView(vector1, n));
            Console.WriteLine("Вектор2: " + GetStringVectorView(vector2, n));
            if (answer == 'y')
            {
                Console.WriteLine("Вектор3: " + GetStringVectorView(vector3, n));
                for (int i = 0; i < n; i++)
                    vector3[i] = sum * vector3[i];
                Console.WriteLine("Скалярное произведение: " + GetStringVectorView(vector3, n));
            }
            else
                Console.WriteLine("Скалярное произведение: " + sum.ToString());
        }

        static void ScalarMultPart(object argument)
        // Функция для перемножения двух координат 
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " запустился");
            Argument arg = (Argument)argument;

            mutexObj.WaitOne();
            Console.WriteLine(Convert.ToString(id) + " трудится");
            sum += arg.a * arg.b;
            Thread.Sleep(1000);
            Console.WriteLine(Convert.ToString(id) + " завершился");
            mutexObj.ReleaseMutex();
        }

        static string GetStringVectorView(int [] vector, int n)
        // Функция для представления вектора в строков виде
        {
            String result = "(";
            for (int i = 0; i < n; i++)
            {
                result += vector[i].ToString();
                if (i != n - 1)
                    result += ", ";
            }
            result += ")";
            return result;
        }
    }

    class Argument
    {
        public int a;
        public int b;

        public Argument(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
    }
}
