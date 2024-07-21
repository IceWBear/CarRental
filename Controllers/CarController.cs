using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project_RentACar.DAO;
using Project_RentACar.Models;
using System.Diagnostics.Contracts;

namespace Project_RentACar.Controllers
{
    public class CarController : Controller
    {
        private readonly AccountDAO dao_user;
        private readonly FinanceDAO finance_dao;
        private readonly CarDAO car_dao;
        CarRentalDBContext context = new CarRentalDBContext();
        public CarController(AccountDAO accountDAO, FinanceDAO financeDAO, CarDAO carDAO)
        {
            dao_user = accountDAO;
            finance_dao = financeDAO;
            car_dao = carDAO;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var res = (from c in context.Cars
                       join cm in context.CarManufacturers
                       on c.ManufacturerId equals cm.ManufacturerId
                       select new
                       {
                           CarId = c.CarId,
                           LicensePlate = c.LicensePlate,
                           Model = c.Model,
                           Year = c.Year,
                           CarTypeId = c.CarTypeId,
                           BranchId = c.BranchId,
                           ManufacturerName = cm.ManufacturerName,
                           Image = c.Image,
                           RentalPrice = c.RentalPrice,
                           Transmission = c.Transmission,
                           Seats = c.Seats,
                           Fuel = c.Fuel,
                           Description = c.Description,
                           Luggage = c.Luggage,
                           Status = c.Status,
                       }).OrderBy(x => x.Model).ToList();
            return Ok(res);
        }

        public IActionResult LoadCar(string manufacturer, string carName, decimal? minRentalPrice, decimal? maxRentalPrice, int? seats, string transmission, string is_available, string fuel, string sortBy, string orderBy, string param)
        {
            Customer customer = HttpContext.Session.GetObject<Customer>("account");
            if (customer != null)
            {
                ViewBag.name = customer.LastName;
            }
            ViewBag.getAllManufacturer = car_dao.GetAllManufacturer();

            var cars = car_dao.FilterCars(manufacturer, carName, minRentalPrice, maxRentalPrice, seats, transmission, is_available, fuel);

            if (string.IsNullOrEmpty(sortBy) && string.IsNullOrEmpty(orderBy))
            {
                cars = cars.OrderBy(c => c.Model).ToList();
            }
            else if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(orderBy))
            {
                if (sortBy == "CarModel")
                {
                    cars = orderBy == "Ascending" ? cars.OrderBy(c => c.Model).ToList() : cars.OrderByDescending(c => c.Model).ToList();
                }
                else if (sortBy == "Price")
                {
                    cars = orderBy == "Ascending" ? cars.OrderBy(c => c.RentalPrice).ToList() : cars.OrderByDescending(c => c.RentalPrice).ToList();
                }
            }
            ViewBag.getAllCar = cars;

            // Send back search and sort parameters to the view
            ViewBag.manufacturer_sentback = manufacturer;
            ViewBag.transmission_sentback = transmission;
            ViewBag.fuel_sentback = fuel;
            ViewBag.carName_sentback = carName;
            ViewBag.minRentalPrice_sentback = minRentalPrice;
            ViewBag.maxRentalPrice_sentback = maxRentalPrice;
            ViewBag.seats_sentback = seats;
            ViewBag.isAvailable_sentback = is_available;
            ViewBag.sortBy_sentback = sortBy;
            ViewBag.orderBy_sentback = orderBy;
            ViewBag.param = param;

            return View("ShowCar");
        }


        public IActionResult ViewDetail(int car_id)
        {

            Customer customer = HttpContext.Session.GetObject<Customer>("account");
            if (customer != null)
            {
                ViewBag.name = customer.LastName;
                var check = car_dao.CheckRentalContractCompleted(car_id, customer.CustomerId);
                if (check == true)
                {
                    ViewBag.check = true;
                }
            }
            if (TempData.ContainsKey("car_id"))
            {
                car_id = (int)TempData["car_id"];
            }
            var contract_draft = HttpContext.Session.GetObject<RentalContract>("contract_draft");

            ViewBag.manufacturer = car_dao.GetCarManufacturerByID(car_id);
            ViewBag.car_detail = car_dao.getCarByID(car_id);
            ViewBag.related_car = car_dao.GetRelatedCar(car_id);
            ViewBag.getAllManufacturer = car_dao.GetAllManufacturer();
            ViewBag.contract_draft = contract_draft;
            ViewBag.account = customer;
            ViewBag.message = TempData["message"] as string;

            var feedback = car_dao.GetAllFeedback(car_id);
            if (feedback != null)
            {
                ViewBag.customer_feedback = dao_user.GetAllCustomer();
                ViewBag.feedback = feedback;
            }

            var rental_contract = car_dao.GetRentalContractByCarID(car_id);
            if (rental_contract != null)
            {
                ViewBag.end_date = rental_contract.EndDate.ToString("yyyy-MM-dd");
            }
            return View("CarDetail");
        }

        public IActionResult GetRentalInfo(int car_id, string fname, string lname, string email, string phone, DateTime start_date, DateTime end_date, string destination, string note)
        {
            Customer customer = HttpContext.Session.GetObject<Customer>("account");
            var car = car_dao.getCarByID(car_id);

            if (start_date >= DateTime.Now && end_date >= start_date && (start_date - DateTime.Now).TotalDays <= 2)
            {
                HttpContext.Session.Remove("contract_draft");
                TimeSpan duration = end_date - start_date;
                int numberOfDays = duration.Days;
                if (numberOfDays == 0)
                {
                    numberOfDays = 1;
                }
                decimal total = car.RentalPrice * numberOfDays;

                RentalContract contract = new RentalContract()
                {
                    CustomerId = customer.CustomerId,
                    CarId = car_id,
                    FirstName = fname,
                    LastName = lname,
                    Email = email,
                    Phone = phone,
                    StartDate = start_date,
                    EndDate = end_date,
                    Destination = destination,
                    TotalAmount = total,
                    Note = note
                };
                HttpContext.Session.SetObject("contract", contract);
                return RedirectToAction("PurchaseByCarRentalPAY", "Purchase");
            }
            else
            {
                TempData["message"] = "Error Date! Please try again.";
                TempData["car_id"] = car_id;

                RentalContract contract_draft = new RentalContract()
                {
                    FirstName = fname,
                    LastName = lname,
                    Email = email,
                    Phone = phone,
                    StartDate = start_date,
                    EndDate = end_date,
                    Destination = destination,
                    Note = note
                };
                HttpContext.Session.SetObject("contract_draft", contract_draft);
                return RedirectToAction("ViewDetail", new { tab = "pills-rental" });
            }
        }

        [HttpPost]
        public IActionResult AddFeedback(string feedback_content, int rating, int car_id)
        {
            Customer customer = HttpContext.Session.GetObject<Customer>("account");
            car_dao.AddFeedback(feedback_content, rating, customer.CustomerId, car_id);
            return RedirectToAction("ViewDetail", new { car_id = car_id, tab = "pills-review" });
        }

        public IActionResult DeleteFeedback(int feedback_id, int car_id)
        {
            car_dao.DeleteFeedback(feedback_id);
            return RedirectToAction("ViewDetail", new { car_id = car_id, tab = "pills-review" });
        }
    }
}
