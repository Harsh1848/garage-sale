using Firebase.Database;
using Firebase.Database.Query;
using GSL.Models;
using GSL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GSL.Controllers
{
    public class GarageDetailsController : Controller
    {
        string garageId;

        [HttpGet]
        public async Task<IActionResult> GarageDetailsAsync(string id)
        {
            string jsonStr = Request.Cookies["UserModel"];
            UserRegistrationModel userModel = JsonConvert.DeserializeObject<UserRegistrationModel>(jsonStr);
            var CurrentTime = DateTime.Now;
            CurrentTime = CurrentTime.AddSeconds(-CurrentTime.Second);
            var firebaseClient = new FirebaseClient("https://gslocator-6b655-default-rtdb.firebaseio.com");
            var alerts = await firebaseClient.Child("alert").OnceAsync<AlertModal>();
            var filteredAlerts = alerts.Where(x =>
                                                    x.Object.currentUserId == userModel.Id &&
                                                    DateTime.Parse(x.Object.itemSellingEndDate).Year == CurrentTime.Year &&
                                                    DateTime.Parse(x.Object.itemSellingEndDate).Month == CurrentTime.Month &&
                                                    DateTime.Parse(x.Object.itemSellingEndDate).Day == CurrentTime.Day &&
                                                    DateTime.Parse(x.Object.itemSellingEndDate).Hour == CurrentTime.Hour &&
                                                    DateTime.Parse(x.Object.itemSellingEndDate).Minute == CurrentTime.Minute
                                                ).ToList();
            if (filteredAlerts.Count > 0)
            {
                TempData["FilteredAlerts"] = filteredAlerts[0].Object.itemName;
            }
            var garages = await firebaseClient.Child("garage").Child(id).Child("garageItems").OnceAsync<GarageItemModel>();
            var garageItemList = garages.Select(item => new GarageItemModel
            {
                Id = item.Key,
                ItemName = item.Object.ItemName, ItemColor = item.Object.ItemColor, ItemPrice = item.Object.ItemPrice,ItemSellingEndDate = item.Object.ItemSellingEndDate, GarageId = item.Object.Id
            }).ToList();

            string garageListStr = Request.Cookies["garage_list"];
            List<GarageModel> garageList = JsonConvert.DeserializeObject < List<GarageModel>>(garageListStr);

            GarageModel garageDetails = garageList.FirstOrDefault(item => item.Id == id);
            var garageDetailsViewModel = new GarageDetailsViewModel
            {
                GarageId = id, GarageItemList = garageItemList, UserType = userModel.UserType, Garage = garageDetails
            };
            return View(garageDetailsViewModel);
        }

        public IActionResult GarageDetails(string id)
        {
            garageId = id;
            return RedirectToAction("AddItem", "AddItem");
        }
    }
}
