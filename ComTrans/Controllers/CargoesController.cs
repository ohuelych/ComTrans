using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ComTrans.Models;

namespace ComTrans.Controllers
{
    public class CargoesController : Controller
    {
        private ComTransEntities1 db = new ComTransEntities1();

        // GET: Cargoes
        public ActionResult Index()
        {
            var cargoes = db.Cargoes.Include(c => c.Order);
            return View(cargoes.ToList());
        }

        // GET: Cargoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cargo cargo = db.Cargoes.Find(id);
            if (cargo == null)
            {
                return HttpNotFound();
            }
            return View(cargo);
        }

        // GET: Cargoes/Create
        public ActionResult Create(int? orderId)
        {
            if (orderId.HasValue)
            {
                ViewBag.currentOrderId = orderId.Value;
            }
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "Id");
            return View();
        }

        // POST: Cargoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Price,Count,Weight,OrderId")] Cargo cargo)
        {
            if (ModelState.IsValid)
            {
                db.Cargoes.Add(cargo);
                db.SaveChanges();
                return RedirectToAction("Create", "Addresses", new { orderId = cargo.OrderId });
                //return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(db.Orders, "Id", "Id", cargo.OrderId);
            return View(cargo);
        }

        // GET: Cargoes/Edit/5
        public ActionResult Edit(int? id, int? orderId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cargo cargo = db.Cargoes.Find(id);
            if (cargo == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "Id", cargo.OrderId);
            ViewBag.currentOrderId = orderId.HasValue ? orderId.Value : 0;
            return View(cargo);
        }

        // POST: Cargoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Price,Count,Weight,OrderId")] Cargo cargo, int orderId)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cargo).State = EntityState.Modified;
                db.SaveChanges();
                if (orderId > 0)
                {
                    return RedirectToAction("Edit", "Orders", new { Id = orderId });
                }
                return RedirectToAction("Index");
            }
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "Id", cargo.OrderId);
            return View(cargo);
        }

        // GET: Cargoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cargo cargo = db.Cargoes.Find(id);
            if (cargo == null)
            {
                return HttpNotFound();
            }
            return View(cargo);
        }

        // POST: Cargoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cargo cargo = db.Cargoes.Find(id);
            db.Cargoes.Remove(cargo);
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
