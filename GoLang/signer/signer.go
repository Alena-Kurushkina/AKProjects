package main

import (
	"fmt"
	"sort"
	"strconv"
	"strings"
	"sync"
)

// SingleHash считает значение crc32(data)+"~"+crc32(md5(data))
func SingleHash(input, output chan interface{}) {
	var mutex sync.Mutex // для блокировки DataSignerMd5
	var wg sync.WaitGroup
	for in := range input {
		str := fmt.Sprintf("%v", in)  // преобразование interface в строку
		locCh := make(chan string, 1) // канал для передачи между crc32Hash и singleHasher
		wg.Add(1)

		go crc32Hash(str, locCh)

		go singleHasher(str, locCh, output, &mutex, &wg)
	}
	wg.Wait()
}

// crc32Hash считает значение crc32(data)
func crc32Hash(str string, locCh chan<- string) {
	hash := DataSignerCrc32(str)
	locCh <- hash
}

// singleHasher считает значение crc32(md5(data)) и в выходной канал подаёт crc32(data)+"~"+crc32(md5(data))
func singleHasher(str string, locCh <-chan string, output chan interface{}, mutex *sync.Mutex, wg *sync.WaitGroup) {
	defer wg.Done()
	mutex.Lock() // т.к. DataSignerMd5 может одновременно вызываться только 1 раз
	mdHash := DataSignerMd5(str)
	mutex.Unlock()
	crcMdHash := DataSignerCrc32(mdHash)
	crcHash := <-locCh // получаем левую часть хэша из локального канала
	res := crcHash + "~" + crcMdHash
	output <- res
}

// MultiHash считает crc32(th+data)) для потока значений из канала, где th=0..5
func MultiHash(input, output chan interface{}) {
	wg := sync.WaitGroup{}
	for in := range input {
		snglHash := fmt.Sprintf("%v", in) // преобразование interface в строку
		wg.Add(1)

		go multiHasher(snglHash, output, &wg)
	}
	wg.Wait()
}

// multiHasher считает crc32(th+data) для значения из канала, где th=0..5
func multiHasher(snglHash string, output chan interface{}, wg *sync.WaitGroup) {
	defer wg.Done()
	mltHash := make([]string, 6) // слайс для 6 значений crc32(th+data), где th=0..5
	wgLoc := sync.WaitGroup{}
	for i := 0; i < 6; i++ {
		wgLoc.Add(1)

		// расчёт значения crc32(i+data)
		go func(i int, mltHash []string, wgLoc *sync.WaitGroup) {
			mltHash[i] = DataSignerCrc32(strconv.Itoa(i) + snglHash)
			wgLoc.Done()
		}(i, mltHash, &wgLoc)
	}
	wgLoc.Wait()
	output <- strings.Join(mltHash, "")
}

// CombineResults получает все результаты, сортирует и объединяет отсортированный результат через _ в одну строку
func CombineResults(input, output chan interface{}) {
	var combParts []string
	for in := range input {
		mltHash := fmt.Sprintf("%v", in) // преобразование interface в строку
		combParts = append(combParts, mltHash)
	}
	sort.Strings(combParts)
	output <- strings.Join(combParts, "_")
}

// ExecutePipeline обеспечивает конвейерную обработку функций-воркеров
func ExecutePipeline(jobs ...job) {
	inChan := make(chan interface{}) // входной канал
	wg := sync.WaitGroup{}
	for _, jobF := range jobs { // перебор по функциям-воркерам
		wg.Add(1)
		outChan := make(chan interface{})
		go executeFunc(&wg, jobF, inChan, outChan)
		inChan = outChan // выходной канал
	}
	wg.Wait()
}

// executeFunc обеспечивает выполнение функции-воркера
func executeFunc(wg *sync.WaitGroup, jobFunction job, inChan chan interface{}, outChan chan interface{}) {
	jobFunction(inChan, outChan)
	wg.Done()
	close(outChan)
}
