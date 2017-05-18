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
    public class TripAddressesController : Controller
    {
        private ComTransEntities1 db = new ComTransEntities1();

        // GET: TripAddresses
        public ActionResult Index()
        {
            var tripAddresses = db.TripAddresses.Include(t => t.Address).Include(t => t.Trip);
            return View(tripAddresses.ToList());
        }

        // GET: TripAddresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TripAddress tripAddress = db.TripAddresses.Find(id);
            if (tripAddress == null)
            {
                return HttpNotFound();
            }
            return View(tripAddress);
        }

        // GET: TripAddresses/Create
        public ActionResult Create()
        {
            ViewBag.AddressId = new SelectList(db.Addresses, "Id", "Address1");
            ViewBag.TripId = new SelectList(db.Trips, "Id", "Route");
            return View();
        }

        // POST: TripAddresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AddressId,TripId,Num")] TripAddress tripAddress)
        {
            if (ModelState.IsValid)
            {
                db.TripAddresses.Add(tripAddress);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AddressId = new SelectList(db.Addresses, "Id", "Address1", tripAddress.AddressId);
            ViewBag.TripId = new SelectList(db.Trips, "Id", "Route", tripAddress.TripId);
            return View(tripAddress);
        }

        // GET: TripAddresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TripAddress tripAddress = db.TripAddresses.Find(id);
            if (tripAddress == null)
            {
                return HttpNotFound();
            }
            ViewBag.AddressId = new SelectList(db.Addresses, "Id", "Address1", tripAddress.AddressId);
            ViewBag.TripId = new SelectList(db.Trips, "Id", "Route", tripAddress.TripId);
            return View(tripAddress);
        }

        // POST: TripAddresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AddressId,TripId,Num")] TripAddress tripAddress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tripAddress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AddressId = new SelectList(db.Addresses, "Id", "Address1", tripAddress.AddressId);
            ViewBag.TripId = new SelectList(db.Trips, "Id", "Route", tripAddress.TripId);
            return View(tripAddress);
        }

        // GET: TripAddresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TripAddress tripAddress = db.TripAddresses.Find(id);
            if (tripAddress == null)
            {
                return HttpNotFound();
            }
            return View(tripAddress);
        }

        // POST: TripAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TripAddress tripAddress = db.TripAddresses.Find(id);
            db.TripAddresses.Remove(tripAddress);
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
