using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using GSL.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GSL.Controllers
{
    public class AddItemController : Controller
    {
        string garageId;

        public IActionResult AddItem(string id)
        {
            garageId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddItemAsync(GarageItemModel model, string id)
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://gslocator-6b655-default-rtdb.firebaseio.com");
                var garage = firebaseClient.Child("garage").Child(id).Child("garageItems");

                _ = garage.PostAsync(model);

                return RedirectToAction("GarageDetails", "GarageDetails", new {id = id});
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
