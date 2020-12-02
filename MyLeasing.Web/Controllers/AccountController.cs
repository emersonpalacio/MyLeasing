using Microsoft.AspNetCore.Mvc;
using MyLeasing.Web.Data;
using MyLeasing.Web.Helpers;
using MyLeasing.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLeasing.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly DataContext _dataContext;

        public AccountController(IUserHelper  userHelper, DataContext dataContext)
        {
            this._userHelper = userHelper;
            this._dataContext = dataContext;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated )
            {
                return RedirectToAction("Index","Home");
            }
         

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnoUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());

                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Failed to login.");
                return View(model);
            }


            return View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }


    }
}
