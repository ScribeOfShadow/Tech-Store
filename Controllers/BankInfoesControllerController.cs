using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookStore.Models;
using Undisclosed_Shop.Models;

namespace Undisclosed_Shop.Controllers
{
    public class BankInfoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankInfoes
        public ActionResult Index()
        {
            return View(db.BankInfos.ToList());
        }

        // GET: BankInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankInfo bankInfo = db.BankInfos.Find(id);
            if (bankInfo == null)
            {
                return HttpNotFound();
            }
            return View(bankInfo);
        }

        // GET: BankInfoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BankInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BankName,BranchCode,Address,City,BankCode")] BankInfo bankInfo)
        {
            if (ModelState.IsValid)
            {
                db.BankInfos.Add(bankInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bankInfo);
        }

        // GET: BankInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankInfo bankInfo = db.BankInfos.Find(id);
            if (bankInfo == null)
            {
                return HttpNotFound();
            }
            return View(bankInfo);
        }

        // POST: BankInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankName,BranchCode,Address,City,BankCode")] BankInfo bankInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bankInfo);
        }

        // GET: BankInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankInfo bankInfo = db.BankInfos.Find(id);
            if (bankInfo == null)
            {
                return HttpNotFound();
            }
            return View(bankInfo);
        }

        // POST: BankInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankInfo bankInfo = db.BankInfos.Find(id);
            db.BankInfos.Remove(bankInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
