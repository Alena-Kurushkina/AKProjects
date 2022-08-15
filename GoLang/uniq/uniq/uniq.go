// Пакет uniq реализует утилиту, с помощью которой можно вывести или отфильтровать повторяющиеся строки в файле.
// Повторяющиеся входные строки не распозноваются как повторяющиеся, если они не следуют строго друг за другом.
// Пакет также поддерживает ряд опций
package uniq

import (
	"errors"
	"strconv"
	"strings"
)

type Options struct {
	CountRepeatingLines  bool // подсчитать количество употреблений строки во входных данных
	GetRepeatingLines    bool // вывести только те строки, которые повторились во входных данных
	GetNonRepeatingLines bool // вывести только те строки, которые не повторились во входных данных
	IgnoreCase           bool // не учитывать регистр букв
	IgnoreFieldsNumber   int  // не учитывать указанное кол-во полей в строке
	IgnoreCharsNumber    int  // не учитывать указанное кол-во символов в строке
}

type LinesCount struct {
	line     string // строка для сравнения
	count    int    // кол-во употреблений строки
	origLine string // оригинальная строка
}

const Usage = "Proper usage: uniq [-c | -d | -u] [-i] [-f num] [-s chars] [input_file [output_file]]"

// compare сравнивает строки с учётом выбора опции Игнорировать регистр
func compare(str1 string, str2 string, ignoreCase bool) int {
	if ignoreCase {
		var res int
		if !strings.EqualFold(str1, str2) {
			res = 1
		}
		return res
	}
	return strings.Compare(str1, str2)
}

// trimFields обрезает указанное кол-во полей в строке
func trimFields(text []string, ignoreNumb int) []string {
	res := make([]string, len(text))
	for k, line := range text {
		fields := strings.Fields(line)
		for i := 0; i < ignoreNumb && i < len(fields); i++ {
			_, line, _ = strings.Cut(line, fields[i]) // oбрезаем в строке поле fields[i] и всё что до него
		}
		res[k] = line
	}
	return res
}

// trimChars обрезает указанное кол-во символов в строке
func trimChars(text []string, ignoreNumb int) []string {
	res := make([]string, len(text))
	for k, line := range text {
		if len(line) != 0 {
			trimCount := ignoreNumb
			if line[0] == 32 { // если первый символ - пробел
				trimCount++
			}
			if trimCount <= len(line) {
				res[k] = line[trimCount:]
			} else {
				res[k] = ""
			}
		}
	}
	return res
}

// getRepeatingLines возвращает только повторяющиеся строки
func getRepeatingLines(lc []LinesCount) string {
	result := []string{}
	for _, line := range lc {
		if line.count > 1 {
			result = append(result, line.origLine)
		}
	}
	return strings.Join(result, "\n")
}

// getNonRepeatingLines возвращает строки, встретившиеся 1 раз
func getNonRepeatingLines(lc []LinesCount) string {
	result := []string{}
	for _, line := range lc {
		if line.count == 1 {
			result = append(result, line.origLine)
		}
	}
	return strings.Join(result, "\n")
}

// formLinesCount возвращает кол-ва употреблений и сами строки через пробел
func formLinesCount(lc []LinesCount) string {
	res := ""
	for i, elem := range lc {
		res = res + strconv.Itoa(elem.count) + " " + elem.origLine
		if i != len(lc)-1 {
			res = res + "\n"
		}
	}
	return res
}

// Uniq подсчитывает кол-ва употреблений строк с учётом указанных опций
func Uniq(origText []string, opt Options) (string, error) {
	text := make([]string, len(origText))
	copy(text, origText)

	// игнорируем указанное кол-во полей в строке
	if opt.IgnoreFieldsNumber != 0 {
		text = trimFields(text, opt.IgnoreFieldsNumber)
	}

	// игнорируем указанное кол-во символов в каждой строке
	if opt.IgnoreCharsNumber != 0 {
		text = trimChars(text, opt.IgnoreCharsNumber)
	}

	// подсчёт кол-ва повторений строк
	linesCount := []LinesCount{}
	i := 0 // итератор по строкам
	for i < len(text) {
		j := i + 1 // индекс строки для сравнения
		count := 1 // счётчик повторений
		for j < len(text) && compare(text[i], text[j], opt.IgnoreCase) == 0 {
			count++
			j++
		}
		linesCount = append(linesCount, LinesCount{text[i], count, origText[i]})
		i = j
	}

	// возвращаем строки, которые встречались > 1 раза
	if opt.GetRepeatingLines {
		if opt.CountRepeatingLines || opt.GetNonRepeatingLines {
			return "", errors.New(Usage)
		}
		res := getRepeatingLines(linesCount)
		return res, nil
	}

	// возвращаем строки, которые встретились 1 раз
	if opt.GetNonRepeatingLines {
		if opt.GetRepeatingLines || opt.CountRepeatingLines {
			return "", errors.New(Usage)
		}
		res := getNonRepeatingLines(linesCount)
		return res, nil
	}

	// возвращаем строки и их кол-ва употреблений
	if opt.CountRepeatingLines {
		if opt.GetRepeatingLines || opt.GetNonRepeatingLines {
			return "", errors.New(Usage)
		}
		res := formLinesCount(linesCount)
		return res, nil
	}

	// вывод уникальных строк
	res := ""
	for i, elem := range linesCount {
		res = res + elem.origLine
		if i != len(linesCount)-1 {
			res = res + "\n"
		}
	}
	return res, nil
}
