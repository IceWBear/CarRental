namespace Project_RentACar.Models
{
    public class SignUp
    {
        public SignUp()
        {
        }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public DateTime dob { get; set; }

        public SignUp(string firstName, string lastName, string email, string password, string address, string phone, DateTime dob)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.password = password;
            this.address = address;
            this.phone = phone;
            this.dob = dob;
        }
    }
}
