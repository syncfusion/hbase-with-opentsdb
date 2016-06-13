#Steps to run HBase Gradle samples through Java API
/* Please ensure that "SYNCBDP_HOME" path is set in system environment variable before building the application through Gradle.
 * set SYNCBDP_HOME="<InstallDrive>\Syncfusion\BigData\<version>"
 * make sure GRADLE_HOME and JAVA_HOME are set in system environment variable
 */
 
*Gradle in command line
Open BigData command shell, type 
To build : gradle -p <Gradle Sample Location>\PreSplitting build
To run : gradle -p <Gradle Sample Location>\PreSplitting run

* Gradle in Netbeans
Open the gradle project in Netbeans IDE.
To build : Right click on the project and click 'build'.
To run : Right click on the project click Custom Tasks-> Custom Tasks
             Under 'Tasks' type 'run'
             Click Execute to run the sample.

* Gradle in Eclipse
File-> Import-> Import Gradle project.
Choose the Gradle project and click 'Build Model'. Click Finish.
To build : Right click the project, click Run As-> Gradle Build.  
                Under Gradle Tasks type 'build' (or) 'jar', Apply and then click Run.
To run :  Right click the project, click Run As-> Gradle Build.
              Under Gradle Tasks type 'run'

* Gradle in IntelliJ
Open the gradle project in IntelliJ IDE.
Click View-> Tool Windows-> Gradle to open gradle projects. 
Click on the icon 'Execute Gradle Task'.
Under 'Gradle project' choose the Gradle project for execution.
Under 'Command line',
        To build sample : build  (or) jar (this generates an executable jar file under build/libs folder)
Click OK to run the sample.
