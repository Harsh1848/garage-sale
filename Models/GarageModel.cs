namespace GSL.Models
{
    public class GarageModel
    {
        public string GarageName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string OwnerName { get; set; }

        public string OwnerPhoneNumber { get; set; }

        public string Id { get; set; }

        public List<GarageItemModel> GarageItemModel { get; set; }
    }
}
