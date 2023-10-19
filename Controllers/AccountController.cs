using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using GSL.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Xml.Linq;

namespace GSL.Controllers
{
    public class AccountController : Controller
    {
        FirebaseAuthProvider auth;

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationModel model)
        {
            try
            {
                //create the user
                var authResult = await auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password);

                var userId = authResult.User.LocalId;

                var firebaseClient = new FirebaseClient("https://gslocator-6b655-default-rtdb.firebaseio.com");
                var users = firebaseClient.Child("users");

                model.Id = userId;

                _ = users.Child(userId).PutAsync(model);

                TempData["User"] = JsonConvert.SerializeObject(model);

                return RedirectToAction("Login", "Account");
            }
            catch (FirebaseAuthException ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                ViewBag.ErrorMessage = ex.Message;
                return View("Register");
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        public AccountController() {
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBnzcYAd1D8h0d-UdGCj5q4iUA88e2CRnQ"));
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                //create the user
                var authResult = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);

                if (authResult != null && authResult.User != null)
                {
                    var userId = authResult.User.LocalId;

                    var firebaseClient = new FirebaseClient("https://gslocator-6b655-default-rtdb.firebaseio.com");
                    var users = await firebaseClient.Child("users").Child(userId).OnceSingleAsync<UserRegistrationModel>();
                    UserRegistrationModel userModel = new UserRegistrationModel
                    {
                        Name = users.Name,
                        Email = users.Email,
                        Password = users.Password,
                        PhoneNumber = users.PhoneNumber,
                        Address = users.Address,
                        Id = users.Id,
                        UserType = users.UserType
                    };

                    TempData["User"] = JsonConvert.SerializeObject(userModel);

                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(1),
                        HttpOnly = true
                    };

                    Response.Cookies.Append("UserModel", JsonConvert.SerializeObject(userModel), cookieOptions);

                    if (userModel.UserType == "Customer" || userModel.UserType == "customer")
                    {
                        return RedirectToAction("CustomerHome", "CustomerHome");
                    }
                    else {
                        return RedirectToAction("ProviderHome", "ProviderHome");
                    }
                    
                }
                else
                {
                    ViewBag.ErrorMessage = "Email ID does not exist. Please register email id first and then login again.";
                }

                return View("Login");
            }
            catch (FirebaseAuthException ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                ViewBag.ErrorMessage = ex.Message;
                return View("Login");
            }
        }

    }
}