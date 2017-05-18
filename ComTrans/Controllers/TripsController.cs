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
    public class TripsController : Controller
    {
        private ComTransEntities1 db = new ComTransEntities1();

        // GET: Trips
        public ActionResult Index()
        {
            var trips = db.Trips.Include(t => t.Car).Include(t => t.Driver);
            return View(trips.ToList());
        }

        // GET: Trips/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }

        // GET: Trips/Create
        public ActionResult Create(string ids)
        {
            ViewBag.ids = ids;
            ViewBag.CarId = new SelectList(db.Cars, "Id", "Name");
            ViewBag.DriverId = new SelectList(db.Drivers, "Id", "Name");
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DriverId,CarId,Route,Date")] Trip trip, string ids)
        {
            string[] idsString = ids.Split(',');
            List<int> orderIds = new List<int>();
            int cargoesWeight = 0;
            foreach (var k in idsString)
            {
                int o = int.Parse(k);
                orderIds.Add(o);
                var ord = this.db.Orders.Where(x => x.Id == o).First();
                cargoesWeight += ord.Cargoes.Sum(x => x.Weight).Value;
            }

            int carCapacity = this.db.Cars.Where(x=>x.Id == trip.CarId).First().Capacity;

            if (cargoesWeight > carCapacity)
            {
                ModelState.AddModelError("", "Груз превышает грузоподъемность автомобиля");
                ViewBag.CarId = new SelectList(db.Cars, "Id", "Name");
                ViewBag.DriverId = new SelectList(db.Drivers, "Id", "Name");
                ViewBag.ids = ids;
                return View(trip);
            }

            var car = this.db.Cars.Where(x => x.Id == trip.CarId).First();
            if (car.Trips.Count > 0)
            {
                ModelState.AddModelError("", "Выбранный автомобиль занят на другом рейсе");
                ViewBag.CarId = new SelectList(db.Cars, "Id", "Name");
                ViewBag.DriverId = new SelectList(db.Drivers, "Id", "Name");
                ViewBag.ids = ids;
                return View(trip);
            }

            var driver = this.db.Drivers.Where(x => x.Id == trip.DriverId).First();
            if (driver.Trips.Count >0)
            {
                ModelState.AddModelError("", "Выбранный водитель занят на другом рейсе");
                ViewBag.CarId = new SelectList(db.Cars, "Id", "Name");
                ViewBag.DriverId = new SelectList(db.Drivers, "Id", "Name");
                ViewBag.ids = ids;
                return View(trip);
            }

            

            if (ModelState.IsValid)
            {
                db.Trips.Add(trip);
                db.SaveChanges();

                string[] idss = ids.Split(',');
                foreach (var id in idss)
                {
                    int curId = int.Parse(id);
                    var order = db.Orders.Where(x => x.Id ==curId).First();
                    foreach (var address in order.Addresses)
                    {
                        TripAddress ta = new TripAddress();
                        //ta.Address = address;
                        ta.AddressId = address.Id;
                        ta.TripId = trip.Id;
                        db.TripAddresses.Add(ta);
                        db.SaveChanges();
                    }  
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CarId = new SelectList(db.Cars, "Id", "Name", trip.CarId);
            ViewBag.DriverId = new SelectList(db.Drivers, "Id", "Name", trip.DriverId);
            return View(trip);
        }

        // GET: Trips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            ViewBag.CarId = new SelectList(db.Cars, "Id", "Name", trip.CarId);
            ViewBag.DriverId = new SelectList(db.Drivers, "Id", "Name", trip.DriverId);
            return View(trip);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DriverId,CarId,Route,Date")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CarId = new SelectList(db.Cars, "Id", "Name", trip.CarId);
            ViewBag.DriverId = new SelectList(db.Drivers, "Id", "Name", trip.DriverId);
            return View(trip);
        }

        // GET: Trips/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trip trip = db.Trips.Find(id);
            db.TripAddresses.RemoveRange(trip.TripAddresses);
            db.Trips.Remove(trip);
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
