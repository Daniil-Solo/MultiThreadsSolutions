"""
Лабораторная 5 Задние 2
Смоделировать механизм рандеву для решения задачи нахождения значения функции e^x по ее разложению в ряд Маклорена.
В качестве вызывающей задачи взять функцию нахождения суммы элементов ряда, а в качестве обслуживающих задач –
функцию нахождения факториала и функцию нахождения степени аргумента.
"""
import threading

n_factorial = 0  # хранит значение n!
x_power_n = 0    # хранит значение x^n

factorial_lock = threading.Lock()  # контроль факториальной функции
x_power_n_lock = threading.Lock()  # контроль степенной функции


def calculate_x_power_n(x, n):
    """
    Функция возводит x в степень n и сохраняет результат в x_power_n
    После завершения снимает блокировку с ожидания значения x в степени n
    """
    global x_power_n
    result = 1
    for i in range(n):
        result *= x
    x_power_n = result
    x_power_n_lock.release()


def calculate_factorial(n):
    """
    Функция вычисляет n-факториал и сохраняет результат в n_factorial
    После завершения снимает блокировку с ожидания значения n-факториал
    """
    global n_factorial
    result = 1
    for i in range(n):
        result *= (i+1)
    n_factorial = result
    factorial_lock.release()


def sum_element(x, n):
    """
    Функция суммирует значения элементов ряда Маклорена для e^x
    """
    global x_power_n, n_factorial
    summa = 1
    print(summa)
    for i in range(1, n+1):
        t_x_pow_n = threading.Thread(target=calculate_x_power_n, args=(x, i, ))
        t_x_pow_n.start()
        t_factorial = threading.Thread(target=calculate_factorial, args=(i, ))
        t_factorial.start()

        x_power_n_lock.acquire()
        factorial_lock.acquire()
        summa += x_power_n / n_factorial
        print(summa)


def main():
    x_power_n_lock.acquire()
    factorial_lock.acquire()
    sum_element(1, 10)


if __name__ == "__main__":
    main()
