// Лабораторная 1. Задание 4
// Программа выводит на форму круги трёх разных цветов в случайных местах (три разных потока).
// Необходимо предусмотреть возможность изменить приоритет потоков управляющими элементами на форме.

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Lab1_Task4
{
    public partial class Form1 : Form
    {
        Graphics graphics;
        int radius = 50;
        object locker = new object();

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 1;
            comboBox3.SelectedIndex = 2;
            button1.Focus();
            graphics = pictureBox1.CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((comboBox1.Text == comboBox2.Text) || (comboBox1.Text == comboBox3.Text) || (comboBox3.Text == comboBox2.Text))
            {
                graphics.Clear(Color.White);
                return;
            }
            else
            {
                int w = pictureBox1.Size.Width;
                int h = pictureBox1.Size.Height;
                graphics.Clear(Color.White);

                // Создаем потоки 
                Thread[] threads = new Thread[3]; 
                for (int i = 0; i < 3; i++)
                {
                    ParameterizedThreadStart start = new ParameterizedThreadStart(CreateCircle);
                    Thread thread = new Thread(start);
                    threads[i] = thread;
                }

                // Выставляем приоритеты
                threads[0].Priority = ThreadPriority.Highest;
                threads[1].Priority = ThreadPriority.Normal;
                threads[2].Priority = ThreadPriority.Lowest;

                // Запускаем потоки
                threads[0].Start(new Argument(w, h, radius, GetColor(comboBox1.Text), graphics, locker));
                threads[1].Start(new Argument(w, h, radius, GetColor(comboBox2.Text), graphics, locker));
                threads[2].Start(new Argument(w, h, radius, GetColor(comboBox3.Text), graphics, locker));

                // Ждем завершения каждого потока
                for (int i = 0; i < 3; i++)
                {
                    threads[i].Join();
                }
            }
            
        }

        public static void CreateCircle(object argument)
        {
            Argument arg = (Argument)argument;

            Random random = new Random();
            int x0 = 50 + random.Next() % (arg.w-100);
            int y0 = 50 + random.Next() % (arg.h-100);
            for(int ang=10; ang<231; ang+=10)
            {
                lock (arg.locker)
                {
                    arg.graphics.DrawArc(new Pen(arg.color, 3f), x0, y0, arg.r, arg.r, (float)ang, (float)(ang + 10));
                    Thread.Sleep(50);
                }
            }

        }

        public static Color GetColor(string colorText)
        {
            if (colorText == "Красный")
                return Color.Red;
            else if (colorText == "Синий")
                return Color.Blue;
            else
                return Color.Green;
        }
    }
    class Argument
    {
        public int w;
        public int h;
        public int r;
        public Color color;
        public Graphics graphics;
        public object locker;

        public Argument(int w, int h, int r, Color color, Graphics graphics, object locker)
        {
            this.w = w;
            this.h = h;
            this.r = r;
            this.color = color;
            this.graphics = graphics;
            this.locker = locker;
        }
    }
}
