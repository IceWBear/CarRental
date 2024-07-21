using LinqKit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project_RentACar.Models;
using System;
using System.Security.Principal;

namespace Project_RentACar.DAO
{
    public class FinanceDAO
    {
        public FinanceDAO()
        {
        }
        public readonly CarRentalDBContext _context;
        public FinanceDAO(CarRentalDBContext context)
        {
            _context = context;
        }
        public List<Transaction> GetAllTransactions(int customerid)
        {
            try
            {
                var transactions = _context.Transactions
                    .Include(t => t.Customer)
                    .Where(t => t.CustomerId == customerid)
                    .OrderByDescending(t => t.IssueDate)
                    .Select(t => new Transaction
                    {
                        Id = t.Id,
                        CustomerId = t.CustomerId,
                        IssueDate = t.IssueDate,
                        Action = t.Action,
                        Note = t.Note
                    })
                    .ToList();
                return transactions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching transactions: {ex.Message}");
                throw;
            }
        }

        public void UpdateStatusRentalContract(int rental_id)
        {
            try
            {
                var rental_contract = _context.RentalContracts.SingleOrDefault(rc => rc.ContractId == rental_id);
                if (rental_contract != null)
                {
                    rental_contract.Status = "Canceled";
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating rental contract: {ex.Message}");
            }
        }

        public void UpdateStatusRentalContract1(int rental_id)
        {
            try
            {
                var rental_contract = _context.RentalContracts.SingleOrDefault(rc => rc.ContractId == rental_id);
                if (rental_contract != null)
                {
                    rental_contract.Status = "Renting";
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating rental contract: {ex.Message}");
            }
        }
        public void UpdateStatusRentalContract2(int rental_id)
        {
            try
            {
                var rental_contract = _context.RentalContracts.SingleOrDefault(rc => rc.ContractId == rental_id);
                if (rental_contract != null)
                {
                    rental_contract.Status = "Completed";
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating rental contract: {ex.Message}");
            }
        }
        public List<RentalContract> GetAllRentalContracts(int customer_id)
        {
            try
            {
                var rentalContracts = _context.RentalContracts
                    .Where(rc => rc.CustomerId == customer_id)
                    .ToList();

                return rentalContracts;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving rental contracts: " + ex.Message);
                return new List<RentalContract>();
            }
        }


        public List<Transaction> SearchTransaction(int customerId, string search)
        {
            var transactions = new List<Transaction>();

            try
            {
                decimal actionBefore, actionAfter;
                bool isNumericSearch = decimal.TryParse(search, out decimal searchNumber);

                actionBefore = isNumericSearch ? searchNumber - 1 : 0;
                actionAfter = isNumericSearch ? searchNumber + 1 : 0;

                bool isDateSearch = DateTime.TryParse(search, out DateTime searchDate);

                var predicate = PredicateBuilder.New<Transaction>(t => t.CustomerId == customerId);

                predicate = predicate.And(t => t.Id.ToString().Contains(search) ||
                                               t.Note.Contains(search) ||
                                               (isNumericSearch && t.Action >= actionBefore && t.Action <= actionAfter) ||
                                               (isDateSearch && t.IssueDate.Date == searchDate.Date));

                transactions = _context.Transactions
                    .Where(predicate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while searching transactions: {ex.Message}");
            }

            return transactions;
        }

        public void UpdateBalance(int customer_id, decimal amount)
        {
            try
            {
                var account = _context.Customers.SingleOrDefault(a => a.CustomerId == customer_id);
                if (account != null)
                {
                    account.Balance += amount;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the balance: {ex.Message}");
            }
        }

        public void UpdateBalance1(int customer_id, decimal amount)
        {
            try
            {
                var account = _context.Customers.SingleOrDefault(a => a.CustomerId == customer_id);
                if (account != null)
                {
                    account.Balance -= amount;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the balance: {ex.Message}");
            }
        }

        public void AddTransaction(int customer_id, decimal amount)
        {
            string note = "Import money from Paypal";
            try
            {
                var transaction = new Transaction
                {
                    CustomerId = customer_id,
                    Action = amount,
                    IssueDate = DateTime.Now,
                    Note = note,
                };
                _context.Transactions.Add(transaction);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the transaction: {ex.Message}");
            }
        }

        public void AddTransaction1(int customer_id, decimal amount, int rental_id)
        {
            string note = "Paid by CRP for rental ID: ";
            try
            {
                var transaction = new Transaction
                {
                    CustomerId = customer_id,
                    Action = amount,
                    IssueDate = DateTime.Now,
                    Note = note + rental_id
                };
                _context.Transactions.Add(transaction);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the transaction: {ex.Message}");
            }
        }

        public Transaction GetTop1Transaction()
        {
            try
            {
                var transaction = _context.Transactions
                    .OrderByDescending(t => t.IssueDate)
                    .FirstOrDefault();

                if (transaction != null)
                {
                    return transaction;
                }
                else
                {
                    throw new Exception("No transactions found");
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public void AddTransaction3(int customer_id, decimal amount, int rental_id)
        {
            string note = "Paid by Paypal for rental ID: ";
            try
            {
                var transaction = new Transaction
                {
                    CustomerId = customer_id,
                    Action = amount,
                    IssueDate = DateTime.Now,
                    Note = note + rental_id
                };
                _context.Transactions.Add(transaction);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the transaction: {ex.Message}");
            }
        }

        public void AddTransaction2(int customer_id, decimal amount, int rental_id)
        {
            string note = "Refund from rental ID: ";
            try
            {
                var transaction = new Transaction
                {
                    CustomerId = customer_id,
                    Action = amount,
                    IssueDate = DateTime.Now,
                    Note = note + rental_id
                };
                _context.Transactions.Add(transaction);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the transaction: {ex.Message}");
            }
        }


        public RentalContract GetRentalContractByID(int rental_id)
        {
            RentalContract rentalContract = null;

            try
            {
                rentalContract = _context.RentalContracts.FirstOrDefault(rc => rc.ContractId == rental_id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the rental contract: {ex.Message}");
            }

            return rentalContract;
        }

        public Transaction GetTransactionByRentalId(int rental_id)
        {
            try
            {
                var transaction = _context.Transactions
     .FirstOrDefault(t => t.Note.Contains(rental_id.ToString()));

                if (transaction != null)
                {
                    return transaction;
                }
                else
                {
                    throw new Exception($"No transaction found for rental ID: {rental_id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving transaction: {ex.Message}");
                return null;
            }
        }

        public List<RentalContract> SearchRentalContracts(int customerId, string search)
        {
            var rentalContracts = new List<RentalContract>();

            try
            {
                decimal actionBefore, actionAfter;
                bool isNumericSearch = decimal.TryParse(search, out decimal searchNumber);

                actionBefore = isNumericSearch ? searchNumber - 1 : 0;
                actionAfter = isNumericSearch ? searchNumber + 1 : 0;

                bool isDateSearch = DateTime.TryParse(search, out DateTime searchDate);

                var predicate = PredicateBuilder.New<RentalContract>(rc => rc.CustomerId == customerId);

                predicate = predicate.And(rc => rc.ContractId.ToString().Contains(search) ||
                                                rc.Status.Contains(search) ||
                                                (isNumericSearch && rc.TotalAmount >= actionBefore && rc.TotalAmount <= actionAfter) ||
                                                (isDateSearch && rc.StartDate.Date == searchDate.Date));

                rentalContracts = _context.RentalContracts
                    .Where(predicate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while searching rental contracts: {ex.Message}");
            }
            return rentalContracts;
        }
    }
}
