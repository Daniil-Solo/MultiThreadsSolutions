// Лабораторная 1. Задание 3
// Дана матрица из натуральных чисел, содержащая n строк и m столбцов.
// Найдите результат умножения матрицы на множитель, вводимый пользователем.

using System;
using System.Threading;

namespace Lab1_Task3
{
    static class Program
    {
        const int n = 10; // количество строк
        const int m = 2000; // количество столбцов
        const int max_value = 10; // максимальное значение элемента матрицы

        static void Main(string[] args)
        {
            int[][] matrix = new int[n][]; // матрица для хранения чисел
            Thread[] threads = new Thread[n]; // массив для хранения потоков

            // Создаем потоки для выделения памяти под строки матрицы
            for (int i = 0; i < n; i++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(CreateRow);
                Thread thread = new Thread(start);
                thread.Name = "Поток №" + i.ToString();
                threads[i] = thread;
            }

            // Запускаем потоки
            for (int i = 0; i < n; i++)
            {
                threads[i].Start(new Argument(matrix, i));
            }

            // Ждем завершения каждого потока
            for (int i = 0; i < n; i++)
            {
                threads[i].Join();
            }

            // Заполняем матрицу случайными значениями от 0 до max_value
            Console.WriteLine("-----------------------------Печать матрицы---------------------------");
            Random random = new Random(12345);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    matrix[i][j] = (byte)random.Next() % (max_value + 1);
                    Console.Write(Convert.ToString(matrix[i][j]) + "\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine();

            // Вводим целое число
            Console.WriteLine("---------------------------Введите целое значение множителя---------------------------");
            int user_number = int.Parse(Console.ReadLine());

            // Создаем потоки для перемножения
            threads = new Thread[n];

            for (int i = 0; i < n; i++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(MultK);
                Thread thread = new Thread(start);
                thread.Name = "Поток №" + i.ToString();
                threads[i] = thread;
            }

            // Запускаем потоки
            for (int i = 0; i < n; i++)
            {
                threads[i].Start(new Argument2(matrix, i, user_number));
            }

            // Ждем завершения каждого потока
            for (int i = 0; i < n; i++)
            {
                threads[i].Join();
            }

            // Печатаем новую матрицу
            Console.WriteLine("-----------------------------Печать матрицы---------------------------");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write(Convert.ToString(matrix[i][j]) + "\t");
                }
                Console.WriteLine();
            }
        }

        public static void CreateRow(object argument)
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " запустился");

            Argument arg = (Argument)argument;
            arg.matrix[arg.index] = new int[m];

            Console.WriteLine(Convert.ToString(id) + " завершился");
        }

        public static void MultK(object argument)
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " запустился");

            Argument2 arg = (Argument2)argument;
            for (int i = 0; i < m; i++)
                arg.matrix[arg.index][i] *= arg.k;

            Console.WriteLine(Convert.ToString(id) + " завершился");
        }

    }
    class Argument
    {
        public int[][] matrix;
        public int index;

        public Argument(int[][] matrix, int index)
        {
            this.matrix = matrix;
            this.index = index;
        }
    }
    class Argument2 : Argument
    {
        public int k;

        public Argument2(int[][] matrix, int index, int k) : base(matrix, index)
        {
            this.k = k;
        }
    }
}
