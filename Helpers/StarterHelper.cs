using EasyTwoJuetengBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Helpers
{
    public static class StarterHelper
    {
        private const string _defaultPassword = "Admin12345!";
        public static void CreateAccountsIfNeccesary(string connString)
        {
            var sqlcontrols = new SqlControls(connString);
            var userRoles = sqlcontrols.MultiSelectData<UserRole>("Select * From UserRoles");
            if(userRoles.Count==0)
            {
                var admin = new UserRole()
                {
                    Name= "Admin"
                }; var agent = new UserRole()
                {
                    Name = "Agent"
                };
                sqlcontrols.Add("UserRoles", admin);
                sqlcontrols.Add("UserRoles", agent);
            }
            var cities = sqlcontrols.MultiSelectData<City>("Select * From Cities");
            if(cities.Count== 0)
            {
                var city = new City()
                {
                    Name = "Olongapo City"
                };
                sqlcontrols.Add("Cities", city);
            }
            var workLocations = sqlcontrols.MultiSelectData<WorkLocation>("Select * From WorkLocations");
            if (workLocations.Count == 0)
            {
                var workLocation = new WorkLocation()
                {
                    Name = "Head Quarter",
                    Barangay= "Sample Barangay",
                    CityId = 1
                };
                sqlcontrols.Add("WorkLocations", workLocation);
            }
            var employees = sqlcontrols.MultiSelectData<Employee>("Select * From Employees");
            if (employees.Count == 0)
            {
                var adminEmployee = new Employee()
                {
                    NickName = "System Generated Super Admin",
                    FirstName = "Super",
                    LastName = "Admin",
                    MiddleName = "Pre Generated",
                    Address = "Olongapo City (System Generated)",
                    ContactNumber = "N/A (Edit Upon Official Use)",
                    EmailAddress = "N/A (Edit Upon Official Use)",
                    WorkLocationId = 1,
                    DateTimeCreated = DateTime.Now
                };
                var agentEmployee = new Employee()
                {
                    NickName = "System Generated Agent",
                    FirstName = "Agent",
                    LastName = "Employee",
                    MiddleName = "Pre Generated",
                    Address = "Olongapo City (System Generated)",
                    ContactNumber = "N/A (Edit Upon Official Use)",
                    EmailAddress = "N/A (Edit Upon Official Use)",
                    WorkLocationId = 1,
                    DateTimeCreated = DateTime.Now
                };
                sqlcontrols.Add("Employees", adminEmployee);
                sqlcontrols.Add("Employees", agentEmployee);
            }
            var users = sqlcontrols.MultiSelectData<User>("Select * From Users");
            if(users.Count==0)
            {
                var adminUser = new User()
                {
                    UserName = "easytwosuperadmin",
                    Password = AES.Encrypt(_defaultPassword),
                    UserRoleId = 1,
                    EmployeeId = 1,
                    DateTimeCreated = DateTime.Now
                };
                var agentUser = new User()
                {
                    UserName = "easytwoagent",
                    Password = AES.Encrypt(_defaultPassword),
                    UserRoleId = 2,
                    EmployeeId = 2,
                    DateTimeCreated = DateTime.Now
                };
                sqlcontrols.Add("Users", adminUser);
                sqlcontrols.Add("Users", agentUser);
            }
            var gameModes = sqlcontrols.MultiSelectData<GameMode>("Select * From GameModes");
            if (gameModes.Count == 0)
            {
                var straightGame = new GameMode()
                {
                    Name = "Straight"
                };
                var mixedGame = new GameMode()
                {
                    Name = "Straight"
                };
                sqlcontrols.Add("GameModes", straightGame);
                sqlcontrols.Add("GameModes", mixedGame);
            }
            var gameSchedules = sqlcontrols.MultiSelectData<GameSchedule>("Select * From GameSchedules");
            if (gameModes.Count == 0)
            {
                var morningGame = new GameSchedule()
                {
                    Name = "Moring Session",
                    EntryStart = TimeSpan.Parse("7:00"),
                    EntryEnd = TimeSpan.Parse("8:30"),
                    DrawTime = TimeSpan.Parse("9:00"),
                    IsActive= true,
                    DateTimeCreated = DateTime.Now
                };
                sqlcontrols.Add("GameSchedules", morningGame);
            }
        }
    }
}
