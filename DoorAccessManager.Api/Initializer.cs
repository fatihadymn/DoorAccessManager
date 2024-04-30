using DoorAccessManager.Data;
using DoorAccessManager.Items.Entities;
using DoorAccessManager.Items.Enums;
using Microsoft.EntityFrameworkCore;

namespace DoorAccessManager.Api
{
    public static class Initializer
    {
        public static void InitializeDatabase(IApplicationBuilder application)
        {
            var serviceScope = application.ApplicationServices.CreateScope();

            var provider = serviceScope.ServiceProvider;

            var db = provider.GetService<ApplicationDbContext>();

            if (db != null)
            {
                try
                {
                    if (db.Offices.Count() > 0)
                        return;
                }
                catch (Exception)
                {
                    Console.WriteLine("This application need migrations.We are applying migrations and adding test data");
                }

                db.Database.Migrate();
            }

            PrepareData(application);
        }

        public static void PrepareUnitTestData(ApplicationDbContext context)
        {
            if (context is not null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Offices.Add(GetOffice());

                context.SaveChanges();
            }
        }

        public static void PrepareData(IApplicationBuilder application)
        {
            var serviceScope = application.ApplicationServices.CreateScope();

            var provider = serviceScope.ServiceProvider;

            var db = provider.GetService<ApplicationDbContext>();

            if (db is not null)
            {
                db.Offices.Add(GetOffice());

                db.SaveChanges();
            }
        }

        private static List<User> GetUsers()
        {
            var roles = GetRoles();
            var employeeRole = roles.Where(x => x.Name == RoleTypes.Employee.ToString()).FirstOrDefault()!;
            var adminRole = roles.Where(x => x.Name == RoleTypes.Admin.ToString()).FirstOrDefault()!;
            var officeManagerRole = roles.Where(x => x.Name == RoleTypes.OfficeManager.ToString()).FirstOrDefault()!;

            return new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    UserName = "first_emp",
                    Name = "First Employee",
                    PasswordHash = "123",
                    Role = employeeRole
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive=true,
                    UserName = "second_emp",
                    Name = "Second Employee",
                    PasswordHash = "123",
                    Role = employeeRole
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    UserName = "admin",
                    Name = "Admin",
                    PasswordHash = "123",
                    Role = adminRole
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    UserName = "office_manager",
                    Name = "Office Manager",
                    PasswordHash = "123",
                    Role = officeManagerRole
                },
            };
        }

        private static List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Name = RoleTypes.Employee.ToString()
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Name = RoleTypes.Admin.ToString()
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Name = RoleTypes.OfficeManager.ToString()
                }
            };
        }

        private static List<Door> GetDoors()
        {
            var roles = GetRoles();
            var employeeRoleId = roles.FirstOrDefault(x => x.Name == RoleTypes.Employee.ToString())!.Id;
            var adminRoleId = roles.FirstOrDefault(x => x.Name == RoleTypes.Admin.ToString())!.Id;
            var officeManagerRoleId = roles.FirstOrDefault(x => x.Name == RoleTypes.OfficeManager.ToString())!.Id;

            return new List<Door>
            {
                new Door
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Name ="Main Entrance",
                    DoorRoles = new List<DoorRole>
                    {
                        new DoorRole
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.UtcNow,
                            RoleId = employeeRoleId
                        },
                        new DoorRole
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.UtcNow,
                            RoleId= adminRoleId
                        },
                        new DoorRole
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.UtcNow,
                            RoleId = officeManagerRoleId
                        }
                    }
                },
                new Door
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Name ="Warehouse",
                    DoorRoles = new List<DoorRole>
                    {
                        new DoorRole
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.UtcNow,
                            RoleId= adminRoleId
                        },
                        new DoorRole
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.UtcNow,
                            RoleId = officeManagerRoleId
                        }
                    }
                }
            };
        }

        private static Office GetOffice()
        {
            return new Office
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                Name = "First Office",
                Doors = GetDoors(),
                Users = GetUsers()
            };
        }
    }
}
