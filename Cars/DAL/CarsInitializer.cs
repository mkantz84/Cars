using Cars.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cars.DAL
{
    public class CarsInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<RentalContext>
    {
        protected override void Seed(RentalContext context)
        {
            var stores = new List<Store>
            {
            new Store{StoreID=1,Address="Haifa",Location="Haifa",StoreName="Haifa" },
            new Store{ StoreID=2,Address="tel aviv",Location="Haifa",StoreName="Haifa"},
            new Store{ StoreID=3,Address="jerusalem",Location="Haifa",StoreName="Haifa"},
            };

            stores.ForEach(s => context.stores.Add(s));
            context.SaveChanges();
            var carTypes = new List<CarType>
            {
            new CarType{CarTypeID=1,ModelName="Golf",ManifacturerName="Wolkswagen",DailyPrice=30,LateDayPrice=40,Gear=GearType.MANUAL,Year=2016},
            new CarType{CarTypeID=2,ModelName="Ibiza",ManifacturerName="Seat",DailyPrice=25,LateDayPrice=35,Gear=GearType.AUTOMATIC,Year=2016},
            new CarType{CarTypeID=3,ModelName="Fabia",ManifacturerName="Skoda",DailyPrice=30,LateDayPrice=45,Gear=GearType.AUTOMATIC,Year=2016},
            new CarType{CarTypeID=4,ModelName="S",ManifacturerName="Mercedes",DailyPrice=60,LateDayPrice=80,Gear=GearType.AUTOMATIC,Year=2015},
            };
            carTypes.ForEach(s => context.CarTypes.Add(s));
            context.SaveChanges();
            var cars = new List<Car>
            {
            new Car{CarNumber=1999667,CarTypeID=3,IsAvailable=true,Kilometer=80000,IsProper=true,StoreID=1},
            new Car{CarNumber=2022233,CarTypeID=2,IsAvailable=true,Kilometer=66500,IsProper=true,StoreID=1},
            new Car{CarNumber=5566677,CarTypeID=3,IsAvailable=true,Kilometer=80000,IsProper=true,StoreID=1},
            new Car{CarNumber=1122233,CarTypeID=3,IsAvailable=true,Kilometer=70000,IsProper=true,StoreID=1},
            new Car{CarNumber=2233344,CarTypeID=1,IsAvailable=true,Kilometer=96700,IsProper=true,StoreID=1},
            new Car{CarNumber=3344455,CarTypeID=1,IsAvailable=false,Kilometer=55000,IsProper=true,StoreID=1},
            new Car{CarNumber=7788899,CarTypeID=3,IsAvailable=true,Kilometer=150000,IsProper=false,StoreID=1 },
            new Car{CarNumber=6677788,CarTypeID=4,IsAvailable=true,Kilometer=80000,IsProper=true,StoreID=1},
            new Car{CarNumber=9999999,CarTypeID=2,IsAvailable=true,Kilometer=90000,IsProper=true,StoreID=1},
            };
            cars.ForEach(s => context.Cars.Add(s));
            context.SaveChanges();
            var users = new List<User>
            {
                new User{Email="kantzmichael@gmail.com",FullName="michael kantz",IsEmployee=true,IsManager=true,Password="titi123",Sex=SexType.MALE,UserName="titi"},
                new User{Email="kantzmichael@gmail.com",FullName="noa halperin",IsEmployee=false,IsManager=false,Password="noa123",Sex=SexType.FEMALE,UserName="noa"},
            };
            users.ForEach(s => context.Users.Add(s));
            context.SaveChanges();
        }
    }
}