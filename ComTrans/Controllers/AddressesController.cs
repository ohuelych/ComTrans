using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ComTrans.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ComTrans.Controllers
{
    public class AddressesController : Controller
    {
        private ComTransEntities1 db = new ComTransEntities1();

        // GET: Addresses
        public ActionResult Index()
        {
            var addresses = db.Addresses.Include(a => a.Order);
            return View(addresses.ToList());
        }

        // GET: Addresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Addresses.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // GET: Addresses/Create
        public ActionResult Create(int? orderId)
        {
            if (orderId.HasValue)
            {
                ViewBag.currentOrderId = orderId.Value;
            }
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "Id");
            return View();
        }

        // POST: Addresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Address1,OrderId,Coord")] Address address)
        {
            if (ModelState.IsValid)
            {
                Task<string> t = GetCoord(address.Address1);
                await t;
                address.Coord = t.Result;
                db.Addresses.Add(address);

                db.SaveChanges();
                return RedirectToAction("Index", "Orders");
                //return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(db.Orders, "Id", "Id", address.OrderId);
            return View(address);
        }

        // GET: Addresses/Edit/5
        public ActionResult Edit(int? id, int? orderId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Addresses.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            ViewBag.currentOrderId = orderId.HasValue ? orderId.Value : 0;
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "Id", address.OrderId);
            return View(address);
        }

        // POST: Addresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Address1,OrderId,Coord")] Address address, int currentOrderId)
        {
            if (ModelState.IsValid)
            {
                Task<string> t = GetCoord(address.Address1);
                await t;
                address.Coord = t.Result;
                db.Entry(address).State = EntityState.Modified;
                db.SaveChanges();
                if (currentOrderId > 0)
                {
                    return RedirectToAction("Edit", "Orders", new { Id = currentOrderId });
                }
                return RedirectToAction("Index");
            }
            ViewBag.OrderId = new SelectList(db.Orders, "Id", "Id", address.OrderId);
            return View(address);
        }

        // GET: Addresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Addresses.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // POST: Addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Address address = db.Addresses.Find(id);
            db.TripAddresses.RemoveRange(address.TripAddresses);
            db.Addresses.Remove(address);
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

        private async Task<string> GetCoord(string address)
        {
            /*HttpWebRequest wr = new HttpWebRequest();
            string URL = string.Format("https://geocode-maps.yandex.ru/1.x/?format=json&geocode={0}&results=1", HttpUtility.HtmlEncode(address));
            wr.Address. = new Uri(URL);*/

            string result = "";
            HttpClient client = new HttpClient();
            string URL = string.Format("https://geocode-maps.yandex.ru/1.x/?format=json&geocode={0}&results=1", HttpUtility.HtmlEncode(address));
            Task<string> resp = client.GetStringAsync(URL);
            await resp;
            
            if (!string.IsNullOrEmpty(resp.Result))
            {
                try
                {
                    result = JObject.Parse(resp.Result)["response"]["GeoObjectCollection"]["featureMember"][0]["GeoObject"]["Point"]["pos"].ToString();
                }
                catch (Exception) { }
                
                
                //return resp.Result;
                
            }
            return result;

            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URL);
            //HttpResponseMessage response = client.GetStringAsync()
        }
    }
}
