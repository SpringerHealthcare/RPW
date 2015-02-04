using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SpringHealthOne.Models;
using SpringHealthOne.SpringWS;

namespace SpringHealthOne.Controllers
{
    public class PagesController : Controller
    {
        private SpringerContext db = new SpringerContext();

        //
        // GET: /Page/

        public ActionResult Index()
        {
            string username = User.Identity.Name;
            Utils Utils = new Utils();
            if (Utils.UserAdmin(username) == true)
            {
                return View(db.Pages.ToList());
            }
            else
            {
                return HttpNotFound();
            }
        }

        

        //
        // GET: /Page/Details/5

        public ActionResult Details(int id = 0)
        {
            Page page = db.Pages.Find(id);
            var e = Encoding.GetEncoding("iso-8859-1");
            if (page == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = page.Title;
            ViewBag.Description = page.MetaDescription;
            ViewBag.MetaTitle = page.MetaTitle;
            ViewBag.Keywords = page.Keywords;
            return View(page);
        }

        //
        // GET: /Pages/Create

        public ActionResult Create()
        {
            string username = User.Identity.Name;
            Utils Utils = new Utils();
            if (Utils.UserAdmin(username) == true)
            {
                return View();
            }
            else
            {
                return HttpNotFound();
            }
        }

        //
        // POST: /Page/Create

        [HttpPost]
        public ActionResult Create(Page page)
        {
            if (ModelState.IsValid)
            {
                //page.PageBody = MvcHtmlString.Create(page.PageBody.ToString());
                db.Pages.Add(page);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(page);
        }

        //
        // GET: /Page/Edit/5

        public ActionResult Edit(int id = 0)
        {
            string username = User.Identity.Name;
            Utils Utils = new Utils();
            if (Utils.UserAdmin(username) == true)
            {
                Page page = db.Pages.Find(id);
                if (page == null)
                {
                    return HttpNotFound();
                }
                return View(page);
            }
            else
            {
                return HttpNotFound();
            }
        }

        //
        // POST: /Page/Edit/5

        [HttpPost]
        public ActionResult Edit(Page page)
        {
            string username = User.Identity.Name;
            Utils Utils = new Utils();
            if (Utils.UserAdmin(username) == true)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(page).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(page);
            }
            else
            {
                return HttpNotFound();
            }
        }

        //
        // GET: /Page/Delete/5

        public ActionResult Delete(int id = 0)
        {
            string username = User.Identity.Name;
            Utils Utils = new Utils();
            if (Utils.UserAdmin(username) == true)
            {
                Page page = db.Pages.Find(id);
                if (page == null)
                {
                    return HttpNotFound();
                }
                return View(page);
            }
            else
            {
                return HttpNotFound();
            }
        }

        //
        // POST: /Page/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Page page = db.Pages.Find(id);
            db.Pages.Remove(page);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
