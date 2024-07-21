using Project_RentACar.Models;

namespace Project_RentACar.DAO
{
    public class CarDAO
    {
        public readonly CarRentalDBContext _context;
        public CarDAO()
        {
        }
        public CarDAO(CarRentalDBContext context)
        {
            _context = context;
        }
        public List<Car> GetAllCar()
        {
            var cars = new List<Car>();
            try
            {
                cars = _context.Cars.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching cars: {ex.Message}");
            }
            return cars;
        }

        public List<CarManufacturer> GetAllManufacturer()
        {
            var car_manufacturer = new List<CarManufacturer>();
            try
            {
                car_manufacturer = _context.CarManufacturers.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching cars: {ex.Message}");
            }
            return car_manufacturer;
        }

        public void UpdateImageCar(int car_id, string image_path)
        {
            try
            {
                var car = _context.Cars.SingleOrDefault(c => c.CarId == car_id);
                if (car != null)
                {
                    car.Image = "/images/" + image_path;
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Car not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public void AddFeedback(string feedback_content, int rating, int customer_id, int car_id)
        {
            try
            {
                Feedback feedback = new Feedback()
                {
                    CustomerId = customer_id,
                    CarId = car_id,
                    FeedbackContent = feedback_content,
                    Rating = rating,
                };
                _context.Feedbacks.Add(feedback);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while add feedback: {ex.Message}");
            }
        }

        public void DeleteFeedback(int feedback_id)
        {
            try
            {
                var feedback = _context.Feedbacks.SingleOrDefault(f => f.FeedbackId == feedback_id);
                _context.Feedbacks.Remove(feedback);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while delete feedback: {ex.Message}");
            }
        }

        public List<Feedback> GetAllFeedback(int car_id)
        {
            var feedback = new List<Feedback>();
            try
            {
                feedback = _context.Feedbacks.Where(f => f.CarId == car_id).ToList();
                return feedback;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching feedback: {ex.Message}");
                return null;
            }
        }

        public Car getCarByID(int car_id)
        {
            try
            {
                var car = _context.Cars.SingleOrDefault(c => c.CarId == car_id);
                return car;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public CarManufacturer GetCarManufacturerByID(int car_id)
        {
            var car = _context.Cars.SingleOrDefault(c => c.CarId == car_id);
            try
            {
                var manufacturer = _context.CarManufacturers.SingleOrDefault(m => m.ManufacturerId == car.ManufacturerId);
                return manufacturer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public List<Car> GetRelatedCar(int currentCar_id)
        {
            var cars = new List<Car>();
            try
            {
                var current_car = _context.Cars.SingleOrDefault(c => c.CarId == currentCar_id);

                if (current_car != null)
                {
                    cars = _context.Cars
                        .Where(c => c.CarId != currentCar_id) // Loại bỏ xe hiện tại khỏi danh sách
                        .OrderBy(c => Math.Abs(c.RentalPrice - current_car.RentalPrice))
                        .Take(3)
                        .ToList();
                }
                else
                {
                    Console.WriteLine("The car with the specified ID was not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching cars: {ex.Message}");
            }
            return cars;
        }
        public void UpdateCarStatus(int car_id)
        {
            try
            {
                var car = _context.Cars.SingleOrDefault(c => c.CarId == car_id);
                if (car != null)
                {
                    car.Status = false;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching cars: {ex.Message}");
            }
        }

        public void UpdateCarStatus1(int car_id)
        {
            try
            {
                var car = _context.Cars.SingleOrDefault(c => c.CarId == car_id);
                if (car != null)
                {
                    car.Status = true;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching cars: {ex.Message}");
            }
        }

        public RentalContract GetRentalContractByCarID(int car_id)
        {
            try
            {
                var rental_contract = _context.RentalContracts.SingleOrDefault(r => r.CarId == car_id && r.Status != "Completed" && r.Status != "Canceled");

                if (rental_contract == null)
                {
                    Console.WriteLine($"No rental contract found for car ID: {car_id}");
                }

                return rental_contract;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching the rental contract for car ID: {car_id}. Error: {ex.Message}");
                return null;
            }
        }

        public bool CheckRentalContractCompleted(int car_id, int customerId)
        {
            try
            {
                // Kiểm tra xem có bất kỳ hợp đồng thuê nào với car_id, customerId và trạng thái là "Completed"
                bool exists = _context.RentalContracts
                    .Any(r => r.CarId == car_id && r.CustomerId == customerId && r.Status == "Completed");

                return exists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while checking the rental contract for car ID: {car_id} and customer ID: {customerId}. Error: {ex.Message}");
                return false;
            }
        }

        public List<Car> FilterCars(string manufacturer, string carName, decimal? minRentalPrice, decimal? maxRentalPrice, int? seats, string transmission, string is_available, string fuel)
        {
            bool is_avail = false;
            if (is_available != null)
            {
                if (is_available.Equals("true"))
                {
                    is_avail = true;
                }
            }
            var query = _context.Cars.AsQueryable();
            if (!string.IsNullOrEmpty(manufacturer))
            {
                query = query.Where(car => car.Manufacturer.ManufacturerName.Equals(manufacturer));
            }

            if (!string.IsNullOrEmpty(carName))
            {
                query = query.Where(car => car.Model.Contains(carName));
            }

            if (minRentalPrice.HasValue)
            {
                query = query.Where(car => car.RentalPrice >= minRentalPrice.Value);
            }

            if (maxRentalPrice.HasValue)
            {
                query = query.Where(car => car.RentalPrice <= maxRentalPrice.Value);
            }

            if (seats.HasValue)
            {
                query = query.Where(car => car.Seats == seats.Value);
            }

            if (!string.IsNullOrEmpty(transmission))
            {
                query = query.Where(car => car.Transmission == transmission);
            }

            if (!string.IsNullOrEmpty(is_available))
            {
                query = query.Where(car => car.Status == is_avail);
            }

            if (!string.IsNullOrEmpty(fuel))
            {
                query = query.Where(car => car.Fuel == fuel);
            }
            return query.ToList();
        }

        //Long
        public void DeleteCar(int carId)
        {
            var car = _context.Cars.FirstOrDefault(x => x.CarId == carId);
            _context.Cars.Remove(car);
        }

        public List<CarType> GetCarTypes()
        {
            List<CarType> carTypes = _context.CarTypes.ToList();
            return carTypes;
        }
        public List<Branch> GetBranch()
        {
            List<Branch> carTypes = _context.Branches.ToList();
            return carTypes;
        }
        public List<CarManufacturer> GetCarManu()
        {
            List<CarManufacturer> carTypes = _context.CarManufacturers.ToList();
            return carTypes;
        }

        public void AddCar(Car car)
        {
            _context.Cars.Add(car);
        }

        public void EditCar(Car car)
        {
            Car car1 = getCarByID(car.CarId);
            car1.Model = car.Model;
            car1.ManufacturerId = car.ManufacturerId;
            car1.Seats = car.Seats;
            car1.Fuel = car.Fuel;
            car1.Luggage = car.Luggage;
            car1.Year = car.Year;
            car1.RentalPrice = car.RentalPrice;
            car1.BranchId = car.BranchId;
            car1.CarTypeId = car.CarTypeId;
            car1.Description = car.Description;
            car1.LicensePlate = car.LicensePlate;
        }

        public class Trans
        {
            public string IssueDate { get; set; }
            public decimal Action { get; set; }
        }

        public List<Trans> Getdata()
        {
            List<Trans> hihi = new List<Trans>();
            foreach (Transaction c in _context.Transactions)
            {
                hihi.Add(new Trans() { IssueDate = c.IssueDate.ToString("yyyy-MM-dd"), Action = c.Action });
            }
            return AggregateTransactions(hihi).OrderBy(x => x.IssueDate).ToList();
        }

        public List<Trans> AggregateTransactions(List<Trans> transactions)
        {
            // Sử dụng Dictionary để nhóm và cộng các giá trị Action
            var groupedTransactions = new Dictionary<string, decimal>();

            foreach (var transaction in transactions)
            {
                if (groupedTransactions.ContainsKey(transaction.IssueDate))
                {
                    groupedTransactions[transaction.IssueDate] += transaction.Action;
                }
                else
                {
                    groupedTransactions[transaction.IssueDate] = transaction.Action;
                }
            }

            // Chuyển đổi từ Dictionary về List<Trans>
            var aggregatedTransactions = groupedTransactions.Select(kvp => new Trans
            {
                IssueDate = kvp.Key,
                Action = kvp.Value
            }).ToList();

            return aggregatedTransactions;
        }

        public class Data
        {
            public string IssueDate { get; set; }
            public int Action { get; set; }
        }

        public List<Data> Getfeedback()
        {
            List<Data> hihi = new List<Data>();
            var feed = (from f in _context.Feedbacks
                        join c in _context.Cars
                        on f.CarId equals c.CarId
                        select new
                        {
                            Models = c.Model,
                            Rating = f.Rating
                        }).ToList();
            foreach (var c in feed)
            {
                hihi.Add(new Data() { IssueDate = c.Models, Action = c.Rating });
            }
            return hihi;
        }

        public class Datas
        {
            public double value { get; set; }
            public string name { get; set; }

        }

        public List<Datas> GetAverageFeedback()
        {
            List<Data> dataList = Getfeedback();

            var groupedData = dataList
                .GroupBy(d => d.IssueDate)
                .Select(g => new Datas
                {
                    value = g.Average(d => d.Action),
                    name = g.Key
                })
                .ToList();

            return groupedData;
        }

        public List<Trans> GetRental()
        {
            List<Trans> hihi = new List<Trans>();
            foreach (RentalContract c in _context.RentalContracts)
            {
                hihi.Add(new Trans() { IssueDate = c.StartDate.ToString("yyyy-MM"), Action = c.TotalAmount });
            }
            return AggregateTransactions(hihi).OrderBy(x => x.IssueDate).ToList();
        }
    }
}
