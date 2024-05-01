using DoorAccessManager.Data;
using DoorAccessManager.Items.Entities;
using DoorAccessManager.Items.Enums;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace DoorAccessManager.Api
{
    public static class Initializer
    {
        private static readonly Guid _employeeRoleId = Guid.NewGuid();
        private static readonly Guid _adminRoleId = Guid.NewGuid();
        private static readonly Guid _officeManagerRoleId = Guid.NewGuid();


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

                context.Roles.AddRange(GetRoles());
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
                db.Roles.AddRange(GetRoles());
                db.Offices.Add(GetOffice());

                db.SaveChanges();
            }
        }

        private static List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Username = "first_emp",
                    Name = "First Employee",
                    PasswordHash = BC.HashPassword("123"),
                    RoleId = _employeeRoleId
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive=true,
                    Username = "second_emp",
                    Name = "Second Employee",
                    PasswordHash = BC.HashPassword("123"),
                    RoleId = _employeeRoleId
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Username = "admin",
                    Name = "Admin",
                    PasswordHash = BC.HashPassword("123"),
                    RoleId = _adminRoleId
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Username = "office_manager",
                    Name = "Office Manager",
                    PasswordHash = BC.HashPassword("123"),
                    RoleId = _officeManagerRoleId
                },
            };
        }

        private static List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role
                {
                    Id = _employeeRoleId,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Name = RoleTypes.Employee.ToString()
                },
                new Role
                {
                    Id = _adminRoleId,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Name = RoleTypes.Admin.ToString()
                },
                new Role
                {
                    Id = _officeManagerRoleId,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Name = RoleTypes.OfficeManager.ToString()
                }
            };
        }

        private static List<Door> GetDoors()
        {
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
                            RoleId = _employeeRoleId
                        },
                        new DoorRole
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.UtcNow,
                            RoleId= _adminRoleId
                        },
                        new DoorRole
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.UtcNow,
                            RoleId = _officeManagerRoleId
                        }
                    }
                },
                new Door
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    Name ="Storage Room",
                    DoorRoles = new List<DoorRole>
                    {
                        new DoorRole
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.UtcNow,
                            RoleId= _adminRoleId
                        },
                        new DoorRole
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.UtcNow,
                            RoleId = _officeManagerRoleId
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
