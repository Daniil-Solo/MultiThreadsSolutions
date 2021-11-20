// Лабораторная 1. Задание 1
// Дана матрица из чисел, содержащая m строк и n столбцов.
// Необходимо найти номер столбца матрицы, сумма элементов которого минимальна.
// Реализовать создание строк матрицы параллельным способом

using System;
using System.Threading;

namespace Lab1_Task1
{
    class Program
    {
        const int m = 3; // количество строк
        const int n = 3; // количество столбцов

        static void Main(string[] args)
        {
            float[][] matrix = new float[m][]; // матрица для хранения чисел
            Thread[] threads = new Thread[m]; // массив для хранения потоков

            // Создаем потоки для выделения памяти под строки матрицы
            for (int i = 0; i < m; i++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(CreateRow);
                Thread thread = new Thread(start);
                thread.Name = "Поток №" + i.ToString();
                threads[i] = thread;
            }

            // Запускаем потоки
            for (int i=0; i<m; i++)
            {
                threads[i].Start(new Argument(matrix, i));
            }

            // Ждем завершения каждого потока
            for (int i = 0; i < m; i++)
            {
                threads[i].Join();
            }

            // Заполняем матрицу случайными значениями от 0 до 2
            Random random = new Random(12345);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i][j] = (byte)random.Next()%3;
                    Console.Write(Convert.ToString(matrix[i][j]) + "\t");
                }
                Console.WriteLine();
            }

            // Создаем массив с результатами суммирования
            float[] sums = new float[n];

            // Создаем потоки для подсчета суммы в каждом столбце
            threads = new Thread[n];

            for (int i = 0; i < n; i++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(SumElementInColumn);
                Thread thread = new Thread(start);
                thread.Name = "Поток №" + i.ToString();
                threads[i] = thread;
            }

            // Запускаем потоки
            for (int i = 0; i < n; i++)
            {
                threads[i].Start(new Argument2(matrix, i, sums));
            }

            // Ждем завершения каждого потока
            for (int i = 0; i < n; i++)
            {
                threads[i].Join();
            }

            // Определяем номер столбца с минимальной суммой элементов
            int index_min_sum = 0;
            float min_sum = float.MaxValue;
            for(int i=0; i<n; i++)
            {
                if (min_sum > sums[i])
                {
                    min_sum = sums[i];
                    index_min_sum = i;
                }
            }
            Console.WriteLine("Столбец с наименьшей суммой элементов под номером " + index_min_sum.ToString() + ", с суммой равной " + min_sum.ToString());
        }

        public static void CreateRow(object argument)
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " начался");
            Argument arg = (Argument)argument;
            arg.matrix[arg.index] = new float[n];
            Console.WriteLine(Convert.ToString(id) + " закончился");
        }

        public static void SumElementInColumn(object argument)
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " начался");
            Argument2 arg = (Argument2)argument;
            float sum = 0;
            for (int i = 0; i < m; i++)
                sum += arg.matrix[i][arg.index];
            arg.sums[arg.index] = sum;
            Console.WriteLine(Convert.ToString(id) + " закончился");
        }

    }

    class Argument
    {
        public float[][] matrix;
        public int index;

        public Argument(float[][] matrix, int index)
        {
            this.matrix = matrix;
            this.index = index;
        }
    }
    class Argument2:Argument
    {
        public float[] sums;

        public Argument2(float[][] matrix, int index, float[] sums): base(matrix, index)
        {
            this.sums = sums;
        }
    }
}
