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
        private static CarFilter carFilter = new CarFilter();


        // GET: Cars
        public ActionResult Index()
        {
            string id = System.Web.HttpContext.Current.User.Identity.Name;
            if (id != "")
            {
                User authUser = db.Users.Find(id);
                Session["authname"] = authUser.UserName;
                if (authUser.IsManager)
                {
                    Session["authrole"] = "Manager";
                }
                else if (authUser.IsEmployee)
                {
                    Session["authrole"] = "Employee";
                }
                else
                {
                    Session["authrole"] = "Customer";
                }
            }

            //ViewBag.authid = "empty";
            //string id=System.Web.HttpContext.Current.User.Identity.Name;            
            //if (id != "")
            //{
            //    ViewBag.authid = id;
            //    ViewBag.username = db.Users.Find(id).UserName;
            //}
            return View();
        }

        public ActionResult UsersList()
        {
            return View(db.Users.ToList());
        }

        public ActionResult ShowCar(string manifacturer, string model)
        {
            ViewBag.years = db.CarTypes.Select(t => t.Year).Distinct();
            ViewBag.mani = db.CarTypes.Select(t => t.ManifacturerName).Distinct();
            ViewBag.model = db.CarTypes.Select(t => t.ModelName).Distinct();
            CarType carType = db.CarTypes.FirstOrDefault
                (t => t.ManifacturerName == manifacturer && t.ModelName == model);
            return View(carType);
        }

        public ActionResult CarsList(int skip = 1, int take = 3)
        {
            string userId = System.Web.HttpContext.Current.User.Identity.Name;
            List<CarType> carTypes = new List<CarType>();
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

            int carsNum = db.CarTypes.Count();

            carTypes = db.CarTypes.OrderBy(t => t.ManifacturerName).ToList();
            if (carFilter.Gear != GearType.DEFAULT)
            {
                carTypes = carTypes.
                    Where(t => t.Gear == carFilter.Gear).
                    ToList();
            }
            if (carFilter.Year != 0)
            {
                carTypes = carTypes.
                    Where(w => w.Year == carFilter.Year).
                    ToList();
            }
            if (carFilter.Manifacturer != "" && carFilter.Manifacturer != null)
            {
                carTypes = carTypes.
                    Where(w => w.ManifacturerName == carFilter.Manifacturer).
                    ToList();
            }
            if (carFilter.Model != "" && carFilter.Model != null)
            {
                carTypes = carTypes.
                    Where(w => w.ModelName == carFilter.Model).
                    ToList();
            }
            if (carFilter.FreeText != "" && carFilter.FreeText != null)
            {
                carTypes = carTypes.
                    Where(s => s.ManifacturerName.ToLower().Contains(carFilter.FreeText.ToLower()) 
                    || (s.ModelName.ToLower().Contains(carFilter.FreeText.ToLower()))).
                    ToList();

            }
            carsNum = carTypes.Count;
            carTypes = carTypes.
                Skip((skip - 1) * take).
                Take(take).
                ToList();
            //carsNum = carTypes.Count;
            ViewBag.carsNum = carsNum;
            ViewBag.years = db.CarTypes.Select(t => t.Year).Distinct();
            ViewBag.mani = db.CarTypes.Select(t => t.ManifacturerName).Distinct();
            ViewBag.model = db.CarTypes.Select(t => t.ModelName).Distinct();
            ViewBag.current = skip;
            if (carsNum % take == 0)
            {
                ViewBag.pages = (carsNum / take);
            }
            else
            {
                ViewBag.pages = (carsNum / take) + 1;
            }

            //return View(cars.Where(t => t.IsAvailable && t.IsProper).ToList());
            return View(carTypes);
        }

        public void InitFilter(GearType filterGear, int filterYear, string manifacturer, string model, string freeText)
        {
            carFilter.Gear = filterGear;
            carFilter.Year = filterYear;
            carFilter.Manifacturer = manifacturer;
            carFilter.Model = model;
            carFilter.FreeText = freeText;
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
            Car selectedCar = new Car();
            List<Rental> rentals =
                db.Rentals.Where(t => (t.StartDate > start && t.StartDate < end) || (t.StartDate < start && t.EndDate > start)).ToList();
            List<Car> unavalaibleCars = rentals.Select(t => t.Car).ToList();
            foreach (var item in db.Cars)
            {
                if (item.CarTypeID == id)
                {
                    selectedCar = item;
                    if (!unavalaibleCars.Exists(c => c.ID == selectedCar.ID))
                    {
                        break;
                    }
                }
            }

            ViewBag.carNumber = selectedCar.CarNumber;
            CarType carType = db.CarTypes.Find(id);
            CarInfo carinfo = new CarInfo
            {
                Car = selectedCar,
                CarType = carType,
                HireDateStart = start,
                HireDateEnd = end,
            };
            if (carType == null)
            {
                return HttpNotFound();
            }
            return View(carinfo);
        }

        public ActionResult UserDetails(string id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [Authorize]
        public ActionResult Purchase(int? id)
        {
            string userId = System.Web.HttpContext.Current.User.Identity.Name;
            Car car = db.Cars.Find(id);
            //car.HireDateStart = start;
            //car.HireDateEnd = end;
            Rental rental = new Rental();

            rental.Car = car;
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
            //if (db.Users.FirstOrDefault(t=>t.UserID==userId).IsManager)
            //{
            //    return View("ManagerOrdersList");
            //}
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
            User user = db.Users.FirstOrDefault(t => t.UserName == userName && t.Password == password);

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.UserID.ToString(), false);
                Session["authname"] = userName;

                if (user.IsManager)
                {
                    Session["authrole"] = "Manager";
                }
                else if (user.IsEmployee)
                {
                    Session["authrole"] = "Employee";
                }
                else
                {
                    Session["authrole"] = "Customer";
                }
                if (returnUrl == "" || returnUrl == null)
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
            string userId = System.Web.HttpContext.Current.User.Identity.Name;

            if (userId != "")
            {
                User user = db.Users.Find(userId);
                if (user.IsManager)
                {
                    return View("ManagerCreateUser");
                }
                return Redirect("/cars/index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(User user)
        {
            string userId = System.Web.HttpContext.Current.User.Identity.Name;
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                if (userId == "")
                {
                    FormsAuthentication.SetAuthCookie(user.UserID, false);
                }
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

        public ActionResult ManagerUsersList()
        {
            return View(db.Users.ToList());
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

        // GET: Cars/EditUser/5
        public ActionResult EditUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CarTypeID = new SelectList(db.CarTypes, "CarTypeID", "ModelName", car.CarTypeID);
            //ViewBag.StoreID = new SelectList(db.stores, "StoreID", "StoreName", car.StoreID);
            return View(user);
        }

        // POST: Cars/EditUser/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "UserID,FullName,UserName,BirthDate,Sex,Email,Password,IsEmployee,IsManager")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.CarTypeID = new SelectList(db.CarTypes, "CarTypeID", "ModelName", car.CarTypeID);
            //ViewBag.StoreID = new SelectList(db.stores, "StoreID", "StoreName", car.StoreID);
            return View(user);
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

        // GET: Cars/DeleteUser/5
        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Cars/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ManagerOrderssList()
        {
            return View(db.Rentals.ToList());
        }

        // GET: Cars/EditOrder/5
        public ActionResult EditOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CarTypeID = new SelectList(db.CarTypes, "CarTypeID", "ModelName", car.CarTypeID);
            //ViewBag.StoreID = new SelectList(db.stores, "StoreID", "StoreName", car.StoreID);
            return View(rental);
        }

        // POST: Cars/EditOrder/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder(Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rental).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.CarTypeID = new SelectList(db.CarTypes, "CarTypeID", "ModelName", car.CarTypeID);
            //ViewBag.StoreID = new SelectList(db.stores, "StoreID", "StoreName", car.StoreID);
            return View(rental);
        }

        public ActionResult OrderDetails(int? id)
        {
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // GET: Cars/Delete/5
        public ActionResult DeleteOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("DeleteOrder")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOrderConfirmed(int id)
        {
            Rental rental = db.Rentals.Find(id);
            db.Rentals.Remove(rental);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateOrder()
        {
            ViewBag.users = db.Users.ToList();
            ViewBag.cars = db.Cars.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder(Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Rentals.Add(rental);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rental);
        }

        public ActionResult ManagerCarTypesList()
        {
            return View(db.CarTypes.ToList());
        }

        // GET: Cars/EditCarType/5
        public ActionResult EditCarType(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarType carType = db.CarTypes.Find(id);
            if (carType == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CarTypeID = new SelectList(db.CarTypes, "CarTypeID", "ModelName", car.CarTypeID);
            //ViewBag.StoreID = new SelectList(db.stores, "StoreID", "StoreName", car.StoreID);
            return View(carType);
        }

        // POST: Cars/EditCarType/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCarType(CarType carType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(carType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(carType);
        }

        public ActionResult CarTypeDetails(int? id)
        {
            CarType carType = db.CarTypes.Find(id);
            if (carType == null)
            {
                return HttpNotFound();
            }
            return View(carType);
        }

        // GET: Cars/DeleteCarType/5
        public ActionResult DeleteCarType(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarType carType = db.CarTypes.Find(id);
            if (carType == null)
            {
                return HttpNotFound();
            }
            return View(carType);
        }

        // POST: Cars/DeleteCarType/5
        [HttpPost, ActionName("DeleteCarType")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCarTypeConfirmed(int id)
        {
            CarType carType = db.CarTypes.Find(id);
            db.CarTypes.Remove(carType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateCarType()
        {
            //ViewBag.users = db.Users.ToList();
            //ViewBag.cars = db.Cars.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCarType(CarType carType)
        {
            if (ModelState.IsValid)
            {
                db.CarTypes.Add(carType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(carType);
        }
    }
}
