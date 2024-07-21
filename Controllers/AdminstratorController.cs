using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Project_RentACar.DAO;
using Project_RentACar.Hubs;
using Project_RentACar.Models;
using System.Data.Entity;

namespace Project_RentACar.Controllers
{
    public class AdminstratorController : Controller
    {

        private readonly AccountDAO dao_user;
        private readonly FinanceDAO finance_dao;
        private readonly CarDAO car_dao;
        private readonly IHubContext<SignalR_Hub> _signalRHub;
        private readonly CarRentalDBContext _context;

        public AdminstratorController(AccountDAO accountDAO, FinanceDAO financeDAO, CarDAO carDAO, IHubContext<SignalR_Hub> signalRHub, CarRentalDBContext context)
        {
            dao_user = accountDAO;
            finance_dao = financeDAO;
            car_dao = carDAO;
            _signalRHub = signalRHub;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var res = (from c in _context.Cars
                       join cm in _context.CarManufacturers
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

        public IActionResult ManageCar()
        {
            ViewBag.getAllCar = car_dao.GetAllCar();
            ViewBag.getAllManufacturer = car_dao.GetAllManufacturer();
            return View();
        }

        public IActionResult AddCar()
        {
            ViewBag.hihi = car_dao.GetCarTypes();
            ViewBag.hehe = car_dao.GetBranch();
            ViewBag.huhu = car_dao.GetCarManu();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCar(Car car)
        {
            ViewBag.hihi = car_dao.GetCarTypes();
            ViewBag.hehe = car_dao.GetBranch();
            ViewBag.huhu = car_dao.GetCarManu();
            car_dao.AddCar(car);
            await _context.SaveChangesAsync();
            await _signalRHub.Clients.All.SendAsync("LoadProduct");
            //Tao object va truyen sang thanh cong
            return RedirectToAction(nameof(ManageCar));

        }

        public async Task<IActionResult> DeleteCar(int id)
        {
            if (TempData.ContainsKey("car_id"))
            {
                id = (int)TempData["car_id"];
            }
            car_dao.DeleteCar(id);
            await _context.SaveChangesAsync();
            await _signalRHub.Clients.All.SendAsync("LoadProduct");
            return RedirectToAction(nameof(ManageCar));
        }
        public IActionResult EditCar(int id)
        {
            ViewBag.hihi = car_dao.GetCarTypes();
            ViewBag.hehe = car_dao.GetBranch();
            ViewBag.huhu = car_dao.GetCarManu();
            var car = car_dao.getCarByID(id);
            ViewBag.car = car;
            return View();
        }

        [HttpPost]
        public IActionResult UpdateCarImage(IFormFile imageFile, int car_id)
        {
            var fileName = Path.GetFileName(imageFile.FileName);
            var account_raw = HttpContext.Session.GetObject<Customer>("account");
            car_dao.UpdateImageCar(car_id, fileName);
            return RedirectToAction("EditCar", new { id = car_id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(Car car)
        {
            ViewBag.hihi = car_dao.GetCarTypes();
            ViewBag.hehe = car_dao.GetBranch();
            ViewBag.huhu = car_dao.GetCarManu();
            car_dao.EditCar(car);
            await _context.SaveChangesAsync();
            await _signalRHub.Clients.All.SendAsync("LoadProduct");
            //Tao object va truyen sang thanh cong
            return RedirectToAction(nameof(ManageCar));
        }

        public IActionResult Dashboard()
        {
            ViewBag.db = (from Tra in _context.Transactions
                          join Cus in _context.Customers
                          on Tra.CustomerId equals Cus.CustomerId
                          select new
                          {
                              Id = Tra.Id,
                              CustomerName = Cus.FirstName + Cus.LastName,
                              IssueDate = Tra.IssueDate.ToString("yyyy/mm/dd"),
                              Action = Tra.Action,
                              Note = Tra.Note,
                          }).ToList();

            return View();
        }

        public async Task<IActionResult> GetdataIcebear()
        {
            return Json(car_dao.Getdata());
        }

        public IActionResult FeedBackReport()
        {
            ViewBag.db = (from feb in _context.Feedbacks
                          join car in _context.Cars
                          on feb.CarId equals car.CarId
                          join cus in _context.Customers
                          on feb.CustomerId equals cus.CustomerId
                          select new
                          {
                              Id = feb.FeedbackId,
                              CustomerName = cus.FirstName + cus.LastName,
                              CarModel = car.Model,
                              Content = feb.FeedbackContent,
                              Rating = feb.Rating,
                              Date = feb.FeedbackDate.ToString("yyyy/mm/dd")
                          }).ToList(); ;
            return View();
        }

        public async Task<IActionResult> GetFeedbackIcebear()
        {
            return Json(car_dao.GetAverageFeedback());
        }

        public IActionResult RentalReport()
        {
            ViewBag.db = (from ren in _context.RentalContracts
                          join car in _context.Cars
                          on ren.CarId equals car.CarId
                          select new
                          {
                              ContractId = ren.ContractId,
                              CustomerName = ren.FirstName + ren.LastName,
                              CarModel = car.Model,
                              Phone = ren.Phone,
                              Destination = ren.Destination,
                              Date = ren.StartDate.ToString("yyyy/MM/dd") + " - " + ren.EndDate.ToString("yyyy/MM/dd"),
                              Total = ren.TotalAmount,
                              Note = ren.Note,
                              Status = ren.Status,
                              CustomerId = ren.CustomerId,
                          }).ToList(); ;
            return View();
        }

        public async Task<IActionResult> GetRentalIcebear()
        {
            return Json(car_dao.GetRental());
        }

        public IActionResult ManageAccount()
        {
            ViewBag.user = dao_user.GetAllCustomer();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ManageAccount(int Id, string active)
        {
            ViewBag.user = dao_user.GetAllCustomer();
            SetActive(Id, active);
            return View();
        }
        public void SetActive(int id, string active)
        {
            if (id != null)
            {
                Customer c = dao_user.GetAccountByID(id);
                if (active == "true")
                {
                    c.IsActive = true;
                }
                else
                {
                    c.IsActive = false;
                }
                _context.SaveChanges();
            }
        }

        public IActionResult RentalCanceled(int rental_id, int customer_id)
        {
            var account = dao_user.GetAccountByID(customer_id);
            var rental_contract = finance_dao.GetRentalContractByID(rental_id);
            var transaction = finance_dao.GetTransactionByRentalId(rental_id);
            finance_dao.UpdateStatusRentalContract(rental_id);
            finance_dao.UpdateBalance(account.CustomerId, transaction.Action);
            finance_dao.AddTransaction2(account.CustomerId, transaction.Action, rental_id);
            car_dao.UpdateCarStatus1(rental_contract.CarId);
            return RedirectToAction("RentalReport");
        }

        public IActionResult ConfirmedRental(int rental_id)
        {
            finance_dao.UpdateStatusRentalContract1(rental_id);
            return RedirectToAction("RentalReport");
        }

        public IActionResult CompletedRental(int rental_id)
        {
            var rental_contract = finance_dao.GetRentalContractByID(rental_id);
            finance_dao.UpdateStatusRentalContract2(rental_id);
            car_dao.UpdateCarStatus1(rental_contract.CarId);
            return RedirectToAction("RentalReport");
        }
    }
}
