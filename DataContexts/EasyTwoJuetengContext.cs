using EasyTwoJuetengBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.DataContexts
{
    public class EasyTwoJuetengContext: DbContext
    {
        public EasyTwoJuetengContext(DbContextOptions<EasyTwoJuetengContext> options)
        : base(options)
        {
        }

        #region User Related DbSets
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }

        #endregion

        #region Employee Location Related DbSets
        public DbSet<City> Cities { get; set; }
        public DbSet<WorkLocation> WorkLocations { get; set; }
        #endregion

        #region Customer Related DbSets
        public DbSet<Customer> Customers { get; set; }
        #endregion

        #region Game Related DbSets
        public DbSet<GameSchedule> GameSchedules { get; set; }
        public DbSet<GameMode> GameModes { get; set; }
        public DbSet<Entry> Entries { get; set; }
        #endregion

        #region Audit Trail Related DbSet
        public DbSet<AuditTrail> AuditTrails { get; set; }
        #endregion


    }
}
