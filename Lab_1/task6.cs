// Лабораторная 1. Задание 6
// Дан текст длиной n символов и ключ длиной k символов. Осуществите блочное шифрование текста по следующему алгоритму:
// 1.Разделить текст на блоки длиной k символов. Если n не кратно k, то допустимо, чтобы длина последнего блока была меньше k.
// 2. Внутри каждого блока выполнить перестановку символов так, чтобы первый символ занял место последнего, второй – предпоследнего и т.д.
// 3. Применить ключ к каждому блоку. Шифрованный i-й символ блока должен быть получен, как результат "исключающего или"
// между i-м исходным символом блока и i-м символом ключа.

using System;
using System.IO;
using System.Threading;

namespace Lab1_Task6
{
    class Program
    {
        static void Main(string[] args)
        {
            // Интерфейсная часть
            Console.WriteLine("Введите строку: ");
            string inputString = Console.ReadLine();
            Console.WriteLine("Введите ключ: ");
            string inputKey = Console.ReadLine();
            Console.WriteLine();
            string outputString = Encrypt(inputString, inputKey);
            Console.WriteLine();
            Console.WriteLine("Результат кодирования: ");
            Console.WriteLine(outputString);

            // Запись в файл
            StreamWriter f = new StreamWriter("encryptedText.txt");
            f.WriteLine(outputString);
            f.Close();
        }

        static string Encrypt(string inputString, string inputKey)
        {
            string result = "";

            int nBlock;
            char[][] blocks;

            int n = inputString.Length;
            int k = inputKey.Length;

            // Определяем количество блоков
            if (n % k == 0)
                nBlock = n / k;
            else
                nBlock = n / k + 1;

            // Разделяем строку на блоки по к символов
            blocks = new char[nBlock][];
            for (int i = 0; i < nBlock; i++)
            {
                blocks[i] = new char[k];
                for (int j = 0; j < k; j++)
                    try
                    {
                        blocks[i][j] = inputString[i * k + j];
                    }
                    catch
                    {
                        blocks[i][j] = '\n';
                    }
            }
                    
            // Подготавливаем потоки
            Thread[] threads = new Thread[nBlock];
            for(int i = 0; i < nBlock; i++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(EncryptBlock);
                Thread thread = new Thread(start);
                thread.Name = "Поток №" + i.ToString();
                threads[i] = thread;    
            }
               
            // Запукаем потоки
            for(int i = 0; i < nBlock; i++)
            {
                threads[i].Start(new Argument(i, blocks, inputKey));
            }

            // Дожидаемся завершения всех потоков
            for (int i = 0; i < nBlock; i++)
                threads[i].Join();

            // Собираем по блокам зашифрованную строку
            for (int i = 0; i < nBlock; i++)
                for(int j = 0; j < k; j++)
                    result += ((byte)blocks[i][j]).ToString() + " ";

            return result;
        }

        static void EncryptBlock(object argument)
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " запустился");

            // Меняем местами элементы по принципу: первый <-> последний
            Argument arg = (Argument)argument;
            int n = arg.key.Length;
            for (int i = 0; i < n/2; i++)
            {
                char temp = arg.blocks[arg.blockNumber][i];
                arg.blocks[arg.blockNumber][i] = arg.blocks[arg.blockNumber][n-i-1];
                arg.blocks[arg.blockNumber][n-i-1] = temp;
            }
            Console.WriteLine(Convert.ToString(id) + " обратил");

            // Шифруем с помощью исключающего ИЛИ между элементами блока и ключа
            for (int i = 0; i < n; i++)
            {
                arg.blocks[arg.blockNumber][i] = (char)((byte)(arg.blocks[arg.blockNumber][i]) ^ (byte)(arg.key[i]));
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
