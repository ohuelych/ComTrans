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
    public class OrdersController : Controller
    {
        private ComTransEntities1 db = new ComTransEntities1();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Client);
            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create(int? clientId)
        {
            if (clientId.HasValue)
            {
                ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", clientId.Value);
            }
            else
            {
                ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name");
            }
            //ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name",clientId.Value);
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ClientId,Date")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                //ViewBag.currentOrderId = order.Id;
                return RedirectToAction("Create", "Cargoes", new { orderId = order.Id });
            }

            ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", order.ClientId);
            return View();
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", order.ClientId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ClientId,Date")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", order.ClientId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);

            var trips = order.Addresses.Select(x => x.TripAddresses);

            foreach (var trip in trips)
            {
                db.TripAddresses.RemoveRange(trip);
            }

            db.Addresses.RemoveRange(order.Addresses);
            db.Cargoes.RemoveRange(order.Cargoes);
            db.Orders.Remove(order);
            
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
