using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CarsProj
{
    class Program
    {
        static void Main(string[] args)
        {

        }
    }

    class Manufacturer
    {
        public int test = 6;
        public int test2 = 66;
        public int test3 = 666;
        private const byte maxCars = 10;
        private byte totalNumCars = 0;

        private string _name;
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrEmpty(value) ? "Default" : value;
        }

        private string _country;
        public string Country
        {
            get => _country;
            set => _country = string.IsNullOrEmpty(value) ? "Default" : value;
        }

        private List<Car> _cars = new List<Car>();
        public List<Car> Cars => _cars;

        public Manufacturer(string Name, string country)
        {
            _name = Name;
            _country = country;
        }

        public Lorry CreateNewCar(Models model, int maxSpeed, int minSpeed, string name, string number)
        {
            if (totalNumCars == 10)
                throw new CarManufacturerLimitExceeded("Car limit exceeded!");
            ++totalNumCars;
            var car = new Lorry(model, maxSpeed, minSpeed, name, number);
            _cars.Add(car); 
            return car;
        }

        public void DeleteCarById(int id) => _cars.Remove(_cars.First(c => c.Id == id));
    }

    


    abstract class Car
    {
        protected static int _totalId = 0;

        public int MaxSpeed { get; protected set; }
        public int MinSpeed { get; protected set; }

        public  Models Model { get; protected set; }

        public int Id { get; protected set; }

        public int TotalSpeed { get; protected set; } = 0;

        public DateTime Date { get; } = DateTime.Now;

        private string _name;
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrEmpty(value) ? "Default" : value;

        }

        private string _number;
        public string Number
        {
            get => _number;
            set => _number = string.IsNullOrEmpty(value) ? "Default" : value;
        }

      /*  public Car(Models model, int maxSpeed, int minSpeed, string name, string number)
        {
            Id = ++_totalId;
            MaxSpeed = maxSpeed;
            _name = name;
            _number = number;
            Model = model;
        } */

        public virtual void Bip() => Console.WriteLine("Bip-Bip");

        public virtual void SpeedUpWithTime(int sec)
        {
            TotalSpeed = (int)(TotalSpeed * 0.015 * sec);
            if(TotalSpeed> MaxSpeed) throw new SpeedLimitExceeded("Speed limit exceeded!", this);
        } 

        public virtual void SpeedDownWithTime(int sec)
        {
            TotalSpeed = (int)(TotalSpeed * 0.015*sec);
            if (TotalSpeed < MinSpeed) throw new SpeedLimitExceeded("Speed limit violated!", this);
        }

        public void Brake()
        {
            int step =(int)(TotalSpeed*0.015);
            for(int i=0;i<10;i++)
            {
                // здесь задержка
                TotalSpeed -= step;
            }
        }

        public void Go()
        {
            int step = (int)(TotalSpeed * 0.015);
            for (int i = 0; i < 10; i++)
            {
                // здесь задержка
                TotalSpeed += step ;
            }
        }
    }

    class Lorry : Car
    {
        public bool DrawTrailer { get; private set; }
        private int DegreeOfDischarge { get; set; } = 0;

        public Lorry(Models model, int maxSpeed, int minSpeed, string name, string number) 
        {
            base.Id = ++_totalId;
            MaxSpeed = maxSpeed;
            Name = name;
            Number = number;
            Model = model;
        }

        public override void SpeedUpWithTime(int sec)
        {
            TotalSpeed = (int)(TotalSpeed * 0.01 * sec);
            if (TotalSpeed > MaxSpeed) throw new SpeedLimitExceeded("Speed limit exceeded!", this);
        }

        public override void SpeedDownWithTime(int sec)
        {
            TotalSpeed = (int)(TotalSpeed * 0.01 * sec);
            if (TotalSpeed < MinSpeed) throw new SpeedLimitExceeded("Speed limit violated!", this);
        }

        public void WipersOn() => DrawTrailer = true;
        public void WipersOff() => DrawTrailer = false;

        public void DischargeOn()
        {
            DegreeOfDischarge = DegreeOfDischarge+=5;
            if (TotalSpeed > 100) throw new SpeedLimitExceeded("Discharge limit exceeded!", this);
        }

        public void DischargeOff(int sec)
        {
            DegreeOfDischarge = DegreeOfDischarge -= 5;
            if (DegreeOfDischarge < 0) throw new SpeedLimitExceeded("Discharge limit violated!", this);
        }
    }

  

    class CarController
    {
        public Car Car;

        public CarController(Car car)
        {
            Car = car;
        }

        public void SpeedUpWithTime(int sec)
        {
            try
            {
                Car.SpeedUpWithTime(sec);
            }
            catch(SpeedLimitExceeded ex)
            {
                Console.WriteLine( ex.Message);
                ex.Car.Brake();
            }
            finally
            {
                Console.WriteLine("Current speed: " + Car.TotalSpeed);
            }
        }


        public void SpeedDownWithTime(int sec)
        {
            try
            {
                Car.SpeedDownWithTime(sec);
            }
            catch (SpeedLimitExceeded ex)
            {
                Console.WriteLine(ex.Message);
                ex.Car.Go();
            }
            finally
            {
                Console.WriteLine("Current speed: " + Car.TotalSpeed);
            }
        }

        public void Bip() => Car.Bip();
    }

    class User
    {
        private static int _totalId = 0;
        public int Id { get; }

        private List<Car> _cars = new List<Car>();
        public List<Car> Cars => _cars;

        private string _name;
        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrEmpty(value) ? "Default" : value;
        }

        private CarController _carController { get; set; }

        public User(string name)
        {
            _name = name;
        }

        public void AddNewCarByManufacturer(Manufacturer manuf, Models model, int maxSpeed, int minSpeed, string name, string number)
        {
            try
            {
                var car = manuf.CreateNewCar(model, maxSpeed, minSpeed, name, number);
                _cars.Add(car);
                _carController  = new CarController(car);
            }
            catch (CarManufacturerLimitExceeded e)
            {
                Console.WriteLine(e);
            }
        }

        public void UpSpeed(int sec) => _carController.SpeedUpWithTime(sec);

        public void DownSpeed(int sec) => _carController.SpeedDownWithTime(sec);

        public void Bip() => _carController.Bip();
    }

    class CarManufacturerLimitExceeded : Exception
    {
        public CarManufacturerLimitExceeded(string message)
            : base(message)
        { }
    }

    class SpeedLimitExceeded : Exception
    {
        public Car Car { get; private set; }
        public SpeedLimitExceeded(string message, Car car)
            : base(message)
        {
            Car=car;
        }
    }

    enum Models
    {
        M6 = 0,
        M1,
        X1,
        X2,
        Z4,
        I8
    }
}
