package main

import (
	"errors"
	"reflect"
	"strconv"
	"testing"
)

var pcases = []struct {
	in  string
	out []string
}{
	{"((6/3+8)*4)-1", []string{"(", "(", "6", "/", "3", "+", "8", ")", "*", "4", ")", "-", "1"}},
	{"-1-2", []string{"-", "1", "-", "2"}},
	{"12+ 145", []string{"12", "+", "145"}},
	{"12abc 14576454356", []string{"12", "a", "b", "c", "14576454356"}},
	{"", nil},
	{"+-fj", nil},
}

func TestParsing(t *testing.T) {
	for k, tt := range pcases {
		t.Run("test_"+strconv.Itoa(k), func(t *testing.T) {
			s := StrToSlice(tt.in)
			if !reflect.DeepEqual(s, tt.out) {
				t.Errorf("got %q, want %q", s, tt.out)
			}
		})
	}
}

var cases = []struct {
	in  []string
	out int
}{
	{[]string{"(", "(", "6", "/", "3", "+", "8", "*", "4", ")", "-", "4", ")", "/", "5"}, 6},
	{[]string{"10", "/", "2"}, 5},
	{[]string{"10", "*", "2"}, 20},
	{[]string{"10", "+", "2"}, 12},
	{[]string{"10", "-", "2"}, 8},
	{[]string{"10", "/", "2", "*", "4"}, 20},
	{[]string{"10", "+", "2", "*", "4"}, 18},
	{[]string{"10", "-", "4", "/", "2"}, 8},
	{[]string{"(", "(", "2", "+", "3", ")", ")"}, 5},
	{[]string{"3", "*", "(", "2", "+", "3", ")"}, 15},
	{[]string{"(", "2", "+", "4", ")", "/", "2"}, 3},
	{[]string{"(", "2", "+", "4", ")", "/", "(", "1", "+", "2", ")"}, 2},
	{[]string{"(", "2", "+", "4", ")", "*", "(", "1", "+", "2", ")"}, 18},
	{[]string{"(", "2", "+", "4", ")", "-", "(", "1", "+", "2", ")"}, 3},
}

func TestCalc(t *testing.T) {
	for k, tt := range cases {
		t.Run("test_"+strconv.Itoa(k), func(t *testing.T) {
			s, err := Calc(tt.in)
			if s != tt.out {
				t.Errorf("got %q, want %q", s, tt.out)
			}
			if err != nil {
				t.Errorf("got error: %q", err)
			}
		})
	}
}

var errorCases = []struct {
	in  []string
	err error
}{
	{[]string{"+", "3"}, errors.New("stack is empty")},
	{[]string{"3", "-"}, errors.New("stack is empty")},
	{[]string{"-", "3", "-", "5"}, errors.New("stack is empty")},
	{[]string{"(", "3", "+", "5", ")", ")"}, errors.New("incorrect usage of (.")},
	{[]string{"(", "3", "+", "5", "-", "g", ")"}, errors.New("unknown elements in expression")},
}

func TestErrorCases(t *testing.T) {
	for k, tt := range errorCases {
		t.Run("test_"+strconv.Itoa(k), func(t *testing.T) {
			_, err := Calc(tt.in)
			if !reflect.DeepEqual(err, tt.err) {
				t.Errorf("got %q, want %q", err, tt.err)

			}
		})
	}
}
