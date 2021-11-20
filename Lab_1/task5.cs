using System;
using System.Collections.Generic;
using System.Threading;
// Дано бинарное дерево, элементы которого содержат целые числа.
// Найдите сумму элементов этого дерева и среднее значение.

namespace Lab1_Task5
{
    class Program
    {
        static float sum = 0;
        static int thread_number = 1;
        static List<Thread> threads = new List<Thread>();
        static void Main(string[] args)
        {
            Tree my_tree = BuildAndGetTree(100);
            ParameterizedThreadStart start = new ParameterizedThreadStart(WalkAndRunThreds);
            Thread thread = new Thread(start);
            thread.Name = "Поток Главный правый";
            thread.Start(my_tree);
            thread.Join();
            bool all_completed = false;
            while (!all_completed)
            {
                int n = threads.Count;
                all_completed = true;
                for (int i = 0; i < n; i++)
                {
                    if (threads[i].ThreadState != ThreadState.Stopped)
                        all_completed = false;
                }
            }
            Console.WriteLine("Сумма: " + sum.ToString());
        }

        static void WalkAndRunThreds(object argument)
        // Функция для проходу по дереву и запуску новых потоков
        {
            string id = Thread.CurrentThread.Name;

            Tree node = (Tree)argument;
            sum += node.inf;
            Console.WriteLine(Convert.ToString(id) + " на узле с числом " + node.inf.ToString());

            if (node.left != null)
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(WalkAndRunThreds);
                Thread thread = new Thread(start);
                thread.Name = "Поток Второстепенный правый №" + thread_number.ToString();
                thread_number++;
                threads.Add(thread);
                thread.Start(node.left);
            }
            if (node.right != null)
            {
                WalkAndRunThreds(node.right);
            }
        }

        static Tree BuildAndGetTree(int n)
        // Функция создает почти сбалансированное дерево из чисел от 1 до n
        {
            Queue<int> numbers = new Queue<int>();
            SortNumbers(1, n, numbers);
            int inf = numbers.Dequeue();
            Tree main_node = new Tree(inf);
            foreach (int number in numbers)
            {
                Insert(main_node, new Tree(number));
            }
            return main_node;
        }

        static void SortNumbers(int l, int r, Queue<int> numbers)
        // Функция формирования очереди чисел, удобной для построения дерева
        {
            if (l == r)
            {
                numbers.Enqueue(l);
                return;
            }
            else if (l > r)
            {
                return;
            }
            else
            {
                int mid = (int)(l + r) / 2;
                numbers.Enqueue(mid);
                SortNumbers(l, mid - 1, numbers);
                SortNumbers(mid + 1, r, numbers);
            }

        }

        static void Insert(Tree main_node, Tree inserted_node)
        // Функция для вставки узла inserted_node в дерево main_node
        {
            if (inserted_node.inf < main_node.inf)
            {
                if (main_node.left == null)
                    main_node.left = inserted_node;
                else
                    Insert(main_node.left, inserted_node);
            }
            else
            {
                if (main_node.right == null)
                    main_node.right = inserted_node;
                else
                    Insert(main_node.right, inserted_node);
            }
        }
    }


    class Tree
    {
        public Tree left;
        public Tree right;
        public float inf;

        public Tree(float inf)
        {
            this.left = null;
            this.right = null;
            this.inf = inf;
        }
    }
}
