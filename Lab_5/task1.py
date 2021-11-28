"""
Лабораторная 5 Задание 1
Написать программу трёхленточной сортировки линейного списка, распараллелив процессы разбора
и слияния списков. Количество потоков и способы их синхронизации определить самостоятельно.
"""
import random
import threading

print_lock = threading.Lock()  # контроль печати в консоль


def merge_sort_without_thread(a):
    """
    Сортировка слиянием без потоков
    """
    print(a)
    if len(a) == 1 or len(a) == 0:
        return a
    l, r = a[:len(a) // 2], a[len(a) // 2:]
    merge_sort_without_thread(l)
    merge_sort_without_thread(r)
    n = m = k = 0
    c = [0] * (len(l) + len(r))
    while n < len(l) and m < len(r):
        if l[n] <= r[m]:
            c[k] = l[n]
            n += 1
        else:
            c[k] = r[m]
            m += 1
        k += 1
    while n < len(l):
        c[k] = l[n]
        n += 1
        k += 1
    while m < len(r):
        c[k] = r[m]
        m += 1
        k += 1
    for i in range(len(a)):
        a[i] = c[i]
    return a


def merge_sort(a):
    """
    Сортировка слиянием с потоками
    """
    global print_lock
    print_lock.acquire()
    print(a)
    print_lock.release()

    if len(a) == 1 or len(a) == 0:
        return a
    l, r = a[:len(a) // 2], a[len(a) // 2:]
    t_left = threading.Thread(target=merge_sort, args=(l, ))
    t_left.start()
    t_right = threading.Thread(target=merge_sort, args=(r, ))
    t_right.start()
    t_left.join()
    t_right.join()
    n = m = k = 0
    c = [0] * (len(l) + len(r))
    while n < len(l) and m < len(r):
        if l[n] <= r[m]:
            c[k] = l[n]
            n += 1
        else:
            c[k] = r[m]
            m += 1
        k += 1
    while n < len(l):
        c[k] = l[n]
        n += 1
        k += 1
    while m < len(r):
        c[k] = r[m]
        m += 1
        k += 1
    for i in range(len(a)):
        a[i] = c[i]
    return a


def main():
    my_list = random.choices(list(range(100)), k=20)
    print("Исходный массив: " + str(my_list))
    merge_sort_without_thread(my_list)
    print()
    my_list = merge_sort(my_list)
    print("Выходной массив: " + str(my_list))


if __name__ == "__main__":
    main()
