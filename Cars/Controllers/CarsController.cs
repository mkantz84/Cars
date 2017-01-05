using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cars.Models;
using Cars.DAL;

namespace Cars.Controllers
{
    public class CarsController : Controller
    {
        private RentalContext db = new RentalContext();

        // GET: Cars
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UsersList()
        {
            return View(db.Users.ToList());
        }

        public ActionResult CarsList()
        {
            var cars = db.Cars.Include(c => c.CarType).Include(c => c.Store);
            CarData carData = new CarData
            {
                Cars = cars.Where(t => t.IsAvailable && t.IsProper),
            };
            return View(cars.Where(t => t.IsAvailable && t.IsProper).ToList());
        }

        // GET: Cars/Details/5
        public ActionResult Details(int? id, DateTime startDate, DateTime endDate)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            CarType carType = db.CarTypes.Find(car.CarTypeID);
            CarInfo carinfo = new CarInfo { Car = car, CarType = carType };
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(carinfo);
        }

        [Authorize]
        public ActionResult Purchase(int carNumber)
        {
            return View();
        }

        // GET: Cars/Create
        public ActionResult Create()
        {
            ViewBag.CarTypeID = new SelectList(db.CarTypes, "CarTypeID", "ModelName");
            ViewBag.StoreID = new SelectList(db.stores, "StoreID", "StoreName");
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CarNumber,Kilometer,IsProper,IsAvailable,CarTypeID,StoreID")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Cars.Add(car);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CarTypeID = new SelectList(db.CarTypes, "CarTypeID", "ModelName", car.CarTypeID);
            ViewBag.StoreID = new SelectList(db.stores, "StoreID", "StoreName", car.StoreID);
            return View(car);
        }

        // GET: Cars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            ViewBag.CarTypeID = new SelectList(db.CarTypes, "CarTypeID", "ModelName", car.CarTypeID);
            ViewBag.StoreID = new SelectList(db.stores, "StoreID", "StoreName", car.StoreID);
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CarNumber,Kilometer,IsProper,IsAvailable,CarTypeID,StoreID")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CarTypeID = new SelectList(db.CarTypes, "CarTypeID", "ModelName", car.CarTypeID);
            ViewBag.StoreID = new SelectList(db.stores, "StoreID", "StoreName", car.StoreID);
            return View(car);
        }

        // GET: Cars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Car car = db.Cars.Find(id);
            db.Cars.Remove(car);
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
