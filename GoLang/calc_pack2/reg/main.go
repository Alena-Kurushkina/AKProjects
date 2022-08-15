package main

import (
	"fmt"
	"regexp"
)

func main() {
	expression := []string{}
	re, _ := regexp.Compile(`\d+`)
	str := "12+3-4*(55/5+2)"
	res := re.FindAllStringIndex(str, -1) // выдаёт [[0 2] [3 4] [5 6] [8 10] [11 12] [13 14]]
	fmt.Println(res)

	i := 0 //итератор по строке
	for _, el := range res {
		if el[0] == i { //если iый символ в строке входит в число
			expression = append(expression, str[el[0]:el[1]])
		} else { //иначе это символ или набор символов, напр. "(("
			for _, el := range str[i:el[0]] { //полагаем что символ занимает 1 байт
				if string(el) != " " {
					expression = append(expression, string(el))
				}
			}
			expression = append(expression, str[el[0]:el[1]])
		}
		i = el[1]
	}
	last := len(res) - 1
	if res[last][1] != len(str) {
		for _, el := range str[res[last][1]:] {
			expression = append(expression, string(el))
		}
	}

	fmt.Println(expression)
}
