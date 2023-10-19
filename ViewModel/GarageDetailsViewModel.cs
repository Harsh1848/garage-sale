using GSL.Models;

namespace GSL.ViewModel
{
    public class GarageDetailsViewModel
    {
        public string GarageId { get; set; }

        public string UserType { get; set; }

        public GarageModel Garage { get; set; }

        public List<GarageItemModel> GarageItemList { get; set; }
    }
}
