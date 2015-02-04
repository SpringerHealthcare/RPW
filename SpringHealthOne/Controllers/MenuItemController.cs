using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SpringHealthOne.Models;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

using System.Globalization;
using SpringHealthOne.SpringWS;
using System.Security.Cryptography;

namespace SpringHealthOne.Controllers
{
    public class MenuItemController : Controller
    {
        private SpringerContext db = new SpringerContext();

        //
        // GET: /MenuItem/

        public ActionResult Index()
        {
            string username = User.Identity.Name;
            Utils Utils = new Utils();
            if (Utils.UserAdmin(username) == true){
                var menuItems = db.MenuItems.OrderBy(c => c.DisplayOrder).ToList();

                return View(menuItems);
            }
            else
            {
                return HttpNotFound();
            }
        }

        //
        // GET: /MenuItem/Details/5

        public ActionResult Details(int id = 0)
        {
            MenuItem menuitem = db.MenuItems.Find(id);
            if (menuitem == null)
            {
                return HttpNotFound();
            }
            
            return View(menuitem);
        }

        //
        // GET: /MenuItem/Create

        public ActionResult Create()
        {
            var model = new MenuItem();
            GetPossibleParents(0);
            return View(model);
        }

        //
        // POST: /MenuItem/Create

        [HttpPost]
        public ActionResult Create(MenuItem menuitem)
        {
            if (menuitem.ParentID == 0)
            {
                menuitem.ParentID = null;
            }
            else
            {
                Page page = db.Pages.Find(menuitem.PageID);
                menuitem.URL = page.Title;
            }

            if (ModelState.IsValid)
            {
                db.MenuItems.Add(menuitem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menuitem);
        }

        //
        // GET: /MenuItem/Edit/5

        public ActionResult Edit(int id = 0)
        {
            MenuItem menuitem = db.MenuItems.Find(id);
            if (menuitem == null)
            {
                return HttpNotFound();
            }

            GetPossibleParents(id);
            GetPages();
            return View(menuitem);
        }

        private void GetPages()
        {
            var pages = db.Pages.Select(c => c).ToList();
            pages.Insert(0, new Page() { PageID = 0, Title = "No Menu" });
            ViewData["Pages"] = new SelectList(pages, "PageID", "Title", null);
        }

        private void GetPossibleParents(int id)
        {
            var myMenu = db.MenuItems.Where(c => c.MenuItemID != id).Select(c => c).ToList();
            myMenu.Insert(0, new MenuItem() { MenuItemID = 0, Label = "No Parent" });
            ViewData["Parents"] = new SelectList(myMenu, "MenuItemID", "Label", null);
        }

        //
        // POST: /MenuItem/Edit/5

        [HttpPost]
        public ActionResult Edit(MenuItem menuitem)
        {
            if (menuitem.PageID == 0)
            {
                menuitem.PageID = null;
            }
            else
            {
                Page page = db.Pages.Find(menuitem.PageID);
                menuitem.URL = page.Title;
            }

            
            if (menuitem.ParentID == 0)
            {
                menuitem.ParentID = null;
            }

            if (ModelState.IsValid)
            {
                db.Entry(menuitem).State = EntityState.Modified;
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menuitem);
        }

        //
        // GET: /MenuItem/Delete/5

        public ActionResult Delete(int id = 0)
        {
            MenuItem menuitem = db.MenuItems.Find(id);
            if (menuitem == null)
            {
                return HttpNotFound();
            }
            return View(menuitem);
        }

        //
        // POST: /MenuItem/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            MenuItem menuitem = db.MenuItems.Find(id);
            db.MenuItems.Remove(menuitem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult MenuPartial()
        {
            IEnumerable<MenuItem> menuItems = db.MenuItems.Where(c => c.ParentID == null).OrderBy(c => c.DisplayOrder).Select(c => c);
            return PartialView("_MenuPartial", menuItems);
        }
    }
}