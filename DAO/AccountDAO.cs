using Project_RentACar.Models;
using System;
using System.Security.Principal;

namespace Project_RentACar.DAO
{
    public class AccountDAO
    {
        public readonly CarRentalDBContext _context;
        public AccountDAO()
        {
        }
        public AccountDAO(CarRentalDBContext context)
        {
            _context = context;
        }
        public Customer Login(string email, String password)
        {
            try
            {
                var account = _context.Customers.SingleOrDefault(data => data.Email == email && data.Password == password);
                if (account != null)
                {
                    return account;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }

        public int GetPasswordAttempts(string email)
        {
            int count = 0;
            try
            {
                var customer = _context.Customers.SingleOrDefault(c => c.Email == email);
                if (customer != null)
                {
                    count = customer.PasswordAttempts;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return count;
        }

        public bool GetAccountStatus(string email)
        {
            bool check = true;
            try
            {
                var customer = _context.Customers.SingleOrDefault(c => c.Email == email);
                if (customer != null)
                {
                    if (customer.IsActive == false)
                    {
                        check = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return check;
        }
        public DateTime? GetUnlockTime(string email)
        {
            DateTime? unlockTime = null;
            try
            {
                var customer = _context.Customers.SingleOrDefault(c => c.Email == email);
                if (customer != null)
                {
                    unlockTime = customer.UnlockTime;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return unlockTime;
        }
        public void UpdatePasswordAttempts(string email)
        {
            try
            {
                var customer = _context.Customers.SingleOrDefault(c => c.Email == email);
                if (customer != null)
                {
                    customer.PasswordAttempts += 1;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void AddUnlockTime(string email)
        {
            try
            {
                var customer = _context.Customers.SingleOrDefault(c => c.Email == email);
                if (customer != null)
                {
                    customer.UnlockTime = DateTime.Now.AddMinutes(1);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }


        public void ResetPasswordAttempts(string email)
        {
            try
            {
                var customer = _context.Customers.SingleOrDefault(c => c.Email == email);
                if (customer != null)
                {
                    customer.PasswordAttempts = 0;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public void DecreasePasswordAttempts(string email)
        {
            var account = _context.Customers.FirstOrDefault(a => a.Email == email);
            if (account != null)
            {
                account.PasswordAttempts -= 1;
                _context.SaveChanges();
            }
        }

        public void ResetUnlockTime(string email)
        {
            var account = _context.Customers.FirstOrDefault(a => a.Email == email);
            if (account != null)
            {
                account.UnlockTime = null;
                _context.SaveChanges();
            }
        }

        public Customer CheckAccountExist(string email)
        {
            try
            {
                var account = _context.Customers.SingleOrDefault(a => a.Email == email);
                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        public Customer GetAccountByID(int customer_id)
        {
            try
            {
                var account = _context.Customers.SingleOrDefault(a => a.CustomerId == customer_id);
                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        public void SignUp(string fname, string lname, string email, string password, string address, string phone, DateTime dob)
        {
            try
            {
                Customer customer = new Customer()
                {
                    FirstName = fname,
                    LastName = lname,
                    Email = email,
                    Password = password,
                    Address = address,
                    PhoneNumber = phone,
                    DateOfBirth = dob
                };
                _context.Customers.Add(customer);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public void ChangePassword(string email, string newpassword)
        {
            try
            {
                var account = _context.Customers.SingleOrDefault(u => u.Email == email);
                if (account != null)
                {
                    account.Password = newpassword;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void UpdateProfile(string email, string newFName, string newLName, string newPhone, string newAddress)
        {
            try
            {
                var account = _context.Customers.SingleOrDefault(c => c.Email == email);
                if (account != null)
                {
                    account.FirstName = newFName;
                    account.LastName = newLName;
                    account.PhoneNumber = newPhone;
                    account.Address = newAddress;
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Account not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public void UpdateImageProfile(string email, string image_path)
        {
            try
            {
                var account = _context.Customers.SingleOrDefault(c => c.Email == email);
                if (account != null)
                {
                    account.Image = image_path;
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Account not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }


        public List<Customer> GetAllCustomer()
        {
            var customer = new List<Customer>();
            try
            {
                customer = _context.Customers.ToList();
                return customer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching customer: {ex.Message}");
                return null;
            }
        }

        public void SetActive(int id, string active)
        {
            if (id != null)
            {
                Customer c = GetAccountByID(id);
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
    }
}
