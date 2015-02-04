using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SpringHealthOne.Models;
using System.Web.Mvc;

namespace SpringHealthOne.App_Start
{
    public class SpringerContextInitializer : DropCreateDatabaseIfModelChanges<SpringerContext>
    {
        protected override void Seed(SpringerContext context)
        {
            /*var menuitems = new List<MenuItem>
            {
                new MenuItem { Label = "About",
                               PageID = 1,
                               DisplayOrder = 1,
                               Published = true },

                new MenuItem { Label = "History",
                               DisplayOrder = 2,
                               Published = true }, 

                new MenuItem { Label = "Home",
                               DisplayOrder = 3,
                               Published = true }
            };

            menuitems.ForEach(m => context.MenuItems.Add(m));

            var pages = new List<Page>
            {
                new Page {Title = "About", SystemName="about"}
            };

            pages.ForEach(p => context.Pages.Add(p));
            context.SaveChanges();
             * */
        }
    }
}