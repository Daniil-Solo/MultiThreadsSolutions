// Лабораторная 1. Задание 2
// Дана матрица из целых чисел, содержащая n строк и m столбцов.
// Отсортируйте каждую строку матрицы по убыванию методом подсчёта.

using System;
using System.Threading;

namespace Lab1_Task2
{
    class Program
    {
        const int n = 10;// количество строк
        const int m = 10; // количество столбцов
        const byte max_value = 100; // максимальное значение элементам массива

        static void Main(string[] args)
        {
            byte[][] matrix = new byte[n][]; // матрица для хранения чисел
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

            // Заполняем матрицу случайными значениями от 0 до 2
            Random random = new Random(12345);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    matrix[i][j] = (byte)(random.Next() % (max_value+1));
                    Console.Write(Convert.ToString(matrix[i][j]) + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------------Сортировка-------------------------------------");
            Console.WriteLine();

            // Создаем потоки для сортировки
            for (int i = 0; i < n; i++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(CountSort);
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

            // Вывод отсортированной матрицы
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
            arg.matrix[arg.index] = new byte[n];
            Console.WriteLine(Convert.ToString(id) + " завершился");
        }

        public static void CountSort(object argument)
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " запустился");

            Argument arg = (Argument)argument;
            byte[] array = arg.matrix[arg.index];

            byte[] c = new byte[max_value + 1];
            for (int i = 0; i < c.Length; i++)
                c[i] = 0;

            for (int i = 0; i < m; i++)
                ++c[array[i]];

            Console.WriteLine(Convert.ToString(id) + " заполнил массив счетчиков");

            int b = 0;
            for (byte i = 0; i < max_value + 1; ++i)
                for (int j = 0; j < c[i]; ++j)
                    array[b++] = i;

            Console.WriteLine(Convert.ToString(id) + " завершился");
        }

    }

    class Argument
    {
        public byte[][] matrix;
        public int index;

        public Argument(byte[][] matrix, int index)
        {
            this.matrix = matrix;
            this.index = index;
        }
    }
}
