using Firebase.Database;
using GSL.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GSL.Controllers
{
    public class ProviderHomeController : Controller
    {

        public async Task<IActionResult> Profile()
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
            return View(userModel);
        }

        public async Task<IActionResult> ProviderHomeAsync()
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
            var garages = await firebaseClient.Child("garage").OnceAsync<GarageModel>();
            var garageList = garages.Select(item => new GarageModel
            {
                Id = item.Key,
                Address = item.Object.Address,
                GarageName = item.Object.GarageName,
                City = item.Object.City,
                State = item.Object.State,
                Country = item.Object.Country,
                PostalCode = item.Object.PostalCode,
                OwnerName = item.Object.OwnerName,
                OwnerPhoneNumber = item.Object.OwnerPhoneNumber,
            }).ToList();

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                HttpOnly = true
            };

            Response.Cookies.Append("garage_list", JsonConvert.SerializeObject(garageList), cookieOptions);

            return View(garageList);
        }

        public IActionResult AddGarage()
        {
            return RedirectToAction("AddGarage", "AddGarage");
        }
    }
}
