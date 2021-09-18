# TasksManager

## About that project
TasksManager is a console application that sends CRUD (Create, Read, Update, Delete) http/s operations and help you manage your tasks seperated by groups.

From TasksManager Version 1.1 I decided to seperate it into different projects, this one and [TasksManager-WebService Project](https://github.com/DorShaar/TasksManager-WebService "TasksManager-WebService"). This project is the client side which is responsible for sending requests to the server and update the database accordingly.

## What Tasks and Groups are consisted of
Each task has status of "Open", "Closed" or "On-Work".\
Each task can have:

* Note.
* Task Triangle - Gives the ability to control your tasks in three dimensions:\
a. Time (What is the dead-line).\
b. Quality (What is the contnet of the task).\
c. Resources (Who work on that task).

Group's status is dependent in all of the tasks statuses it conatins.\

## Usage
 ```tasker get tasks```\
 Exmple: \
 ![alt text](https://github.com/DorShaar/TasksManager/blob/master/images/get_tasks.png "tasker get tasks")
 
 Tasker optional operations:\
 ![alt text](https://github.com/DorShaar/TasksManager/blob/master/images/help.PNG "tasker help")
 
 Tasker get options:\
 ![alt text](https://github.com/DorShaar/TasksManager/blob/master/images/get_groups_help.PNG "tasker get options")

## Development patterns and information I used and learn in that project

IOC (Invertion of control),\
Console logger with Microsoft.ILogger,\
Console with command line parser,\
Usage of HttpClientFactory,\
Use Github-NuGet's service,\
