using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project_RentACar.DAO;
using Project_RentACar.Models;

namespace Project_RentACar.Controllers
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }

    public class LoginController : Controller
    {
        private readonly AccountDAO dao_user;
        public LoginController(AccountDAO accountDAO)
        {
            dao_user = accountDAO;
        }
        public IActionResult Login(string car_id)
        {
            if (car_id != null)
            {
                HttpContext.Session.SetString("car_id", car_id);
            }
            ViewBag.error = TempData["error"] as string;
            return View("LoginForm");
        }

        [HttpPost]
        public IActionResult Login(String email, String password)
        {
            try
            {
                DateTime stamp = DateTime.Now;
                var account = dao_user.Login(email, password);

                if (dao_user.Login(email, password) == null)
                {
                    if (dao_user.GetPasswordAttempts(email) == 5 && dao_user.GetUnlockTime(email) == null)
                    {
                        dao_user.AddUnlockTime(email);
                    }
                    try
                    {
                        if (dao_user.GetPasswordAttempts(email) == 5 && stamp > dao_user.GetUnlockTime(email))
                        {
                            dao_user.DecreasePasswordAttempts(email);
                            dao_user.ResetUnlockTime(email);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    if (dao_user.GetPasswordAttempts(email) == 5)
                    {
                        ViewBag.error = "You try many times. Your account is temporarily locked in 30 mins !";
                        ViewBag.email = email;
                        return View("LoginForm");
                    }
                    else
                    {
                        dao_user.UpdatePasswordAttempts(email);
                        ViewBag.error = "Wrong Email or Password!";
                        ViewBag.email = email;
                        return View("LoginForm");
                    }
                }
                else
                {
                    try
                    {
                        if (dao_user.GetPasswordAttempts(email) == 5 && stamp > dao_user.GetUnlockTime(email))
                        {
                            dao_user.DecreasePasswordAttempts(email);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    if (dao_user.GetPasswordAttempts(email) == 5)
                    {
                        ViewBag.error = "You try many times. Your account is temporarily locked in 30 mins !";
                        ViewBag.email = email;
                        return View("LoginForm");
                    }
                    else if (dao_user.GetAccountStatus(email) == false)
                    {
                        ViewBag.error = "Your account is locked! Please contact with admin to solve.";
                        ViewBag.email = email;
                        return View("LoginForm");
                    }
                    else
                    {
                        dao_user.ResetUnlockTime(email);
                        dao_user.ResetPasswordAttempts(email);
                        if (account.Role == 0)
                        {
                            HttpContext.Session.SetObject("account", account);
                            string car_id = HttpContext.Session.GetString("car_id");
                            if (car_id == null)
                            {
                                return RedirectToAction("LoadCar", "Car");
                            }
                            else
                            {
                                HttpContext.Session.Remove("car_id");
                                return RedirectToAction("ViewDetail", "Car", new { car_id = car_id, tab = "pills-rental" });
                            }
                        }
                        else
                        {
                            HttpContext.Session.SetObject("account", account);
                            return RedirectToAction("Dashboard", "Adminstrator");
                        }
                    }
                }
            }
            catch
            {
                ViewBag.error = "Something went wrong!";
                return View("LoginForm");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}


