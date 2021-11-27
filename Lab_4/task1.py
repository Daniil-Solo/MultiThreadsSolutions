"""
Лабораторная 4 Задание 1

Напишите программу управления пулом печати. В системе, имеется пул печати объемом n байт,
поток управления пулом и m потоков, осуществляющих печать.
Поток, получивший доступ к пулу может записать в него либо весь текст, предназначенный для печати,
либо часть текста, если места в пуле недостаточно. Если места в пуле недостаточно, то поток должен
быть приостановлен до тех пор, пока поток управления пулом не освободит место в пуле.
Поток, начавший вывод информации в пул не может быть прерван другими потоками вывода текста до тех пор,
пока не осуществит вывод всего текста. Запись текста всегда осуществляется в конец пула.
Поток управления пулом может получить доступ к пулу только тогда, когда в пуле имеются заполненные ячейки.
Чтение из пула осуществляется из начала. При этом остальное содержимое пула сдвигается к началу – выведенный
на печать текст удаляется. Для имитации работы пула используйте потоки, читающие текстовые файлы,
и поток управления, выполняющий запись текста в файл.
"""
import random
import threading

n = 10      # объем пула печати
m = 3       # число потоков для печати
pull = ""   # пулл печати
count = 0   # количество отработавших потоков для печати

read_lock = threading.Lock()        # контроль чтения
stop_read_lock = threading.Lock()   # контроль остановки чтения
manage_lock = threading.Lock()      # контроль управления (записи)

alphabet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'


def read_file(filename):
    """
    Функция чтения из файла побуферно
    После считывания буфера дальнейшее считывание блокируется и разблокируется запись
    После окончания считывания отправляется на запись разделитель и чтение становится доступным следующему потоку
    """
    global pull, n, read_lock, manage_lock, stop_read_lock, count

    read_lock.acquire()
    print(threading.currentThread().getName() + " начал работу")
    with open(filename, 'r') as file:
        pull = file.read(n)
        while pull:
            print("Текущий пулл: " + pull)
            manage_lock.release()
            stop_read_lock.acquire()
            pull = file.read(n)
    pull = "\n--------\n"
    print(threading.currentThread().getName() + " закончил работу")
    count += 1
    manage_lock.release()
    stop_read_lock.acquire()
    read_lock.release()


def write_file(filename):
    """
    Функция записывает в файл значение буфера и дает разблокировку на продолжение считывания
    Завершается после отработки всех потоков для чтения
    """
    global pull, count, m
    with open(filename, 'w') as file:
        while count < m:
            manage_lock.acquire()
            file.write(pull)
            print("Печатает: " + pull)
            stop_read_lock.release()


def main():
    stop_read_lock.acquire()
    manage_lock.acquire()

    t_manager = threading.Thread(target=write_file, args=("output.txt", ))
    t_manager.start()

    t_printers = []
    for i in range(m):
        filename = f"text{i+1}.txt"
        with open(filename, 'w') as file:
            string = ''.join(random.choices(alphabet, k=15))
            file.write(string)
        t_printer = threading.Thread(target=read_file, args=(filename, ))
        t_printers.append(t_printer)
        t_printers[-1].start()


if __name__ == "__main__":
    main()
