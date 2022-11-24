using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ShahadatApp.Models;
namespace ShahadatApp.DAL
{
    public class ShahadatAppContext : DbContext
    {
        public ShahadatAppContext() : base()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ShahadatAppContext>());
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Citizen> Citizen { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<Talab> Talab { get; set; }
        public DbSet<Notification> Notification { get; set; }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}