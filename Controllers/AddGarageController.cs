using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using GSL.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace GSL.Controllers
{
    public class AddGarageController : Controller
    {
        FirebaseAuthProvider auth;
        UserRegistrationModel userModel;

        public AddGarageController() {
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBnzcYAd1D8h0d-UdGCj5q4iUA88e2CRnQ"));
        }

        public ActionResult AddGarage() { 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddGarageAsync(GarageModel model)
        {
            try
            {
                string jsonStr = Request.Cookies["UserModel"];
                userModel = JsonConvert.DeserializeObject<UserRegistrationModel>(jsonStr);
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
                var garage = firebaseClient.Child("garage");
                
                _ = garage.PostAsync(model);

                return RedirectToAction("ProviderHome", "ProviderHome");
            }
            catch (FirebaseAuthException ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                ViewBag.ErrorMessage = ex.Message;
                return View("Register");
            }
        }
    }
}
