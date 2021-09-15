// Лабораторная 2 Задание 1
// Необходимо сформировать матрицу вещественных чисел, содержащую n строк и m столбцов,
// а затем найти ее сумму. Формирование строк матрицы должно выполняться параллельно n потоками.
// Подсчет суммы элементов также должен выполняться n параллельными потоками.
// Поток подсчета суммы по строке может быть запущен только тогда, когда поток формирования строки
// матрицы закончит свою работу. 

using System;
using System.Threading;

namespace Lab2_Task1
{
    static class Program
    {
        static object locker = new object();
        static float sum = 0; // сумма элементов матрицы
        const int n = 10; // количество строк
        const int m = 10; // количество столбцов

        static void Main(string[] args)
        {
            float[][] matrix = new float[n][]; // матрица для хранения чисел
            Thread[] threads = new Thread[n]; // массив для хранения потоков для генерации строк матрицы
            bool[] stoppedThreads = new bool[n]; // массив для хранения состояния потока (True-поток завершил работу, False-поток еще выполняет задачу)
            Thread[] threads2 = new Thread[n]; // массив для хранения потоков для подсчета суммы

            // Создаем потоки для выделения памяти под строки матрицы и потоки для подсчета суммы элементов в строке
            for (int i = 0; i < n; i++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(CreateRow);
                Thread thread = new Thread(start);
                thread.Name = "Поток A" + i.ToString();
                threads[i] = thread;
                stoppedThreads[i] = false;

                ParameterizedThreadStart start2 = new ParameterizedThreadStart(SumElementsRow);
                Thread thread2 = new Thread(start2);
                thread2.Name = "Поток B" + i.ToString();
                threads2[i] = thread2;
            }

            // Запускаем потоки для формирования строк матрицы
            for (int i = 0; i < n; i++)
            {
                threads[i].Start(new Argument(matrix, i));
            }

            // Ждем завершения каждого потока
            while(true)
            {
                // Проверяем потоки для создания строк. Если один из потоков завершился, то он запускает поток по подсчету элементов в этой строке
                for(int i = 0; i < n; i++)
                    if (threads[i].ThreadState == ThreadState.Stopped && !stoppedThreads[i])
                    {
                        Console.WriteLine(threads[i].Name + " запускает " + threads2[i].Name);
                        stoppedThreads[i] = true;
                        threads2[i].Start(new Argument(matrix, i));
                    }

                // Проверяем потоки для подсчета. Если завершились все, то можно выходить из бесконечного цикла
                bool readyToBreak = true;
                for (int i = 0; i < n; i++)
                    if (threads2[i].ThreadState != ThreadState.Stopped)
                        readyToBreak = false;
                if (readyToBreak)
                    break;
            }

            // Вывод матрицы и суммы элементов
            Console.WriteLine();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                    Console.Write(matrix[i][j].ToString() + "\t");
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Сумма элементов матрицы " + Math.Round(sum, 2).ToString());
        }

        public static void CreateRow(object argument)
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " запустился для формирования строки");

            Argument arg = (Argument)argument;
            arg.matrix[arg.index] = new float[m];

            Console.WriteLine(Convert.ToString(id) + " выделил память под строку");

            Random rand = new Random();
            for (int i = 0; i < m; i++)
                arg.matrix[arg.index][i] = (float)(rand.Next(100, 10000) / 100.0);

            Console.WriteLine(Convert.ToString(id) + " сгенерировал значения");
        }

        public static void SumElementsRow(object argument)
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " запустился для подсчета строки");

            Argument arg = (Argument)argument;

            for(int i = 0; i < m; i++)
            {
                lock(locker)
                {
                    sum += arg.matrix[arg.index][i];
                }
            }

            Console.WriteLine(Convert.ToString(id) + " завершил подсчет строки");
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
}
