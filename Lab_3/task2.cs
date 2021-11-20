using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Lab3_Task2_
{
    public partial class Form1 : Form
    {
        static Mutex mutexObj = new Mutex();
        public Form1()
        {
            InitializeComponent();
        }

        public void Write(object argument)
        // Функция заносит в файл текст из текстбокса писателя
        {
            App1_status.Invoke(new Action(() => { App1_status.Text = "Ждет разблокировки мьютекса"; }));

            mutexObj.WaitOne();

            App1_status.Invoke(new Action(() => { App1_status.Text = "Зашел в мьютекс"; }));
            String text = (String)argument; 
            String[] string_lines = text.Split('\n');
            StreamWriter f = new StreamWriter("text.txt");
            for (int i = 0; i < string_lines.Length; i++)
            {
                f.WriteLine(string_lines[i]);
                Thread.Sleep(1500);
            }
            f.Close();

            mutexObj.ReleaseMutex();

            App1_status.Invoke(new Action(() => { App1_status.Text = "Вышел из мьютекса"; }));
        }

        public void Read(object argument)
        // Функция печатает текст из файла в текстбокс читателя
        {
            while(true)
            {
                App2_status.Invoke(new Action(() => { App2_status.Text = "Ждет разблокировки мьютекса"; }));

                mutexObj.WaitOne();

                App2_status.Invoke(new Action(() => { App2_status.Text = "Зашел в мьютекс"; }));
                String text = "";
                StreamReader f = new StreamReader("text.txt");
                while (!f.EndOfStream)
                {
                    text += f.ReadLine() + '\n' + '\r';
                    App2_text.Invoke(new Action(() => { App2_text.Text = text; }));
                    Thread.Sleep(500);
                }
                f.Close();

                mutexObj.ReleaseMutex();

                App2_status.Invoke(new Action(() => { App2_status.Text = "Вышел из мьютекса"; }));
            }
        }

        private void App1_button_Click(object sender, EventArgs e)
        // Запуск потока писателя
        {
            ParameterizedThreadStart start = new ParameterizedThreadStart(Write);
            Thread thread = new Thread(start);
            String text = App1_text.Text;
            thread.Start(text);
        }

        private void App2_button_Click(object sender, EventArgs e)
        // Запуск потока читателя
        {
            ParameterizedThreadStart start = new ParameterizedThreadStart(Read);
            Thread thread = new Thread(start);
            String text = App1_text.Text;
            thread.Start(text);
        }
    }

}
