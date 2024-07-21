using Microsoft.AspNetCore.Mvc;
using Project_RentACar.DAO;
using Project_RentACar.Models;
using System.Security.Principal;

namespace Project_RentACar.Controllers
{
    public class PurchaseController : Controller
    {
        CarRentalDBContext context = new CarRentalDBContext();
        private readonly AccountDAO dao_user;
        private readonly FinanceDAO finance_dao;
        private readonly CarDAO car_dao;
        public PurchaseController(AccountDAO accountDAO, FinanceDAO financeDAO, CarDAO carDAO)
        {
            dao_user = accountDAO;
            finance_dao = financeDAO;
            car_dao = carDAO;
        }
        public IActionResult PurchaseByCarRentalPAY()
        {
            var rental_contract = HttpContext.Session.GetObject<RentalContract>("contract");
            DateTime start_date = rental_contract.StartDate;
            DateTime end_date = rental_contract.EndDate;
            TimeSpan duration = end_date - start_date;
            int numberOfDays = duration.Days;
            if (numberOfDays == 0)
            {
                numberOfDays = 1;
            }
            var account = HttpContext.Session.GetObject<Customer>("account");
            var car = car_dao.getCarByID(rental_contract.CarId);
            string formattedBalance = account.Balance.ToString("#,##0.00");
            string formattedDiscount = (rental_contract.TotalAmount / 100 * 5).ToString("#,##0.00");
            string vatTax = (rental_contract.TotalAmount / 100 * 10).ToString("#,##0.00");
            string total = (rental_contract.TotalAmount - rental_contract.TotalAmount / 100 * 5 + 25 + rental_contract.TotalAmount / 100 * 10).ToString("#,##0.00");
            if (account.Balance < Decimal.Parse(total))
            {
                ViewBag.error = "Your CarRentalPAY balance is not enough!";
            }
            HttpContext.Session.SetString("total_CRP", total);
            ViewBag.car = car;
            ViewBag.car_manufacturer = car_dao.GetCarManufacturerByID(rental_contract.CarId);
            ViewBag.contract = rental_contract;
            ViewBag.account_balance = formattedBalance;
            ViewBag.number_of_day = numberOfDays;
            ViewBag.formattedDiscount = formattedDiscount;
            ViewBag.vat_tax = vatTax;
            ViewBag.total = total;
            return View("Checkout");
        }

        public IActionResult PurchaseByPaypal()
        {
            var rental_contract = HttpContext.Session.GetObject<RentalContract>("contract");
            DateTime start_date = rental_contract.StartDate;
            DateTime end_date = rental_contract.EndDate;
            TimeSpan duration = end_date - start_date;
            int numberOfDays = duration.Days;
            if (numberOfDays == 0)
            {
                numberOfDays = 1;
            }
            var account = HttpContext.Session.GetObject<Customer>("account");
            var car = car_dao.getCarByID(rental_contract.CarId);
            string formattedBalance = account.Balance.ToString("#,##0.00");
            string vatTax = (rental_contract.TotalAmount / 100 * 10).ToString("#,##0.00");
            string total = (rental_contract.TotalAmount + 25 + rental_contract.TotalAmount / 100 * 10).ToString("#,##0.00");
            HttpContext.Session.SetString("total_paypal", total);
            ViewBag.car = car;
            ViewBag.car_manufacturer = car_dao.GetCarManufacturerByID(rental_contract.CarId);
            ViewBag.contract = rental_contract;
            ViewBag.account_balance = formattedBalance;
            ViewBag.number_of_day = numberOfDays;
            ViewBag.formattedDiscount = "0";
            ViewBag.vat_tax = vatTax;
            ViewBag.total = total;
            return View("Checkout");
        }
        public IActionResult CheckpointPurchase()
        {
            return View("CheckpointPurchase");
        }

        [HttpPost]
        public ActionResult CheckpointPurchase(string password)
        {
            string userResponse = HttpContext.Request.Form["g-recaptcha-response"];
            var account = HttpContext.Session.GetObject<Customer>("account");
            if (userResponse.Equals(""))
            {
                ViewBag.error = "You must verify captcha!";
                return View("CheckpointPurchase");
            }
            else if (dao_user.GetPasswordAttempts(account.Email) == 5)
            {
                dao_user.AddUnlockTime(account.Email);
                HttpContext.Session.Clear();
                TempData["error"] = "You try many times. Your account is temporarily locked in 30 mins!";
                return RedirectToAction("Login", "Login");
            }
            else if (!account.Password.Equals(password))
            {
                dao_user.UpdatePasswordAttempts(account.Email);
                ViewBag.error = "Wrong Password!";
                return View("CheckpointPurchase");
            }
            else
            {
                return RedirectToAction("CheckoutByCarRentalPAY", "Purchase");
            }
        }

