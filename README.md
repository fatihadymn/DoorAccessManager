# **Door Acceess Manager**
Door Access Manager system provide to manage and view **Office**, **User**, **Door** and **Access Logs**. There is no necessary to install any additional things to run and build the code base. If your system has .Net 7 requirements to run it, that is enough.

## **Tech Stack and Libraries**
- ```.Net 7.0```
- ```SQLite```
  - To run solution without any changes or installments. It is a relational database and creating while runtime.
- ```InMemoryDatabase (For UnitTests)```
  - When we want to write some unit tests for our repositories. We cannot mock our Database Context directly. So I am configuring InMemoryDatabase and able to write tests with real database.
- ```Entity Framework```
- ```Mapster```
  - This is the alternative for AutoMapper. However while using Mapster we do not have to add Profiles like in AutoMapper. We can write custom mapping and we can map directly. 
- ```Fluent Validation```
  - I used this library for request validations instead of Data Annotations.
- ```BCrypt```
  - This library provide to hash for strings. I want to keep hashed passwords instead of passwords directly in database.
- ```JWT```
  - This is my authentication type for my solution.
<br></br>
## **About Architecture**
This project has one Web API and four Class Library.  

![image](https://github.com/fatihadymn/DoorAccessManager/assets/38660944/495cb477-0627-4592-ab02-38e1bf88e7a4)

- ```DoorAccessManager.Api``` has Controllers and Initializations for project. This is the main API Project.
- ```DoorAccessManager.Core``` has Services' classes and interfaces.
- ```DoorAccessManager.Data``` has our Database Context, Configurations for Entities, Repositories and Migrations.
- ```DoorAccessManager.Items``` has Entities, Request/Response Models, Enums, Mapping Configurations and Validators for requests.
- ```DoorAccessManager.Tests``` has Unit Tests for project.
<br></br>
## **How to see Database**
  After running the project, **DoorAccessManager.db** will be created automatically. Also migrations will be apply to database and some data will be added by project. This file is creating by system while runtime. If you want to reset it just delete file and restart the project.
  
  ![image](https://github.com/fatihadymn/DoorAccessManager/assets/38660944/7aacd717-432b-4bfa-b4f1-8bbc8df89b80)

  If you want to check inside of this file. You can use some online applications like [SQLiteViewer](https://sqliteviewer.app/).
<br></br>
## **Test**
This system has three roles like below.
```sh
Admin
Employee
OfficeManager 
  ```

This system has default users and all passwords are ```123``` for all users below.

![image](https://github.com/fatihadymn/DoorAccessManager/assets/38660944/ad04797e-84a0-431a-a074-d88acc66f08d)

### **Swagger**
You can see all endpoints on swagger's page when you run the project. Every endpoints need **JWT TOKEN** except ```POST /api/users/login``` method. We can use Authorize button in the top right with ```Bearer {JWT TOKEN}```.

![image](https://github.com/fatihadymn/DoorAccessManager/assets/38660944/70cc0c0f-3e47-4c38-85a9-a762c0903a28)
<br></br>
These endpoints are required to access doors and to see access logs.

![image](https://github.com/fatihadymn/DoorAccessManager/assets/38660944/9e7fad84-cb67-415c-96e8-88360adb3b39)

- ```GET /api/doors``` provides authenticated user's office doors list. It is up to office and authenticated user's role. Every role can access this endpoint.
- ```POST /api/doors/{id}/access``` provides to check that authenticated user can access that door. Every role can access this endpoint.
- ```GET /api/doors/{id}/access-logs``` provides to see access logs for exact door. Only Admin and OfficeManager can access this endpoint.
- ```POST /api/users/login``` is using for authentication. When we use this endpoint with correct ```username``` and ```password``` we can get **JWT TOKEN**. Everyone should access this endpoint.
<br></br>
These endpoints are extra to manage couple of User Process.

![image](https://github.com/fatihadymn/DoorAccessManager/assets/38660944/fcd27330-cb8c-4cf6-a668-1219e93bef28)

- ```POST /api/users``` provides to create new user to authenticated user's office. Only Admin can access this endpoint.
- ```GET /api/users``` provides to office's users list. Only Admin can access this endpoint.
- ``` PATCH /api/users/{id}/password``` provides to change only your password. Every role can access this endpoint.
- ```DELETE /api/users/{id}``` can make soft delete from users. Only Admin can access this endpoint.
<br></br>
## **Clear**
  When all tests are done, you can just delete your local database file and that's it.
