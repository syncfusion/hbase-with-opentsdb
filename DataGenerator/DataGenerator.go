package main                                                                                                                                                           

import (
    "fmt"
    "os"
	"math/rand"
	"net"
    "strconv"
	"path/filepath"
	)
	
func main() {
	//Create a single buffered channel - We will use it write data into logs
	dataChannel := make(chan string, 4000000)
	
	//Create TCP connection to TSDB server by specifying its HostName and PortNumber.
	//connection, _ := net.Dial("tcp", "HostName/IP:PortNumber")
	connection, _ := net.Dial("tcp", "192.168.80.8:4242")
	
	//Starting date for time series data points - 01/15/2016 (JAN 15 2015)
	timeinSeconds := int64(1452839461)
	
	//Start GO Routines to write random time series data points for Metrics(CPU, Memory, Disk and Network usage) into HBase
	go Write(connection, "cpu.usage", timeinSeconds, 100, dataChannel) // limit value specifies the range between 0 and 100 percent of CPU usage
	go Write(connection, "memory.usage", timeinSeconds, 100, dataChannel)
	go Write(connection, "disk.usage", timeinSeconds, 100, dataChannel)
	go Write(connection, "network.usage", timeinSeconds, 10, dataChannel) // limit value specifies the range between 0 and 10 MBps of network usage

	//Log the generated random data into a file
	go Logger(dataChannel)
	
	//Wait for user input to close.
	fmt.Scanln();	
}
	
//Method to Generate and write 500k rows (time series data) per metric into HBase via TSDB server
func Write(connection net.Conn, metric string, timeinSeconds int64, limit int, dataChannel chan string) {

	for i := 1; i<=500000; i++ {
		value := strconv.Itoa(rand.Intn(limit))
		//Put command for writing data into TSDB server
		telnetCommand := "put " + metric + " " + strconv.FormatInt(timeinSeconds, 10) + " " + value + " core=4"			 		 
		//Writes data into HBase via TSDB server
		fmt.Fprintf(connection, telnetCommand + "\n" )
				
		//Send data to channel for logging
		dataChannel <- metric + "\t" + strconv.FormatInt(timeinSeconds, 10) + "\t" + value
		timeinSeconds +=1 //Increment Timestamp by 1 second
	}
}	

//Method to log the data points written into HBase
func Logger(dataChannel chan string){
	//Create log file
	Path, _ := filepath.Abs("../log.tsv")
    file, err := os.Create(Path)
		if err != nil {
			return
		}

	//Write data from dataChannel into the text file.
	for message := range dataChannel {
		file.WriteString(message + "\n")
	}	
}



