using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using SpringHealthOne.App_Start;
using System.Web.Mvc;

namespace SpringHealthOne.Models
{
    public class SpringerContext : DbContext
    {
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Page> Pages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<SpringerContext>(new SpringerContextInitializer());

            modelBuilder.Configurations.Add(new MenuItemConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}