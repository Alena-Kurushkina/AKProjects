package main

import (
	"fmt"
	"io"
	"io/fs"
	"io/ioutil"
	"os"
)

// Tree строит древовидное представление каталога по указанному пути
func tree(out io.Writer, path, tabs string, flag bool) error {
	files, err := ioutil.ReadDir(path)
	if err != nil {
		return err
	}
	if !flag {
		files = sortDir(files) // будем учитывать только директории
	}
	filesLen := len(files)
	//проходим по всем FileInfo в текущей директории
	for k, f := range files {
		var startGraf string // граф перед именем директории или файла
		if k == filesLen-1 { // если текущий FileInfo является последним в директории
			startGraf = "└───"
		} else {
			startGraf = "├───"
		}
		if f.IsDir() { // если текущий FileInfo является директорией
			fmt.Fprintf(out, "%s%s%s\n", tabs, startGraf, f.Name())
			var tabNext string // отступ перед графами следующего уровня
			if k == filesLen-1 {
				tabNext = tabs + "\t"
			} else {
				tabNext = tabs + "│\t"
			}
			pathNext := path + string(os.PathSeparator) + f.Name() // путь к директории следующего уровня
			err1 := tree(out, pathNext, tabNext, flag)             // рекурсивный вызов для обработки следующего уровня
			if err1 != nil {
				return err1
			}
		} else { // если текущий FileInfo является файлом
			fmt.Fprintf(out, "%s%s%s", tabs, startGraf, f.Name())
			if f.Size() == 0 {
				fmt.Fprintf(out, " (empty)\n")
			} else {
				fmt.Fprintf(out, " (%db)\n", f.Size())
			}
		}
	}
	return nil
}

// sortDir выбирает директории из заданного слайса FileInfo
func sortDir(files []fs.FileInfo) []fs.FileInfo {
	dirList := []fs.FileInfo{}
	for _, f := range files {
		if f.IsDir() {
			dirList = append(dirList, f)
		}
	}
	return dirList
}

func DirTree(out io.Writer, path string, fl bool) error {
	err := tree(out, path, "", fl)
	return err
}

//func main() {
//	out := new(bytes.Buffer)
//	Tree(out, "testdata", "", false)
//}
