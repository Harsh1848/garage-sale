using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using GSL.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GSL.Controllers
{
    public class AlertController : Controller
    {
        FirebaseAuthProvider auth;
        UserRegistrationModel userModel;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAlert([FromBody] AlertModal model)
        {
            try
            {
                string jsonStr = Request.Cookies["UserModel"];
                UserRegistrationModel userModel = JsonConvert.DeserializeObject<UserRegistrationModel>(jsonStr);

                var firebaseClient = new FirebaseClient("https://gslocator-6b655-default-rtdb.firebaseio.com");
                var garages = await firebaseClient.Child("garage").Child(model.garageId).Child("garageItems").Child(model.itemId).OnceSingleAsync<GarageItemModel>();
                //model.itemSellingEndDate = garageItem.Select(x => x.Key == model.itemId).
                model.itemSellingEndDate = garages.ItemSellingEndDate;
                model.gotIt = false;
                model.currentUserId = userModel.Id;
                model.itemName = garages.ItemName;
                var alert = firebaseClient.Child("alert");
                 _ = alert.PostAsync(model);
                TempData["AlertAdd"] = "Alert added successfully";
                return Json(new { success = true });
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
