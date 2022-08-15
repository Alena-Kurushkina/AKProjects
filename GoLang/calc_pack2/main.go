package main

import (
	"bufio"
	"calc_pack2/stack"
	"errors"
	"fmt"
	"os"
	"regexp"
	"strconv"
)

// StrToSlice преобразует строку выражения в слайс
func StrToSlice(str string) []string {
	expression := []string{}
	re, _ := regexp.Compile(`\d+`)         // регулярное выражение для поиска чисел в строке
	inds := re.FindAllStringIndex(str, -1) // массив индексов начала и конца вхождения чисел в строку

	if inds == nil {
		return nil
	}
	i := 0                    // итератор по строке
	for _, el := range inds { // итератор по массиву индексов
		indStart := el[0]  // индекс начала вхождения числа в строку
		indEnd := el[1]    // индекс конца вхождения числа в строку
		if indStart == i { // если iый символ в строке является частью числа
			expression = append(expression, str[indStart:indEnd])
		} else { // иначе это символ(операнд) или набор символов, напр. "(("
			processSimbols(&expression, str[i:indStart])
			expression = append(expression, str[indStart:indEnd])
		}
		i = indEnd
	}
	// добавим в слайс символы после последнего числа
	if i < len(str)-1 {
		processSimbols(&expression, str[i:])
	}
	return expression
}

// processSimbols добавляет символы строки в заданный слайс
func processSimbols(expression *[]string, str string) {
	for _, simb := range str {
		if string(simb) != " " {
			*expression = append(*expression, string(simb))
		}
	}
}

func Calc(elems []string) (int, error) {
	var chStack stack.Stack[string] // стек для операндов
	var numStack stack.Stack[int]   // стек для чисел

	// проходимся по элементам выражения
	for _, in := range elems {
		switch in {
		case "+":
			if len(chStack) != 0 {
				// получаем верхний операнд в стеке
				ch, err := chStack.Top()
				if err != nil {
					return 0, err
				}
				// если операнд выше по приоритету, то сначала выполняем соотв. операцию
				switch ch {
				case "/":
					stack.Division(&numStack)
					chStack.Pop()
				case "*":
					stack.Multiplication(&numStack)
					chStack.Pop()
				}
			}
			// кладём в стек новый операнд
			chStack.Push("+")
		case "-":
			if len(chStack) != 0 {
				ch, err := chStack.Top()
				if err != nil {
					return 0, err
				}
				switch ch {
				case "/":
					stack.Division(&numStack)
					chStack.Pop()
				case "*":
					stack.Multiplication(&numStack)
					chStack.Pop()
				}
			}
			chStack.Push("-")
		case "*":
			if len(chStack) != 0 {
				// получаем верхний операнд в стеке
				ch, err := chStack.Top()
				if err != nil {
					return 0, err
				}
				// сохраняем заданный в выражении порядок операций
				if ch == "/" {
					stack.Division(&numStack)
					chStack.Pop()
				}
			}
			// кладём в стек новый операнд
			chStack.Push("*")
		case "/":
			if len(chStack) != 0 {
				ch, err := chStack.Top()
				if err != nil {
					return 0, err
				}
				if ch == "*" {
					stack.Multiplication(&numStack)
					chStack.Pop()
				}
			}
			chStack.Push("/")
		case "(":
			chStack.Push("(")
		case ")":
		L:
			// выполняем все операции из стека пока не дойдём до "("
			for ch, err := chStack.Pop(); ; {
				switch ch {
				case "(":
					break L
				case "+":
					stack.Addition(&numStack)
				case "-":
					stack.Subtraction(&numStack)
				case "/":
					stack.Division(&numStack)
				case "*":
					stack.Multiplication(&numStack)
				default:
					if err != nil {
						return 0, errors.New("incorrect usage of (.")
					}
				}
				ch, err = chStack.Pop()
			}
		default:
			// пытаемся преобразовать в число
			// в случае успеха кладём число в стек
			num, errConv := strconv.Atoi(in)
			if errConv != nil {
				return 0, errors.New("unknown elements in expression")
			}
			numStack.Push(num)
		}
	}

	if _, err := chStack.Top(); err != nil { // если стек знаков пуст
		if _, err1 := numStack.Top(); err1 != nil { // и стек чисел пуст
			return 0, err
		}
	} else { // если стек знаков не пуст, выполняем оставшиеся операции
		for ch, err := chStack.Pop(); err == nil; {
			var opErr error
			switch ch {
			case "+":
				opErr = stack.Addition(&numStack)
			case "-":
				opErr = stack.Subtraction(&numStack)
			case "/":
				opErr = stack.Division(&numStack)
			case "*":
				opErr = stack.Multiplication(&numStack)
			}
			if opErr != nil {
				return 0, opErr
			}
			ch, err = chStack.Pop()
		}
	}
	// возвращаем число
	el, _ := numStack.Pop()
	res, _ := el.(int)
	return res, nil
}

func main() {
	sc := bufio.NewScanner(os.Stdin)
	sc.Scan()
	input := sc.Text()

	elems := StrToSlice(input)
	res, err := Calc(elems)
	if err != nil {
		fmt.Println(err)
	}
	fmt.Println(res)
}
