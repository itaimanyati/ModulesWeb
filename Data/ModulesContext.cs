using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModulesWeb.Data
{
    public class ModulesContext : DbContext
    {
        public ModulesContext(DbContextOptions<ModulesContext> options) :base(options)
        {

        }
        public DbSet<Module> Modules { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<StudyHours> StudyHours { get; set; }

        public DbSet<Semester> Semesters { get; set; }

        public DbSet<Notification> Notifications { get; set; }
    }
}
