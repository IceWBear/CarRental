using Microsoft.AspNetCore.Mvc;
using Project_RentACar.DAO;
using Project_RentACar.Models;
using System.Text.RegularExpressions;

namespace Project_RentACar.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AccountDAO dao_user;
        private readonly FinanceDAO finance_dao;
        private readonly CarDAO car_dao;
        public ProfileController(AccountDAO accountDAO, FinanceDAO financeDAO, CarDAO carDAO)
        {
            dao_user = accountDAO;
            finance_dao = financeDAO;
            car_dao = carDAO;
        }
        public IActionResult Profile()
        {
            ViewBag.customer = HttpContext.Session.GetObject<Customer>("account");
            return View("Profile");
        }

        [HttpPost]
        public IActionResult UpdateProfile(string fname, string lname, string phone, string address)
        {
            var account_raw = HttpContext.Session.GetObject<Customer>("account");
            dao_user.UpdateProfile(account_raw.Email, fname, lname, phone, address);
            var account = dao_user.CheckAccountExist(account_raw.Email);
            HttpContext.Session.SetObject("account", account);
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult UpdateImageProfile(IFormFile imageFile)
        {
            var fileName = Path.GetFileName(imageFile.FileName);
            var account_raw = HttpContext.Session.GetObject<Customer>("account");
            dao_user.UpdateImageProfile(account_raw.Email, fileName);
            var account = dao_user.CheckAccountExist(account_raw.Email);
            HttpContext.Session.SetObject("account", account);
            return RedirectToAction("Profile");
        }

        public IActionResult Security()
        {
            return View("Security");
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldpassword, string newpassword, string renewpassword)
        {
            var account = HttpContext.Session.GetObject<Customer>("account");
            if (dao_user.GetPasswordAttempts(account.Email) == 5)
            {
                dao_user.AddUnlockTime(account.Email);
                HttpContext.Session.Clear();
                TempData["error"] = "You try many times. Your account is temporarily locked in 30 mins!";
                return RedirectToAction("Login", "Login");
            }
            else
            {
                if (newpassword != renewpassword)
                {
                    ViewBag.message = "Re-password is not match with New Password!";
                    return View("Security");
                }
                else if (oldpassword != account.Password)
                {
                    dao_user.UpdatePasswordAttempts(account.Email);
                    ViewBag.message = "Old password is wrong!";
                    return View("Security");
                }
                else if (newpassword.Contains(" "))
                {
                    ViewBag.message = "Password should not contain spaces.";
                    return View("Security");
                }
                else if (Regex.IsMatch(newpassword, @".*\s{2,}.*"))
                {
                    ViewBag.message = "Password should not contain consecutive spaces.";
                    return View("Security");
                }
                else if (!Regex.IsMatch(newpassword, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&+=!]).{8,}$"))
                {
                    string passError = "Password does not meet complexity requirements. You are missing:\n";
                    if (!Regex.IsMatch(newpassword, @".*[A-Z].*"))
                    {
                        passError += "  - at least one uppercase letter\n";
                    }
                    if (!Regex.IsMatch(newpassword, @".*[a-z].*"))
                    {
                        passError += "  - at least one lowercase letter\n";
                    }
                    if (!Regex.IsMatch(newpassword, @".*\d.*"))
                    {
                        passError += "  - at least one digit\n";
                    }
                    if (!Regex.IsMatch(newpassword, @".*[@#$%^&+=!].*"))
                    {
                        passError += "  - at least one special character\n";
                    }
                    if (newpassword.Length < 8)
                    {
                        passError += "  - a minimum length of 8 characters\n";
                    }
                    ViewBag.message = passError;
                    return View("Security");
                }
                else
                {
                    dao_user.ChangePassword(account.Email, newpassword);
                    dao_user.ResetPasswordAttempts(account.Email);
                    ViewBag.message = "Change password successfully! Redirecting to Profile page in a few second....";
                    return View("Security");
                }
            }
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
                return RedirectToAction("Finance", "Profile");
            }
        }

        public IActionResult Finance()
        {
            var account_raw = HttpContext.Session.GetObject<Customer>("account");
            var account = dao_user.CheckAccountExist(account_raw.Email);
            string formattedBalance = account.Balance.ToString("#,##0.00");
            ViewBag.balance = formattedBalance;
            ViewBag.list_transaction = finance_dao.GetAllTransactions(account.CustomerId);
            return View("Finance");
        }
        public IActionResult RedirectPaypal()
        {
            return View("RedirectPayPal");
        }

        public IActionResult SubmitPurchase(decimal amount)
        {
            var account = HttpContext.Session.GetObject<Customer>("account");
            finance_dao.UpdateBalance(account.CustomerId, amount);
            finance_dao.AddTransaction(account.CustomerId, amount);
            return RedirectToAction("Finance");
        }

        [HttpPost]
        public IActionResult FinanceSearch(string search)
        {
            if (search == null)
            {
                return RedirectToAction("Finance");
            }
            else
            {
                var account = HttpContext.Session.GetObject<Customer>("account");
                ViewBag.list_transaction = finance_dao.SearchTransaction(account.CustomerId, search);
                string formattedBalance = account.Balance.ToString("#,##0.00");
                ViewBag.balance = formattedBalance;
                ViewBag.search = search;
                return View("Finance");
            }
        }

        public IActionResult Rental()
        {
            var account = HttpContext.Session.GetObject<Customer>("account");
            ViewBag.listRentalContract = finance_dao.GetAllRentalContracts(account.CustomerId);
            return View("Rental");
        }

        [HttpPost]
        public IActionResult RentalSearch(string search)
        {
            if (search == null)
            {
                return RedirectToAction("Rental");
            }
            else
            {
                var account = HttpContext.Session.GetObject<Customer>("account");
                ViewBag.listRentalContract = finance_dao.SearchRentalContracts(account.CustomerId, search);
                ViewBag.search = search;
            }
            return View("Rental");
        }

        public IActionResult ViewRentalDetail(int rental_id)
        {
            var rental_contract = finance_dao.GetRentalContractByID(rental_id);
            DateTime start_date = rental_contract.StartDate;
            DateTime end_date = rental_contract.EndDate;
            TimeSpan duration = end_date - start_date;
            int numberOfDays = duration.Days;
            if (numberOfDays == 0)
            {
                numberOfDays = 1;
            }
            var car = car_dao.getCarByID(rental_contract.CarId);
            ViewBag.NumberOfDays = numberOfDays;
            ViewBag.transaction = finance_dao.GetTransactionByRentalId(rental_id);
            ViewBag.contract = rental_contract;
            ViewBag.car = car;
            ViewBag.car_manufacturer = car_dao.GetCarManufacturerByID(rental_contract.CarId);
            return View("ViewRentalDetail");
        }

        public IActionResult RentalCanceled(int rental_id)
        {
            var account = HttpContext.Session.GetObject<Customer>("account");
            var rental_contract = finance_dao.GetRentalContractByID(rental_id);
            var transaction = finance_dao.GetTransactionByRentalId(rental_id);
            finance_dao.UpdateStatusRentalContract(rental_id);
            finance_dao.UpdateBalance(account.CustomerId, transaction.Action);
            finance_dao.AddTransaction2(account.CustomerId, transaction.Action, rental_id);
            car_dao.UpdateCarStatus1(rental_contract.CarId);
            return RedirectToAction("Rental");
        }
    }
}
