package main

import (
	"fmt"
	"reflect"
	"strconv"
	"testing"
	"uniq/uniq"
)

var inCase = []string{"I love music.", "I love music.", "I love music.", "", "I love music of Kartik.", "I love music of Kartik.", "Thanks.", "I love music of Kartik.", "I love music of Kartik."}

var cases = []struct {
	in  []string
	out string
	opt uniq.Options
}{
	{inCase,
		"I love music.\n\nI love music of Kartik.\nThanks.\nI love music of Kartik.", uniq.Options{}},
	{inCase,
		"3 I love music.\n1 \n2 I love music of Kartik.\n1 Thanks.\n2 I love music of Kartik.", uniq.Options{CountRepeatingLines: true}},
	{inCase,
		"I love music.\nI love music of Kartik.\nI love music of Kartik.", uniq.Options{GetRepeatingLines: true}},
	{inCase,
		"\nThanks.", uniq.Options{GetNonRepeatingLines: true}},
	{[]string{"I LOVE MUSIC.", "I love music.", "I LoVe MuSiC.", "", "I love MuSIC of Kartik.", "I love music of kartik.", "Thanks.", "I love music of kartik.", "I love MuSIC of Kartik."},
		"I LOVE MUSIC.\n\nI love MuSIC of Kartik.\nThanks.\nI love music of kartik.", uniq.Options{IgnoreCase: true}},
	{[]string{"We love music.", "I love music.", "They love music.", "", "I love music of Kartik.", "We love music of Kartik.", "Thanks."},
		"We love music.\n\nI love music of Kartik.\nThanks.", uniq.Options{IgnoreFieldsNumber: 1}},
	{[]string{"I love music.", "A love music.", "C love music.", "", "I love music of Kartik.", "We love music of Kartik.", "Thanks."},
		"I love music.\n\nI love music of Kartik.\nWe love music of Kartik.\nThanks.", uniq.Options{IgnoreCharsNumber: 1}},
}

func TestMainTask(t *testing.T) {
	for k, tt := range cases {
		t.Run("test_"+strconv.Itoa(k), func(t *testing.T) {
			s, err := uniq.Uniq(tt.in, tt.opt)
			if s != tt.out {
				t.Errorf("got %q, want %q", s, tt.out)
			}
			if err != nil {
				t.Errorf("got error: %q", err)
			}
		})
	}
}

var myCases = []struct {
	in  []string
	out string
	opt uniq.Options
}{
	{[]string{"I love music.", "We fove music."},
		"I love music.", uniq.Options{IgnoreFieldsNumber: 1, IgnoreCharsNumber: 1}},
	{[]string{"I  love music.", "We fove music."},
		"I  love music.\nWe fove music.", uniq.Options{IgnoreFieldsNumber: 1, IgnoreCharsNumber: 1}},
	{[]string{"I  love music.", "We fove music."},
		"I  love music.\nWe fove music.", uniq.Options{IgnoreFieldsNumber: 1, IgnoreCharsNumber: 1}},
	{[]string{" I love music.", "I love music."},
		" I love music.", uniq.Options{IgnoreCharsNumber: 1}},
	{[]string{"", "I love music.", "", "I love music.", "I love music.", ""},
		"1 \n1 I love music.\n1 \n2 I love music.\n1 ", uniq.Options{CountRepeatingLines: true}},
	{[]string{"", "I", "We"},
		"3 ", uniq.Options{CountRepeatingLines: true, IgnoreFieldsNumber: 1}},
	{[]string{"", "I", "We"},
		"2 \n1 We", uniq.Options{CountRepeatingLines: true, IgnoreCharsNumber: 1}},
	{[]string{"", "I", "We"},
		"3 ", uniq.Options{CountRepeatingLines: true, IgnoreCharsNumber: 2}},
	{[]string{"I", "He", "We love"},
		"3 I", uniq.Options{CountRepeatingLines: true, IgnoreFieldsNumber: 2}},
}

func TestMyCases(t *testing.T) {
	for k, tt := range myCases {
		t.Run("test_"+strconv.Itoa(k), func(t *testing.T) {
			s, err := uniq.Uniq(tt.in, tt.opt)
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
	opt uniq.Options
	err error
}{
	{uniq.Options{GetNonRepeatingLines: true, CountRepeatingLines: true},
		fmt.Errorf("Proper usage: uniq [-c | -d | -u] [-i] [-f num] [-s chars] [input_file [output_file]]")},
	{uniq.Options{GetNonRepeatingLines: true, GetRepeatingLines: true},
		fmt.Errorf("Proper usage: uniq [-c | -d | -u] [-i] [-f num] [-s chars] [input_file [output_file]]")},
	{uniq.Options{GetRepeatingLines: true, CountRepeatingLines: true},
		fmt.Errorf("Proper usage: uniq [-c | -d | -u] [-i] [-f num] [-s chars] [input_file [output_file]]")},
}

func TestErrorCases(t *testing.T) {
	for k, tt := range errorCases {
		t.Run("test_"+strconv.Itoa(k), func(t *testing.T) {
			_, err := uniq.Uniq([]string{"I love music"}, tt.opt)
			if !reflect.DeepEqual(err, tt.err) {
				t.Errorf("got %q, want %q", err, tt.err)
			}
		})
	}
}
