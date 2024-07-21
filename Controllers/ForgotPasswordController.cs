using Microsoft.AspNetCore.Mvc;
using Project_RentACar.DAO;
using Project_RentACar.Models;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace Project_RentACar.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly AccountDAO dao_user;
        public ForgotPasswordController(AccountDAO accountDAO)
        {
            dao_user = accountDAO;
        }
        public IActionResult EnterEmail()
        {
            return View("EnterEmail");
        }

        [HttpGet]
        public async Task<IActionResult> ValidateEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                email = HttpContext.Session.GetString("email");
                if (string.IsNullOrEmpty(email))
                {
                    ViewBag.error = "Email is missing. Please start over.";
                    return View("EnterEmail");
                }
            }
            Customer customer = dao_user.CheckAccountExist(email);
            if (customer == null)
            {
                ViewBag.error = "Your email is not available in the system. Try again!";
                ViewBag.email = email;
                return View("EnterEmail");
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
                HttpContext.Session.SetString("email", email);
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
                    ViewBag.error = "You must verify captcha!";
                    return View("EnterOTP");
                }
                else if (timeDifference.TotalSeconds >= 30)
                {
                    ViewBag.error = "Your OTP is terminated! Please try again.";
                    HttpContext.Session.Remove("otp");
                    HttpContext.Session.Remove("currentDateTime");
                    return View("EnterOTP");
                }
                else if (otp == storedOtp)
                {
                    HttpContext.Session.Remove("otp");
                    HttpContext.Session.Remove("currentDateTime");
                    return RedirectToAction("NewPassword");
                }
                else
                {
                    ViewBag.error = "OTP is wrong!";
                    return View("EnterOTP");
                }
            }
            catch (Exception)
            {
                return View("EnterOTP");
            }
        }
        public IActionResult NewPassword()
        {
            return View("EnterNewPassword");
        }
        [HttpPost]
        public IActionResult NewPassword(string newpassword, string renewpassword)
        {
            if (newpassword != renewpassword)
            {
                ViewBag.error = "Re-password is not match with New Password!";
                return View("EnterNewPassword");
            }
            else if (newpassword.Contains(" "))
            {
                ViewBag.error = "Password should not contain spaces.";
                return View("EnterNewPassword");
            }
            else if (newpassword.Contains("  ")) // Two consecutive spaces
            {
                ViewBag.error = "Password should not contain spaces.";
                return View("EnterNewPassword");
            }
            else if (!Regex.IsMatch(newpassword, "^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[@#$%^&+=!]).{8,}$"))
            {
                var passError = "Password does not meet complexity requirements. You are missing: \n";
                if (!Regex.IsMatch(newpassword, ".*[A-Z].*"))
                {
                    passError += "  - at least one uppercase letter\n";
                }
                if (!Regex.IsMatch(newpassword, ".*[a-z].*"))
                {
                    passError += "  - at least one lowercase letter\n";
                }
                if (!Regex.IsMatch(newpassword, ".*\\d.*"))
                {
                    passError += "  - at least one digit\n";
                }
                if (!Regex.IsMatch(newpassword, ".*[@#$%^&+=!].*"))
                {
                    passError += "  - at least one special character\n";
                }
                if (newpassword.Length < 8)
                {
                    passError += "  - a minimum length of 8 characters\n";
                }
                if (!string.IsNullOrEmpty(passError))
                {
                    ViewBag.error = passError + ".";
                    return View("EnterNewPassword");
                }
            }
            else
            {
                string email = HttpContext.Session.GetString("email");
                dao_user.ChangePassword(email, newpassword);
                dao_user.ResetPasswordAttempts(email);
                dao_user.ResetUnlockTime(email);
                HttpContext.Session.Remove(email);
                ViewBag.message = "Reset password successfully! Redirecting to Login page in a few second....";
                return View("EnterNewPassword");
            }
            return View("EnterNewPassword");
        }
    }
}