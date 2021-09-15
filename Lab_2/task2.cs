// Лабораторная 2 Задание 2
// Дан текст длиной n символов. Необходимо найти контрольную сумму по данному тексту
// в виде суммы кодов символов по модулю 256. Для нахождения суммы запустите k потоков, где k < n.
// Каждый i-й поток, должен обрабатывать только символы с номерами i + k *s, где s – шаг работы потока.

using System;
using System.Threading;

namespace Lab2_Task2
{
    class Program
    {
        static object locker = new object();
        static int k = 10;
        static int controlSum = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Введите текст");
            String input = Console.ReadLine();
            if (k > input.Length)
                k = input.Length;
            CalculateControlSum(input);
            Console.WriteLine("Контрольная сумма: " + controlSum.ToString());
        }

        public static void CalculateControlSum(String input)
        {
            Thread[] threads = new Thread[k];
            // Создаем потоки для обработки части строки
            for (int i = 0; i < k; i++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(PartStringHandler);
                Thread thread = new Thread(start);
                thread.Name = "Поток №" + i.ToString();
                threads[i] = thread;
            }
            // Запускаем потоки
            for (int i = 0; i < k; i++)
            {
                threads[i].Start(new Argument(input, i));
            }
            // Ждем завершения каждого потока
            for (int i = 0; i < k; i++)
            {
                threads[i].Join();
            }
        }

        public static void PartStringHandler(object argument)
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " запустился");

            Argument arg = (Argument)argument;
            int index = arg.i;
            while (index < arg.allString.Length)
            {
                int code = ((int)arg.allString[index])%256;
                lock(locker)
                {
                    controlSum += code;
                }
                index += k;
            }

            Console.WriteLine(Convert.ToString(id) + " завершился");
        }
    }
    class Argument
    {
        public String allString;
        public int i;

        public Argument(String allString, int i)
        {
            this.allString = allString;
            this.i = i;
        }
    }
}
