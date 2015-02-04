using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using SpringHealthOne.Models;

namespace SpringHealthOne.App_Start
{
    public class MenuItemConfiguration : EntityTypeConfiguration<MenuItem>
    {
        public MenuItemConfiguration()
        {
            HasKey(x => x.MenuItemID);

            Property(x => x.MenuItemID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            HasOptional(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentID)
                .WillCascadeOnDelete(false);
        }
    } 
}