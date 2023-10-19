namespace GSL.Models
{
    public class UserRegistrationModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string UserType { get; set; } // Customer or SalesProvider
        public string Password { get; set; }

        public string Id { get; set; }
    }

}
