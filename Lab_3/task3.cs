//Лабораторная 3. Задание 3
//Написать программу распределения чисел в хеш-таблицу. Имеется n потоков,
//генерирующих случайные целые числа. Для определения местоположения числа x
//в хеш-таблице использовать следующую хеш-функцию:
// int hash(int x)
// {
//    return x % k;
// }
// Если поток собирается записать число x в i-ю строку таблицы,
// то он должен проверить, не пишет ли другой поток свое число в эту же строку.
// Количество строк таблицы и основание хеш-функции (число k) определяется пользователем.
// Операции работы с хеш-таблицей вынести в отдельный класс (реализовать модель монитора)
using System;
using System.Threading;
using System.Collections.Generic;

namespace Lab3_Task3
{
    class Program
    {
        static int k = 3;
        static void Main(string[] args)
        {
            int n = 5;
            Thread[] threads = new Thread[n];

            // Создаем хеш-таблицу и массив мьютексов
            List<int>[] hash_table = new List<int>[k];
            Mutex[] mutexObjects = new Mutex[k];

            for(int i = 0; i < k; i++)
            {
                mutexObjects[i] = new Mutex();
                hash_table[i] = new List<int>();
            }

            // Подготавливаем потоки
            for (int i = 0; i < n; i++)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(AddNumber);
                Thread thread = new Thread(start);
                thread.Name = "Поток №" + i.ToString();
                threads[i] = thread;
            }

            // Запукаем потоки
            for (int i = 0; i < n; i++)
            {
                threads[i].Start(new Argument(hash_table, mutexObjects));
            }

            // Дожидаемся завершения всех потоков
            for (int i = 0; i < n; i++)
                threads[i].Join();

            // Вывод состояни хэш-таблицы
            Console.WriteLine();
            Console.WriteLine("Хэш-тблица");
            for (int i = 0; i < k; i++)
            {
                String values = String.Join(", ", hash_table[i]);
                Console.WriteLine(i.ToString() + ": " + values);
            }
        }

        static void AddNumber(object argument)
        // Функция добавляет число в хэш-таблицу
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(Convert.ToString(id) + " запустился");
            Argument arg = (Argument)argument;

            Random rnd = new Random();
            int value = rnd.Next();
            Console.WriteLine(Convert.ToString(id) + " сгенерировал значение " + value.ToString());
            int hash = GetHash(value);
            Console.WriteLine(Convert.ToString(id) + " сгенерировал хэш " + hash.ToString());

            MyMonitor monitor = new MyMonitor(arg.hash_table, arg.mutexObjects[hash]);
            monitor.AddValueToHashTableRow(hash, value);

            Console.WriteLine(Convert.ToString(id) + " завершился");
        }
        static int GetHash(int value)
        // Функция возвращает хэш
        {
            return value % k;
        }
    }
    class MyMonitor
    // Класс для мьютексной работы с хэш-табицой
    {
        private List<int>[] hash_table;
        private Mutex mutexObj;

        public MyMonitor(List<int>[] hash_table, Mutex mutexObj)
        {
            this.hash_table = hash_table;
            this.mutexObj = mutexObj;
        }

        public void AddValueToHashTableRow(int row_index, int value)
        // Функция добавляет в строку с номером row_index значение value хэш-таблицы
        {
            string id = Thread.CurrentThread.Name;
            Console.WriteLine(id + " ждет открытия мьютекса у строки " + row_index.ToString());
            mutexObj.WaitOne();
            Console.WriteLine(id + " зашел в мьютекс у строки " + row_index.ToString());
            hash_table[row_index].Add(value);
            Console.WriteLine(id + " добавил " + value.ToString() + " в строку " + row_index.ToString());
            mutexObj.ReleaseMutex();
            Console.WriteLine(id + " вышел из мьютекса");
        }
    }

    class Argument
    {
        public List<int>[] hash_table;
        public Mutex[] mutexObjects;

        public Argument(List<int>[] hash_table, Mutex[] mutexObjects)
        {
            this.hash_table = hash_table;
            this.mutexObjects = mutexObjects;
        }
    }
}
