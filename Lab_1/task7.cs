// Лабораторная 1. Задание 7
// Дешифрование текста из предыдущего задания

using System;
using System.IO;
using System.Threading;

namespace Lab1_Task7
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputString;
            string inputKey;

            Console.WriteLine("1 - из файла encryptedText.txt, 2 - ввод в консоли");
            string answ = Console.ReadLine(); 
            if (answ == "1")
            {
                StreamReader f = new StreamReader("encryptedText.txt");
                inputString = f.ReadLine();
                inputKey = f.ReadLine();
                f.Close();
                Console.WriteLine("Данные берутся из файла encryptedText.txt");
            }
            else
            {
                Console.WriteLine("Введите зашифрованную строку: ");
                inputString = Console.ReadLine();
                Console.WriteLine("Введите ключ: ");
                inputKey = Console.ReadLine();
            }
            Console.WriteLine();
            string outputString = Decrypt(inputString, inputKey);
            Console.WriteLine();
            Console.WriteLine("Результат декодирования: ");
            Console.WriteLine(outputString);
        }

        static string Decrypt(string inputString, string inputKey)
        {
            string result = "";

            int nBlock;
            char[][] blocks;

            string[] symbols = inputString.Trim().Split(' ');

            int n = symbols.Length;
            int k = inputKey.Length;

            // Определяем количество блоков
            if (n % k == 0)
                nBlock = n / k;
            else
                nBlock = n / k + 1;

            // Заполняем блоки, переводя число в char
            blocks = new char[nBlock][];
            for (int i = 0; i < nBlock; i++)
            {
                blocks[i] = new char[k];
                for (int j = 0; j < k; j++)
                    try
                    {
                        blocks[i][j] = (char)Convert.ToInt32(symbols[i * k + j]);
                    }
                    catch
                    {
                        blocks[i][j] = ' ';
                    }
            }

            // Подготавливаем потоки
            Thread[] threads = new Thread[nBlock];
            for (int i = 0; i < nBlock; i++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(DecryptBlock);
                Thread thread = new Thread(start);
                thread.Name = "Поток №" + i.ToString();
                threads[i] = thread;
            }

            // Запукаем потоки
            for (int i = 0; i < nBlock; i++)
            {
                threads[i].Start(new Argument(i, blocks, inputKey));
            }

            // Дожидаемся завершения всех потоков
            for (int i = 0; i < nBlock; i++)
                threads[i].Join();

            // Собираем по блокам зашифрованную строку
            for (int i = 0; i < nBlock; i++)
                for (int j = 0; j < k; j++)
                    result += blocks[i][j];

            return result;
        }

        static void DecryptBlock(object argument)
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " запустился");

            // Дешифруем с помощью исключающего ИЛИ между элементами блока и ключа
            Argument arg = (Argument)argument;
            int n = arg.key.Length;
            for (int i = 0; i < n; i++)
            {
                arg.blocks[arg.blockNumber][i] = (char)((byte)(arg.blocks[arg.blockNumber][i]) ^ (byte)(arg.key[i]));
            }
            Console.WriteLine(Convert.ToString(id) + " выполнил xor");

            // Меняем местами элементы по принципу: первый <-> последний
            for (int i = 0; i < n / 2; i++)
            {
                char temp = arg.blocks[arg.blockNumber][i];
                arg.blocks[arg.blockNumber][i] = arg.blocks[arg.blockNumber][n - i - 1];
                arg.blocks[arg.blockNumber][n - i - 1] = temp;
            }

            Console.WriteLine(Convert.ToString(id) + " завершился");
        }
    }

    class Argument
    {
        public int blockNumber;
        public char[][] blocks;
        public string key;

        public Argument(int blockNumber, char[][] blocks, string key)
        {
            this.blockNumber = blockNumber;
            this.blocks = blocks;
            this.key = key;
        }
    }
}
