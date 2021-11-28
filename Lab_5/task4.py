"""
Лабораторная работа 5 Задание 4
Написать 2 программы. Первая программа - меню с вариантами действий второй программы (без вывода результата).
Вторая программа содержит в себе действия:
1 - перемножить 2 больших числа (числа спросить у пользователя);
2 -	удалить из массива вещественных чисел дубликаты (массив дан в файле), сохранить в другой файл;
3 -	вывести самое часто встречающееся слово в тексте (файл дан) и количество вхождений;
4 -	вывести минимальный элемент среди элементов, расположенных ниже главной диагонали, и максимальный элемент,
    среди элементов расположенных выше побочной диагонали квадратной матрицы размером nxn (размер вводится
    пользователем, значения генерируются автоматически);
5 -	вывести все простые числа от 0 до n (вводится пользователем), используя алгоритм «Решето Эратосфена»;
Примечание: обе программы должны обрабатывать случаи закрытия любой из них.
"""
import threading
import tkinter as tk
from functions import *


current_task = None                            # номер текущей задачи
task_lock = threading.Lock()                   # контроль выбора задачи
input_lock = threading.Lock()                  # контроль готовности входных данных
update_app2_status_lock = threading.Lock()     # контроль обновления статуса второй программы при получении новой задачи
exit_lock = threading.Lock()                   # контроль корректного закрытия обновляющего потока второй программы
destroy = 0                                    # индикатор удаленной программы


def main1():
    """
    Программа для отправления команд
    """
    global current_task, task_lock, update_app2_status_lock, destroy

    def sorry_they_closed_me():
        global destroy
        old_destroy = destroy
        destroy = 1
        if old_destroy == 0:
            update_app2_status_lock.release()
        window.destroy()

    def handle_for_btn(number):
        """
        Функция для обработки нажатия на кнопку с заданием
        """
        global current_task, task_lock, update_app2_status_lock, destroy

        def check_task(event):
            """
            Функция для проверки выполнения задачи
            """
            global destroy
            if destroy == 2:
                lbl_status["text"] = "За что вы так с ней?"
            elif "занят" in lbl_status["text"]:
                return
            else:
                t = threading.Thread(target=update_current_task)
                t.start()

        def update_current_task():
            """
            Функция для обновления состояния текущей задачи
            Инициирует вторую программу на проверку задачи
            Замораживается, пока задача не будет выполнена
            """
            global current_task, update_app2_status_lock
            lbl_status["text"] = f"Статус: занят задачей {number}"
            current_task = number
            update_app2_status_lock.release()
            task_lock.acquire()
            current_task = None
            lbl_status["text"] = "Статус: готов к работе"

        return check_task

    window = tk.Tk()
    window.title("Программа 1 - Менеджер")

    lbl_status = tk.Label(
        text="Статус: ",
        width=80,
        master=window,
    )
    lbl_status.pack()

    frm_app1 = tk.Frame(borderwidth=3, master=window)
    frm_app1.pack(ipadx=5, ipady=5)
    text_for_btn = [
        "1. Перемножить 2 больших числа (2 целых числа вводить через пробел)",
        "2. Удалить дубликаты из файла old_text.txt",
        "3. Вывести самое популярное слово из файла big_txt.txt",
        "4. Вывести минимальный элемент под главной диагональю и максимальный выше побочной диагонали\n"
        " матрицы n*n (1 целое число)",
        "5. Вывести простые числа от 2 до n (1 целое число)",
    ]
    for idx, text in enumerate(text_for_btn):
        btn = tk.Button(
            text=text,
            width=90,
            height=3,
            master=frm_app1,
        )
        btn.grid(row=idx, column=0, pady=3)
        btn.bind("<Button-1>", handle_for_btn(idx + 1))

    window.protocol('WM_DELETE_WINDOW', sorry_they_closed_me)
    window.mainloop()


def main2():
    """
    Программа для исполнения команд
    """
    global current_task, task_lock, input_lock, update_app2_status_lock, destroy, exit_lock

    def sorry_they_closed_me():
        global destroy
        old_destroy = destroy
        destroy = 2
        if old_destroy == 0:
            update_app2_status_lock.release()
        exit_lock.acquire()
        window.destroy()

    def send_input_data(event):
        """
        Функция для разблокировки выполнения задачи
        Сообщает о том, что данные введены
        """
        global input_lock
        if current_task is not None:
            input_lock.release()

    def check_new_task():
        """
        Функция проверки задачи
        Если задача пришла и программа была иницирована, то начинается выполненеи задачи
        """
        global current_task, input_lock, update_app2_status_lock, exit_lock
        while True:
            update_app2_status_lock.acquire()
            if destroy == 1:
                lbl_status["text"] = "За что вы так с ней?"
                break
            elif destroy == 2:
                break
            elif current_task is None:
                lbl_status["text"] = "Статус: не занят"
            else:
                lbl_status["text"] = f"Статус: пришла задача {current_task}"
                time.sleep(1)
                match_task(current_task)
        exit_lock.release()

    def match_task(number):
        """
        Функция распределения задач и запуска её выполнения
        """
        global task_lock, input_lock
        task = None
        if number == 1:
            task = Task(multiplication_2_numbers, number)
        elif number == 2:
            task = Task(remove_duplicate, number)
        elif number == 3:
            task = Task(find_most_popular_word, number)
        elif number == 4:
            task = Task(find_element_in_matrix, number)
        elif number == 5:
            task = Task(simple_number, number)
        task.set_elements(lbl_status, ent_input, txt_output, input_lock)
        task.run()
        time.sleep(1)
        task_lock.release()

    window = tk.Tk()
    window.title("Программа 2 - Исполнитель")

    frm_app2 = tk.Frame(borderwidth=3, master=window)
    frm_app2.pack(ipadx=5, ipady=5)

    lbl_status = tk.Label(text="Статус: ", width=80, master=frm_app2)
    lbl_input = tk.Label(master=frm_app2, text="Общий блок для входных данных", width=90)
    ent_input = tk.Entry(master=frm_app2, width=90)
    btn_ready = tk.Button(master=frm_app2, width=50, text="Начать выполнение")
    btn_ready.bind("<Button-1>", send_input_data)
    lbl_output = tk.Label(master=frm_app2, text="Общий блок для выходных данных", width=90)
    txt_output = tk.Text(master=frm_app2, width=70, height=5)

    for idx, element in enumerate([lbl_status, lbl_input, ent_input, btn_ready, lbl_output, txt_output]):
        element.grid(row=idx, column=0, pady=3)

    t_checker = threading.Thread(target=check_new_task)
    t_checker.start()
    window.protocol('WM_DELETE_WINDOW', sorry_they_closed_me)
    window.mainloop()


if __name__ == "__main__":
    task_lock.acquire()
    input_lock.acquire()
    exit_lock.acquire()
    t1 = threading.Thread(target=main1)
    t1.start()
    time.sleep(0.5)
    t2 = threading.Thread(target=main2)
    t2.start()
