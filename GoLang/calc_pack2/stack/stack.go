// Пакет stack реализует методы работы со стеком
package stack

import (
	"errors"
	"golang.org/x/exp/constraints"
)

type Stack[S any] []S

// Pop извлекает элемент из стека
func (stack *Stack[S]) Pop() (any, error) {
	if len(*stack) == 0 {
		return nil, errors.New("stack is empty")
	}
	top := len(*stack) - 1
	topEl := (*stack)[top]
	*stack = (*stack)[:top]
	return topEl, nil
}

// Push кладёт элемент в стек
func (stack *Stack[S]) Push(el any) {
	elAs, _ := el.(S)
	*stack = append(*stack, elAs)
}

// Top получает верхний элемент стека, но не извлекает его
func (stack *Stack[S]) Top() (any, error) {
	if len(*stack) == 0 {
		return nil, errors.New("stack is empty")
	}
	top := len(*stack) - 1
	topEl := (*stack)[top]
	return topEl, nil
}

// Addition складывает два верхних числа из стека,
// результат помещается в стек
func Addition[T constraints.Integer](stack *Stack[T]) error {
	// получаем верхний элемент
	el1, err := stack.Pop()
	if err != nil {
		return err
	}
	num1, _ := el1.(int)

	// получаем верхний элемент
	el2, errN := stack.Pop()
	if errN != nil {
		return errN
	}
	num2, _ := el2.(int)

	// складываем числа и результат кладём в стек
	res := num2 + num1
	stack.Push(res)
	return nil
}

// Subtraction вычитает два верхних числа из стека,
// результат помещается в стек
func Subtraction[T constraints.Integer](stack *Stack[T]) error {
	// получаем верхний элемент
	el1, err := stack.Pop()
	if err != nil {
		return err
	}
	num1, _ := el1.(int)

	// получаем верхний элемент
	el2, errN := stack.Pop()
	if errN != nil {
		return errN
	}
	num2, _ := el2.(int)

	// вычитаем числа и результат кладём в стек
	res := num2 - num1
	stack.Push(res)
	return nil
}

// Multiplication умножает два верхних числа из стека,
// результат помещается в стек
func Multiplication[T constraints.Integer](stack *Stack[T]) error {
	// получаем верхний элемент
	el1, err := stack.Pop()
	if err != nil {
		return err
	}
	num1, _ := el1.(int)

	// получаем верхний элемент
	el2, errN := stack.Pop()
	if errN != nil {
		return errN
	}
	num2, _ := el2.(int)

	// умножаем числа и результат кладём в стек
	res := num2 * num1
	stack.Push(res)
	return nil
}

// Division делит два верхних числа из стека,
// результат помещается в стек
func Division[T constraints.Integer](stack *Stack[T]) error {
	// получаем верхний элемент
	el1, err := stack.Pop()
	if err != nil {
		return err
	}
	num1, _ := el1.(int)

	// получаем верхний элемент
	el2, errN := stack.Pop()
	if errN != nil {
		return errN
	}
	num2, _ := el2.(int)

	// делим числа и результат кладём в стек
	res := num2 / num1
	stack.Push(res)
	return nil
}
