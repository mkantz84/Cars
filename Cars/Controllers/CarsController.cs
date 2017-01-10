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
using System.Web.Security;

namespace Cars.Controllers
{
    public class CarsController : Controller
    {
        private RentalContext db = new RentalContext();
        private static DateTime start;
        private static DateTime end;

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
            string userId = System.Web.HttpContext.Current.User.Identity.Name;

            if (userId != "")
            {
                bool isManager = db.Users.FirstOrDefault(t => t.UserID == userId).IsManager;
                if (isManager)
                {
                    return Redirect("/cars/managercarslist");
                }
            }
            var cars = db.Cars.Include(c => c.CarType).Include(c => c.Store);
            CarData carData = new CarData
            {
                Cars = cars.Where(t => t.IsAvailable && t.IsProper),
            };
            return View(cars.Where(t => t.IsAvailable && t.IsProper).ToList());
        }

        public void InitStartDate(DateTime startDate)
        {
            start = startDate;
        }

        public void InitEndDate(DateTime endDate)
        {
            end = endDate;
        }

        // GET: Cars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            //car.HireDateStart = start;
            //car.HireDateEnd = end;
            CarType carType = db.CarTypes.Find(car.CarTypeID);
            CarInfo carinfo = new CarInfo
            {
                Car = car,
                CarType = carType,
                HireDateStart = start,
                HireDateEnd = end,
            };
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(carinfo);
        }

        [Authorize]
        public ActionResult Purchase(int? id)
        {
            string userId = System.Web.HttpContext.Current.User.Identity.Name;
            Car car = db.Cars.Find(id);
            //car.HireDateStart = start;
            //car.HireDateEnd = end;
            Rental rental = new Rental();

            rental.CarNumber = car.CarNumber;
            rental.StartDate = start;
            rental.EndDate = end;
            //int userId= db.Users.FirstOrDefault(t => t.UserName == name).UserID;
            rental.UserId = userId;
            //Car=car,

            db.Rentals.Add(rental);
            db.SaveChanges();
            return View();
        }

        [Authorize]
        public ActionResult Purchases()
        {
            string userId = System.Web.HttpContext.Current.User.Identity.Name;
            //int userId = db.Users.FirstOrDefault(t => t.UserName == name).UserID;
            List<Rental> rentals = db.Rentals.Where(t => t.UserId == userId).ToList();
            return View(rentals);
        }

        [Authorize]
        public ActionResult Workers()
        {
            string userId = System.Web.HttpContext.Current.User.Identity.Name;
            User user = db.Users.FirstOrDefault(t => t.UserID == userId);
            if (user.IsManager)
            {
                return Redirect("/cars/manager");
            }
            else if (user.IsEmployee)
            {
                return Redirect("/cars/employee");
            }
            return Redirect("/cars/index");
        }

        public ActionResult employee()
        {
            return View();
        }

        public ActionResult manager()
        {
            return View();
        }

        public ActionResult ManagerCarsList()
        {
            return View(db.Cars.ToList());
        }

        public ActionResult returning(string userId, int carNumber)
        {
            Rental rental = db.Rentals.FirstOrDefault(t => t.UserId == userId && t.CarNumber == carNumber);
            if (rental != null)
            {
                CarType carType = db.CarTypes.FirstOrDefault(t => t.CarTypeID == db.Cars.FirstOrDefault(c => c.CarNumber == carNumber).CarTypeID);
                ViewBag.dailyPrice = carType.DailyPrice;
                ViewBag.latePrice = carType.LateDayPrice;
                return View(rental);
            }
            return Redirect("/cars/employees");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult returning(Rental rental)
        {
            if (ModelState.IsValid)
            {
                Rental rent = db.Rentals.Find(rental.RentalID);
                rent.ReturningDate = rental.ReturningDate;
                db.SaveChanges();
                return Redirect("/cars/thanks");
            }
            return RedirectToAction("/cars/returning", rental.UserId, rental.CarNumber);
        }

        public ActionResult thanks()
        {
            return View();
        }

        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(string userName, string password, string returnUrl)
        {
            User user = db.Users.FirstOrDefault(t => t.UserName == userName);

            if (user != null && user.Password == password)
            {
                FormsAuthentication.SetAuthCookie(user.UserID.ToString(), false);
                if (returnUrl == "")
                {
                    return Redirect("/cars/index");
                }
                return Redirect(returnUrl);
            }
            return View();
        }

        public ActionResult logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return Redirect("/cars/index");
        }

        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                FormsAuthentication.SetAuthCookie(user.UserName, false);
                return RedirectToAction("Index");
            }

            //ViewBag.CarTypeID = new SelectList(db.CarTypes, "CarTypeID", "ModelName", car.CarTypeID);
            //ViewBag.StoreID = new SelectList(db.stores, "StoreID", "StoreName", car.StoreID);
            return View(user);
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
        public ActionResult Edit([Bind(Include = "ID,CarNumber,Kilometer,IsProper,IsAvailable,CarTypeID,StoreID")] Car car)
        {
            if (ModelState.IsValid)
            {
                //Car editedCar = db.Cars.Find(car.ID);
                //editedCar = car;
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

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
