import re
import random
import time
import tkinter


class Task:
    def __init__(self, function, number):
        self.lbl_status = None
        self.ent_input = None
        self.txt_output = None
        self.input_lock = None
        self.function = function
        self.number = number

    def run(self):
        self.lbl_status["text"] = f"Статус: задача {self.number} ждет входные данные"
        self.input_lock.acquire()
        data = self.ent_input.get().split(' ')
        self.lbl_status["text"] = f"Статус: задача {self.number} выполняется"
        try:
            result = self.function(data)
        except Exception as ex:
            self.lbl_status["text"] = f"Статус: задача {self.number} аварийно завершена"
            self.txt_output.delete(1.0, tkinter.END)
            self.txt_output.insert(1.0, ex)
            return
        self.lbl_status["text"] = f"Статус: задача {self.number} успешно завершена"
        self.txt_output.delete(1.0, tkinter.END)
        self.txt_output.insert(1.0, result)

    def set_elements(self, lbl_status, ent_input, txt_output, input_lock):
        self.lbl_status = lbl_status
        self.ent_input = ent_input
        self.txt_output = txt_output
        self.input_lock = input_lock


def multiplication_2_numbers(*args):
    """
    перемножить 2 больших числа (числа спросить у пользователя)
    В питоне перемножение больших чисел уже реализовано
    """
    number1 = int(args[0][0])
    number2 = int(args[0][1])
    return f"{number1 * number2}"


def remove_duplicate(*args):
    """
    удалить из массива вещественных чисел дубликаты (массив дан в файле), сохранить в другой файл
    """
    old_filename = "old_text.txt"
    new_filename = "new_text.txt"
    with open(old_filename, 'r') as read_file:
        string = read_file.readline()
        numbers = [number.strip() for number in string.split(' ')]
        result_numbers = list(set(numbers))
    with open(new_filename, 'w') as write_file:
        string = ' '.join(result_numbers)
        write_file.write(string)
    return f"Результат в файле {new_filename}"


def find_most_popular_word(*args):
    """
    вывести самое часто встречающееся слово в тексте (файл дан) и количество вхождений
    """
    filename = "big_text.txt"
    with open(filename, 'r') as file:
        lines = file.readlines()
    text = ' '.join(lines)
    all_words = re.findall("[A-Za-z]+", text)
    del text
    word_count_dict = dict()
    for word in all_words:
        if word in word_count_dict:
            word_count_dict[word] += 1
        else:
            word_count_dict[word] = 1
    word_count_list = [(key, word_count_dict[key]) for key in word_count_dict]
    most_popular_word = sorted(word_count_list, key=lambda x: x[1], reverse=True)[0]
    return f"{most_popular_word}"


def find_element_in_matrix(*args):
    """
    вывести минимальный элемент среди элементов, расположенных ниже главной диагонали, и максимальный элемент,
    среди элементов расположенных выше побочной диагонали квадратной матрицы размером nxn (размер вводится
    пользователем, значения генерируются автоматически);
    """
    n = int(args[0][0])
    matrix = []
    min_element = 100
    max_element = -1
    for i in range(n):
        matrix.append([])
        for j in range(n):
            matrix[i].append(random.randint(0, 100))

            # ниже главной диаогнали
            if i > j:
                if min_element > matrix[i][j]:
                    min_element = matrix[i][j]

            # выше побочной диагонали
            if i < n - j - 1:
                if max_element < matrix[i][j]:
                    max_element = matrix[i][j]
    return f"Минимальный элемент: {min_element}, максимальный элемент: {max_element}"


def simple_number(*args):
    """
    вывести все простые числа от 0 до n (вводится пользователем), используя алгоритм «Решето Эратосфена»;
    """
    n = int(args[0][0])
    result_list = []
    if n == 0 or n == 1:
        return "Нет таких чисел"
    sieve = set(range(2, n + 1))
    while sieve:
        prime = min(sieve)
        result_list.append(prime)
        sieve -= set(range(prime, n + 1, prime))
    return f"{result_list}"
