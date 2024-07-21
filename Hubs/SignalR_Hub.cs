using Microsoft.AspNetCore.SignalR;
using Project_RentACar.Models;
using System.Data.Entity;

namespace Project_RentACar.Hubs
{
    public class SignalR_Hub : Hub
    {
        private readonly CarRentalDBContext _context;

        public SignalR_Hub(CarRentalDBContext context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetAllProducts()
        {
            var pro = _context.Cars.OrderBy(x => x.Model);
            return await pro.ToListAsync();
        }

        public async Task AddProduct(Car product)
        {
            _context.Cars.Add(product);
            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("LoadProduct", product);
        }

        public async Task UpdateProduct(Car product)
        {
            _context.Cars.Update(product);
            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("LoadProduct", product);
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _context.Cars.FindAsync(id);

            if (product != null)
            {
                _context.Cars.Remove(product);
                await _context.SaveChangesAsync();
                await Clients.All.SendAsync("LoadProduct", id);
            }
        }
    }
}
