"""
Лабораторная 4 Задание 2
В текстовом файле находится текст произвольной длины на английском языке. Необходимо выполнить
шифрование этого текста по следующему алгоритму:
  ● строчные буквы в тексте преобразуются в заглавные и наоборот;
  ● каждая буква заменяется на следующую по алфавиту, А -> В, Z -> A.
Зашифрованный текст должен быть помещен в новый текстовый файл.
Выборку текста из файла выполняет один поток, который считывает символы последовательно в буфер,
длиной m символов. Каждый новый символ записывается в первую свободную ячейку с конца буфера.
Шифрование текста осуществляют n потоков-шифровщиков, которые модифицируют буквы в буфере по указанным правилам.
Запись шифрованного текста выполняет третий поток, который вычитывает символы из буфера от начала к концу в файл
и освобождает ячейки для записи новых символов.
Также необходимо написать программу-дешифровщик зашифрованного текста, которая начинает работу только после
окончания работы программы-шифровщика.
"""
import threading

m = 10  # Размер буфера
n = 3  # Число потоков-шифровщиков
buffer = []  # Буфер
n_ended = 0
is_end = False

read_lock = threading.Lock()                         # контроль чтения
cypher_locks = [threading.Lock() for _ in range(n)]  # контроль шифрования
write_lock = threading.Lock()                        # контроль записи
print_lock = threading.Lock()                        # контроль печати в консоль

alphabet_upper = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
alphabet_lover = list(alphabet_upper.lower())  # список латинских букв в нижнем регистре
alphabet_upper = list(alphabet_upper)          # список латинских букв в верхнем регистре
cypher_dict = dict()                           # словарь для шифрования
decypher_dict = dict()                         # словарь для дешифрования
# заполнение словарей
for index, (up_letter, low_letter) in enumerate(zip(alphabet_upper, alphabet_lover)):
    try:
        cypher_dict[up_letter] = alphabet_upper[index+1]
        cypher_dict[low_letter] = alphabet_lover[index + 1]
    except IndexError:
        cypher_dict[up_letter] = alphabet_upper[0]
        cypher_dict[low_letter] = alphabet_lover[0]
    try:
        decypher_dict[up_letter] = alphabet_upper[index - 1]
        decypher_dict[low_letter] = alphabet_lover[index - 1]
    except IndexError:
        decypher_dict[up_letter] = alphabet_upper[-1]
        decypher_dict[low_letter] = alphabet_lover[-1]

# Константы для режима работы потока-преобразователя
ENCRYPTION = 1
DECRYPTION = 0


def read_file_with_buffer(filename):
    """
    Функция читает файл побуферно и вызывает потоки-преобразователи
    Когда файл заканчивается, завершает другие потоки
    """
    global m, buffer, read_lock, cypher_locks, write_lock, is_end
    with open(filename, 'r') as file:
        buffer = file.read(m)
        read_lock.acquire()
        while buffer:
            # работаем с буфером
            print("Считанный буффер: " + buffer)
            buffer = list(buffer)
            print("Разблокировка шифрующих потоков")
            for cypher_lock in cypher_locks:
                cypher_lock.release()
            # ждем разрешения продолжения работы
            read_lock.acquire()
            # обновляем буфер
            buffer = file.read(m)
    print("Все данные были считаны!")
    is_end = True
    for cypher_lock in cypher_locks:
        cypher_lock.release()
    write_lock.release()


def cypher_buffer(i, mode):
    """
    Поток шифрует или дешифрует в зависимости от режима и своего номера
    Когда преобразовал, вызывает функцию проверки, что все потоки-преобразователи закончили с текущим буфером
    """
    global buffer, m, n, cypher_locks, print_lock
    index = i
    while True:
        cypher_locks[i].acquire()
        if is_end:
            break
        print_lock.acquire()
        print("Поток " + threading.currentThread().getName() + " начал преобразование")
        print_lock.release()
        while index < len(buffer):
            old_letter = buffer[index]
            new_letter = transform_symbol(old_letter, mode)
            buffer[index] = new_letter
            print_lock.acquire()
            print(str(old_letter) + " -> " + new_letter)
            print_lock.release()
            index += n
        index = i
        check_end_of_cyphering()


def transform_symbol(symbol: str, mode: int) -> str:
    """
    Функция возвращает преобразованный символ в зависимости от режима преобразования
    """
    global cypher_dict, decypher_dict
    new_symbol = symbol.upper() if symbol.islower() else symbol.lower()
    if mode == ENCRYPTION:
        new_symbol = cypher_dict[new_symbol]
    elif mode == DECRYPTION:
        new_symbol = decypher_dict[new_symbol]
    else:
        pass
    return new_symbol


def check_end_of_cyphering():
    """
    Функция увеличивает значение счетчика закончивших с текущим буфером потоков
    После того, как отработали все потоки, запускается поток на запись преобразованного буфера в файл
    """
    global n_ended, write_lock
    n_ended += 1
    if n_ended == n:
        print("Разблокировка на запись")
        write_lock.release()
        n_ended = 0


def write_file_with_buffer(filename):
    """
    Функция записывает буфер в файл и дает разблокировку считывающему потоку
    """
    global buffer, write_lock, read_lock
    with open(filename, 'w') as file:
        while True:
            write_lock.acquire()
            if is_end:
                break
            print("Записанный буфер: " + ''.join(buffer))
            file.write(''.join(buffer))
            print("Разблокировка на чтение")
            read_lock.release()


def main():
    global read_lock, cypher_locks, write_lock, is_end

    # Шифрование
    for cypher_lock in cypher_locks:
        cypher_lock.acquire()
    write_lock.acquire()

    t_reader = threading.Thread(target=read_file_with_buffer, args=("text.txt", ))
    t_reader.start()
    t_writer = threading.Thread(target=write_file_with_buffer, args=("enc_text.txt", ))
    t_writer.start()
    t_cyphers = list()
    for i in range(n):
        t_cypher = threading.Thread(target=cypher_buffer, args=(i, ENCRYPTION, ))
        t_cyphers.append(t_cypher)
        t_cyphers[-1].start()

    t_reader.join()
    for t_cypher in t_cyphers:
        t_cypher.join()
    t_writer.join()

    print("\nЗапускаем дешифровку\n")

    # Дешифрование
    read_lock.release()
    is_end = False
    for cypher_lock in cypher_locks:
        cypher_lock.release()
        cypher_lock.acquire()
    write_lock.release()
    write_lock.acquire()

    t_reader = threading.Thread(target=read_file_with_buffer, args=("enc_text.txt", ))
    t_reader.start()
    t_writer = threading.Thread(target=write_file_with_buffer, args=("dec_text.txt", ))
    t_writer.start()
    for i in range(n):
        t_cypher = threading.Thread(target=cypher_buffer, args=(i, DECRYPTION, ))
        t_cypher.start()
    t_reader.join()
    for t_cypher in t_cyphers:
        t_cypher.join()
    t_writer.join()


if __name__ == "__main__":
    main()
