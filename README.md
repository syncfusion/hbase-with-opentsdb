##Store Massive amounts of time series data using the Syncfusion Big Data Platform and Open TSDB

We setup a Apache HBase cluster using the [Syncfusion Big Data platform](https://www.syncfusion.com/products/big-data). No special configuration was initially needed. We simply used the cluster manager provided with the Syncfusion Big Data Platform to setup the cluster in a few minutes. You may use a cluster of any size for testing (minimum size of 3 data nodes is recommended).

We installed OpenTSDB on a single machine running Ubuntu. Installation directions are available [here](http://opentsdb.net/docs/build/html/installation.html#id1). We configured the OpenTSDB tool to perform read/write operations into Apache HBase running on the Syncfusion Big Data Platform. To generate sustained writes, we have used a custom Go language client that interacts with OpenTSDB to write generated data points into HBase. The Go language was chosen for simulating writes since it has excellent support for concurrent execution of functions. It was very simple to scale to a high write rate with Go.

On the client side, we use a Windows Desktop client running on the Microsoft .NET Windows Presentation Foundation (WPF). Data from read operations was used to populate a rich chart UI. 

###Steps to run the DataGenerator 

1. Download and install the GO environment - [https://golang.org/doc/install ](https://golang.org/doc/install )
2. Make sure that environmental variables are configured with right path as mentioned in the Go documentation.
3. Download our Data Generator 
4. Update TSDB server details (hostname and port number, timestamp) in the sample, we have used time stamp as” 1452839461” dated 2016/01/15-12:01:01 for the sample. 
5. Run the GO client using below command 
    
  ```go run DataGenerator.go ```

###Steps to run the DataChecker

1. Open and build the DataChecker project in Visual Studio.
2. Run the console application using below command

  ```Syncfusion.DataChecker.exe "logFileLocation” 	"TSDBHostName:PortNumber”```

Application will print “Data integrity check success.” for the metric if the data from the log file is available in HBase.

###Steps to run the MonitoringDashboard

1. Run the application using Visual Studio.
2. Provide TSDB server details, start date and time and then click fetch button.

You will see that the time series data from HBase will be populated in the charts. 

###Steps to run the PreSplitting 

>Please ensure that "SYNCBDP_HOME" path is set in system environment variable before building the application through Gradle.
>set SYNCBDP_HOME="<InstallDrive>\Syncfusion\BigData\<version>"
>make sure GRADLE_HOME and JAVA_HOME are set in system environment variable
 
####Gradle in command line
Open Hadoop command shell, type 

To build: ```gradle -p <Gradle Sample Location>\PreSplitting build```

To run: ```gradle -p <Gradle Sample Location>\PreSplitting run```

####Gradle in Netbeans

Open the gradle project in Netbeans IDE.

To build:

        Right click on the project and click 'build'.

To run: 

        Right click on the project click Custom Tasks-> Custom Tasks

        Under 'Tasks' type 'run'

        Click Execute to run the sample.

####Gradle in Eclipse

File-> Import-> Import Gradle project.

Choose the Gradle project and click 'Build Model'. Click Finish.

To build:

          Right click the project, click Run As-> Gradle Build.  

          Under Gradle Tasks type 'build' (or) 'jar', Apply and then click Run.
            
To run:

          Right click the project, click Run As-> Gradle Build.

          Under Gradle Tasks type 'run'

###License
Copyright 2016 Syncfusion Inc.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0
    
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
