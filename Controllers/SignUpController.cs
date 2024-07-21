using Microsoft.AspNetCore.Mvc;
using Project_RentACar.DAO;
using Project_RentACar.Models;
using System.Net.Mail;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Newtonsoft.Json.Linq;
namespace Project_RentACar.Controllers
{
    public class SignUpController : Controller
    {
        private readonly AccountDAO dao_user;
        public SignUpController(AccountDAO accountDAO)
        {
            dao_user = accountDAO;
        }

        public IActionResult SignUp()
        {
            return View("SignUpForm");
        }

        [HttpPost]

        public async Task<IActionResult> ValidateEmailAsync(string fname, string lname, string email, string password, string repassword, DateTime dob, string phone, string address)
        {
            int age = DateTime.Today.Year - dob.Year;
            if (dob > DateTime.Today.AddYears(-age))
            {
                age--;
            }

            SignUp signUp = new SignUp()
            {
                firstName = fname,
                lastName = lname,
                email = email,
                password = password,
                dob = dob,
                phone = phone,
                address = address
            };
            ViewBag.signup = signUp;
            HttpContext.Session.SetObject("signup", signUp);

            if (!repassword.Equals(password))
            {
                ViewBag.error = "Repassword does not match with password !";
                ViewBag.signup_raw = signUp;
                return View("SignUpForm");
            }
            else if (!Regex.IsMatch(email, "^[A-Za-z0-9+_.-]+@(.+)$"))
            {
                ViewBag.error = "Invalid email address format !";
                ViewBag.signup_raw = signUp;
                return View("SignUpForm");
            }
            else if (Regex.IsMatch(password, ".*\\s.*"))
            {
                ViewBag.error = "Password should not contain spaces.";
                ViewBag.signup_raw = signUp;
                return View("SignUpForm");
            }
            else if (Regex.IsMatch(password, ".*\\s{2,}.*"))
            {
                ViewBag.error = "Password should not contain multiple consecutive spaces.";
                ViewBag.signup_raw = signUp;
                return View("SignUpForm");
            }
            else if (!Regex.IsMatch(password, "^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@#$%^&+=!]).{8,}$"))
            {
                string passError = "Password does not meet complexity requirements. You are missing: \n";
                if (!Regex.IsMatch(password, ".*[A-Z].*"))
                {
                    passError += "  - at least one uppercase letter\n";
                }
                if (!Regex.IsMatch(password, ".*[a-z].*"))
                {
                    passError += "  - at least one lowercase letter\n";
                }
                if (!Regex.IsMatch(password, ".*\\d.*"))
                {
                    passError += "  - at least one digit\n";
                }
                if (!Regex.IsMatch(password, ".*[@#$%^&+=!].*"))
                {
                    passError += "  - at least one special character\n";
                }
                if (password.Length < 8)
                {
                    passError += "  - a minimum length of 8 characters\n";
                }
                ViewBag.error = passError;
                ViewBag.signup_raw = signUp;
                return View("SignUpForm");
            }
            else if (!Regex.IsMatch(phone, "^\\d{10,}$"))
            {
                ViewBag.error = "Phone number must have at least 10 digits!";
                ViewBag.signup_raw = signUp;
                return View("SignUpForm");
            }
            else if (dao_user.CheckAccountExist(email) != null)
            {
                ViewBag.error = "Email already exists! Please use other email and try again.";
                ViewBag.signup_raw = signUp;
                return View("SignUpForm");
            }
            else if (age < 18)
            {
                ViewBag.error = "You're under 18 years old !";
                ViewBag.signup_raw = signUp;
                return View("SignUpForm");
            }
            else
            {
                int otpValue = new Random().Next(100000, 1000000);
                string fromEmail = "alphashopfpt@gmail.com";
                string fromPassword = "hgmw axdt dcsj jhig";
                try
                {
                    var message = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = "Car Rent",
                        Body = $"Hello. This is your OTP code. Do not share this code with anyone! Your OTP is: {otpValue}"
                    };
                    message.To.Add(new MailAddress(email));

                    using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
                        smtpClient.EnableSsl = true;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        await smtpClient.SendMailAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to send email", ex);
                }

                HttpContext.Session.SetInt32("otp", otpValue);
                DateTime now = DateTime.Now;
                HttpContext.Session.SetString("currentDateTime", now.ToString("o"));
                return RedirectToAction("ValidateOTP");
            }
        }

        public IActionResult ValidateOTP()
        {
            return View("EnterOTP");
        }

        [HttpPost]
        public IActionResult ValidateOTP(string digit1, string digit2, string digit3, string digit4, string digit5, string digit6)
        {
            string userResponse = HttpContext.Request.Form["g-recaptcha-response"];
            int otp = Int32.Parse($"{digit1}{digit2}{digit3}{digit4}{digit5}{digit6}");
            int? storedOtp = HttpContext.Session.GetInt32("otp");
            string dateTimeString = HttpContext.Session.GetString("currentDateTime");
            DateTime storedDateTime = DateTime.Parse(dateTimeString);
            DateTime now = DateTime.Now;
            TimeSpan timeDifference = now - storedDateTime;

            try
            {
                if (userResponse.Equals(""))
                {
                    ViewBag.messgage = "You must verify captcha!";
                    return View("EnterOTP");
                }
                else if (timeDifference.TotalSeconds >= 30)
                {
                    ViewBag.message = "Your OTP is terminated! Please try again.";
                    HttpContext.Session.Remove("otp");
                    HttpContext.Session.Remove("currentDateTime");
                    return View("EnterOTP");
                }
                else if (otp == storedOtp)
                {
                    HttpContext.Session.Remove("otp");
                    HttpContext.Session.Remove("currentDateTime");
                    var signUp = HttpContext.Session.GetObject<SignUp>("signup");
                    dao_user.SignUp(signUp.firstName, signUp.lastName, signUp.email, signUp.password, signUp.address, signUp.phone, signUp.dob);
                    ViewBag.message = "Resgistration is done. Redirecting to Login page in a few second....";
                    HttpContext.Session.Remove("signup");
                    return View("EnterOTP");
                }
                else
                {
                    ViewBag.message = "OTP is wrong!";
                    return View("EnterOTP");
                }
            }
            catch (Exception)
            {
                return View("EnterOTP");
            }
        }
    }
}


