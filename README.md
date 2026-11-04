# DTS Developer Technical Test
## Introduction
This repository contains all the data related to this candidates implementation of the backend server for the DTS Developer Technical Test.

## Technical Notes
### Backend API
The backend API is implemented using .NET 10.0 and is written in C#, specifically language version 14.0. It makes use of the following .NET libraries:

- Microsoft ASP.NET Core 7.0
- Microsoft Entity Framework Core 7.0

These provide the necessary functionality for application routing and database access respectively.

#### API Endpoints
The following endpoints are available for use:

- GET /task - Retrieves a list of all tasks in the database.
- GET /task/\{id} - Retrieves a specific task by its ID.
- POST /task - Creates a new task in the database with the supplied data.
- PUT /task/\{id} - Updates an existing task in the database with the supplied data.
- DELETE /task/\{id} - Deletes a specific task from the database by its ID.

POST requests to the /task endpoint require a JSON body with the following structure:
```json
{
  "title": "Task Title",
  "description": "Task Description",
  "status": "Pending",
  "dueDate": "2024-06-30T00:00:00Z"
}
```
Note that the all fields except "description" must be non-null. Additionally, the "status" field must correspond exactly to one of the enum types defined by TaskStatus. Requests can either be the string representation or the numerical value.

As for PUT requests, the JSON body must have the same structure as above, but all fields are optional. Please note that while tasks also have an ID value, this is automatically tracked and updated by Entity Framework Core and as such does not need to be set in either request.
