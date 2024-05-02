# Door Acceess Manager
Door Access Manager system provide to manage and view **Office**, **User**, **Door** and **Access Logs**. There is no necessary to install any additional things to run and build the code base.If your system has .Net 7 requirements to run it, that is enough.

## Tech Stack and Libraries
- **.Net 7.0**
- **SQLite**
  - To run solution without any changes or installments. It is a relational database and creating while runtime.
- **InMemoryDatabase (For UnitTests)**
  - When we want to write some unit tests for our repositories. We cannot mock our Database Context directly. So I am configuring InMemoryDatabase and able to write tests with real database.
- **Entity Framework**
- **Mapster**
  - This is the alternative for AutoMapper. However while using Mapster we do not have to add Profiles like in AutoMapper. We can write custom mapping and we can map directly. 
- **Fluent Validation**
  - I used this library for request validations instead of Data Annotations.
- **BCrypt**
  - This library provide to hash for strings. I want to keep hashed passwords instead of passwords directly in database.
- **JWT**
  - This is my authentication type for my solution.

## About Architecture
This project has one Web API and four Class Library.  

![image](https://github.com/fatihadymn/DoorAccessManager/assets/38660944/495cb477-0627-4592-ab02-38e1bf88e7a4)

- **DoorAccessManager.Api** has Controllers and Initializations for project. This is the main API Project.
- **DoorAccessManager.Core** has Services' classes and interfaces.
- **DoorAccessManager.Data** has our Database Context, Configurations for Entities, Repositories and Migrations.
- **DoorAccessManager.Items** has Entities, Request/Response Models, Enums, Mapping Configurations and Validators for requests.
- **DoorAccessManager.Tests** has Unit Tests for project.

## How to see Database
  After running the project, **DoorAccessManager.db** will be created automatically. Also migrations will be apply to database and some data will be added by project automatically. This file is creating by system while runtime. If you want to reset it just delete file and restart the project.
  
  ![image](https://github.com/fatihadymn/DoorAccessManager/assets/38660944/7aacd717-432b-4bfa-b4f1-8bbc8df89b80)

  If you want to check inside of this file. You can use some online applications like [SQLiteViewer](https://sqliteviewer.app/).

## Test
### Swagger
You can see all endpoints on swagger's page when you run the project.

![image](https://github.com/fatihadymn/DoorAccessManager/assets/38660944/70cc0c0f-3e47-4c38-85a9-a762c0903a28)



---------------------

## Clear
  When all tests are done, you can just delete your local database file and that's it.
