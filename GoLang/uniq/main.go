package main

import (
	"bufio"
	"flag"
	"fmt"
	"io"
	"os"
	"uniq/uniq"
)

func main() {
	opt := uniq.Options{}
	opt.CountRepeatingLines = *(flag.Bool("c", false, "count repeating lines"))
	opt.GetRepeatingLines = *(flag.Bool("d", false, "get only repeating lines"))
	opt.GetNonRepeatingLines = *(flag.Bool("u", false, "get only non-repeating lines"))
	opt.IgnoreCase = *(flag.Bool("i", false, "ignore case"))
	flag.IntVar(&opt.IgnoreFieldsNumber, "f", 0, "number of fields to ignore")
	flag.IntVar(&opt.IgnoreCharsNumber, "s", 0, "number of chars to ignore")

	// uncomment while running main.go

	//flag.Usage = func() { // [4]
	//	fmt.Fprintf(flag.CommandLine.Output(), uniq.Usage, os.Args[0])
	//	flag.PrintDefaults()
	//}

	flag.Parse()

	// определяем входной поток
	var in io.Reader
	if inFilename := flag.Arg(0); inFilename != "" {
		inf, err := os.Open(inFilename)
		if err != nil {
			fmt.Println("error opening file: ", err)
			os.Exit(1)
		}
		defer inf.Close()
		in = inf
	} else {
		in = os.Stdin
	}

	// определяем выходной поток
	var out io.Writer
	if outFilename := flag.Arg(1); outFilename != "" {
		outf, err := os.Create(outFilename)
		if err != nil {
			fmt.Println("error creating file: ", err)
			os.Exit(1)
		}
		defer outf.Close()
		out = outf
	} else {
		out = os.Stdout
	}

	// считываем из входного потока
	buf := bufio.NewScanner(in)
	text := []string{}
	for buf.Scan() {
		text = append(text, buf.Text())
	}

	// ищем уникальные строки с учётом опций
	res, err := uniq.Uniq(text, opt)
	if err != nil {
		fmt.Println(err)
	}

	// записываем в поток вывода
	writer := bufio.NewWriter(out)
	writer.WriteString(res)
	err1 := writer.Flush()
	if err1 != nil {
		fmt.Println(err1)
	}
}