        public IActionResult CheckoutByCarRentalPAY()
        {
            var rental_contract = HttpContext.Session.GetObject<RentalContract>("contract");
            string selectedPaymentMethod = "CarRentalPAY";
            RentalContract rentalContract = new RentalContract()
            {
                CarId = rental_contract.CarId,
                CustomerId = rental_contract.CustomerId,
                FirstName = rental_contract.FirstName,
                LastName = rental_contract.LastName,
                Email = rental_contract.Email,
                Phone = rental_contract.Phone,
                Destination = rental_contract.Destination,
                StartDate = rental_contract.StartDate,
                EndDate = rental_contract.EndDate,
                TotalAmount = rental_contract.TotalAmount,
                PaymentMethod = selectedPaymentMethod,
                Note = rental_contract.Note,
                Status = "Waiting for confirm",
            };
            context.RentalContracts.Add(rentalContract);
            context.SaveChanges();

            HttpContext.Session.SetObject("contract", rentalContract);

            decimal amount = Convert.ToDecimal(HttpContext.Session.GetString("total_CRP"));
            finance_dao.UpdateBalance1(rentalContract.CustomerId, amount);
            finance_dao.AddTransaction1(rentalContract.CustomerId, amount, rentalContract.ContractId);
            return RedirectToAction("Invoice");
        }

        public IActionResult CheckoutRedirectPaypal(decimal amount)
        {
            ViewBag.amount = amount;
            return View("CheckoutRedirectPaypal");
        }

        public IActionResult CheckoutByPaypal()
        {
            var rental_contract = HttpContext.Session.GetObject<RentalContract>("contract");
            string selectedPaymentMethod = "Paypal";

            RentalContract rentalContract = new RentalContract()
            {
                CarId = rental_contract.CarId,
                CustomerId = rental_contract.CustomerId,
                FirstName = rental_contract.FirstName,
                LastName = rental_contract.LastName,
                Email = rental_contract.Email,
                Phone = rental_contract.Phone,
                Destination = rental_contract.Destination,
                StartDate = rental_contract.StartDate,
                EndDate = rental_contract.EndDate,
                TotalAmount = rental_contract.TotalAmount,
                PaymentMethod = selectedPaymentMethod,
                Note = rental_contract.Note,
                Status = "Waiting for confirm",
            };
            context.RentalContracts.Add(rentalContract);
            context.SaveChanges();

            HttpContext.Session.SetObject("contract", rentalContract);

            decimal amount = Convert.ToDecimal(HttpContext.Session.GetString("total_paypal"));
            finance_dao.AddTransaction3(rentalContract.CustomerId, amount, rentalContract.ContractId);
            return RedirectToAction("Invoice");
        }

        public IActionResult Invoice()
        {
            var rental_contract = HttpContext.Session.GetObject<RentalContract>("contract");
            DateTime start_date = rental_contract.StartDate;
            DateTime end_date = rental_contract.EndDate;
            TimeSpan duration = end_date - start_date;
            int numberOfDays = duration.Days;
            if (numberOfDays == 0)
            {
                numberOfDays = 1;
            }

            var car = car_dao.getCarByID(rental_contract.CarId);
            car_dao.UpdateCarStatus(rental_contract.CarId);
            ViewBag.NumberOfDays = numberOfDays;
            ViewBag.transaction = finance_dao.GetTop1Transaction();
            ViewBag.contract = rental_contract;
            ViewBag.car = car;
            ViewBag.car_manufacturer = car_dao.GetCarManufacturerByID(rental_contract.CarId);
            return View("Invoice");
        }
        public IActionResult Exit()
        {
            var sessionKeys = HttpContext.Session.Keys.ToList();

            foreach (var key in sessionKeys)
            {
                if (key != "account")
                {
                    HttpContext.Session.Remove(key);
                }
            }
            return RedirectToAction("LoadCar", "Car");
        }
    }
}
