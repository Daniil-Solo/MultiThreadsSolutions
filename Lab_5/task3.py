"""
Лабораторная 5 Задание 3
Даны n файлов, содержащие неубывающие последовательности чисел. Написать программу, выполняющую слияние этих файлов
в один таким образом, чтобы выборку данных из файлов и формирование результирующего файла выполняли три разных потока.
Каждый из потоков выборки данных может прочитать очередное значение из своего файла только тогда, когда поток
формирования результата передал соответствующее число в файл. Поток формирования результата может выполнять обработку
данных только тогда, когда оба потока выборки предоставили ему данные.
"""
import random
import threading

value_locks = [threading.Lock(), threading.Lock()]       # контроли считывания значений
manipulate_locks = [threading.Lock(), threading.Lock()]  # контроль готовности значений
values = [0, 0]                                          # значения


def get_value(filename, i):
    """
    Функция считывает значения из файла и снимает блокировку с готовности своего значения
    После завершения в значение записывается NaN
    """
    global values, value_locks, manipulate_locks
    with open(filename, 'r') as file:
        value = file.readline().strip()
        while value:
            values[i] = int(value)
            print(threading.currentThread().getName() + " считал значение " + str(values[i]))
            manipulate_locks[i].release()
            value_locks[i].acquire()
            value = file.readline()
        print(threading.currentThread().getName() + " завершил считывание")
        values[i] = "NaN"
        manipulate_locks[i].release()


def manipulate(filename):
    """
    Функция ожидает значения и сравнивает их, записывая наименьший результат в файл
    После записи снимает блокировку с получения значения для потока с наименьшим текущим значением
    При получении NaN снимает блокировку с потока, в котором было число
    При двух NaN завершает обработку
    """
    global values, value_locks
    with open(filename, 'w') as file:
        while True:
            manipulate_locks[0].acquire()
            manipulate_locks[1].acquire()
            print(f"Значения: {values[0]} и {values[1]}")
            if values[0] == "NaN" and values[1] == "NaN":
                break
            elif values[0] == "NaN":
                file.write(str(values[1]) + "\n")
                value_locks[1].release()
                manipulate_locks[0].release()
                print(f"Записывается в файл: {values[1]}")
            elif values[1] == "NaN":
                file.write(str(values[0]) + "\n")
                value_locks[0].release()
                manipulate_locks[1].release()
                print(f"Записывается в файл: {values[0]}")
            elif values[0] <= values[1]:
                file.write(str(values[0]) + "\n")
                value_locks[0].release()
                manipulate_locks[1].release()
                print(f"Записывается в файл: {values[0]}")
            else:
                file.write(str(values[1]) + "\n")
                value_locks[1].release()
                manipulate_locks[0].release()
                print(f"Записывается в файл: {values[1]}")
    print("Обработка завершена")


def create_non_decreasing_sequence(n):
    """
    Функция возвращает неубывающую последовательность из n элементов
    """
    my_list = random.choices(list(range(100)), k=n)
    return sorted(my_list)


def save_sequence(sequence, filename):
    """
    Функция сохраняет последовательность sequence в файл filename
    """
    with open(filename, 'w') as file:
        for item in sequence:
            file.write(str(item) + "\n")


def main():
    for i in range(1, 3):
        seq = create_non_decreasing_sequence(10)
        save_sequence(seq, f"seq{i}.txt")

    for i in range(2):
        manipulate_locks[i].acquire()
        value_locks[i].acquire()
        t_reader = threading.Thread(target=get_value, args=(f"seq{i+1}.txt", i))
        t_reader.start()

    t_writer = threading.Thread(target=manipulate, args=("output.txt", ))
    t_writer.start()
    t_writer.join()


if __name__ == "__main__":
    main()
