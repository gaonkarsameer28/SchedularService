# Scheduler Service

## Description

This project implements a flexible and scalable scheduler service built with C#, .NET Core, MongoDB, and the Quartz framework. It utilizes the Singleton Factory pattern to ensure only one instance of the scheduler service exists. This pattern provides a central point of access to the scheduling functionality and simplifies configuration.

## Features

Dynamic Scheduling: Schedule tasks using cron expressions for flexible recurring or one-time execution.
RESTful API: Add, update, and complete scheduled jobs through a user-friendly REST API.
Extensible Functionality: Integrate with any external API using the provided API information (URL, type, and body).
Singleton Factory Pattern: Ensures a single instance of the scheduler service and simplifies configuration.
Database Agnostic: Easily switch between MongoDB and other databases by modifying the DbSettings class.
## Project Structure

The solution comprises two projects:

QuartzService: 

This project houses the core scheduling functionality using Quartz. It utilizes the Singleton Factory pattern for creating and managing the scheduler service instance.

QuartzApi: 

This sample API demonstrates interacting with MongoDB (can be replaced with other databases) and provides endpoints for managing scheduled jobs through REST (add, update, complete).

## Installation

Prerequisites: Ensure you have .NET Core SDK installed (dotnet --version).
Clone Repository: Clone this repository locally using git clone https://<your_repository_url>.
Restore Dependencies: Run dotnet restore in both QuartzService and QuartzApi directories.
## Configuration

Database Connection: Update the connection string in the DbSettings.cs file (QuartzApi) to your preferred database.
Singleton Factory Configuration (QuartzService):
The implementation details of the Singleton Factory pattern might be specific to your project structure. Look for code that creates and manages a single instance of the scheduler service class.
## Usage

1. Running the Project:

Start the QuartzApi project first.
Retrieve the API base URL from the running QuartzApi instance.
Update appsettings.json in QuartzService with the retrieved API base URL.
Run the QuartzService project.

2. API Usage:

 The API is expected to provide functionalities like:
Adding new scheduled jobs
Updating existing jobs
Marking jobs as completed
## Additional Notes

This project serves as a foundation for building a robust scheduler service.
You can implement additional features like authentication, authorization, job logging, and monitoring as needed.
For more advanced scheduling scenarios, consider exploring advanced features of the Quartz framework.


## Additional Resources

Quartz Framework: https://www.quartz-scheduler.net/documentation/
MongoDB: https://www.mongodb.com/
.NET Core: https://learn.microsoft.com/en-us/dotnet/core/introduction
Singleton Factory Pattern: https://refactoring.guru/design-patterns/singleton
## Job Model Properties
C#

public string Id { get; set; }
public string Timestamp { get; set; }
public DateTime Startdate { get; set; }
public DateTime Enddate { get; set; }
public DateTime LastExcecutedate { get; set; }
public string CronExpression { get; set; }
public string JobName { get; set; }
public string JobStatus { get; set; }
public string APIBody { get; set; }
public string APIType { get; set; }
public string APIURL { get; set; }
