using Microsoft.AspNetCore.Mvc;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;
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
        private readonly ICombosHelper _combosHelper;

        public AccountController(IUserHelper  userHelper, 
                                 DataContext dataContext,
                                 ICombosHelper combosHelper)
        {
            this._userHelper = userHelper;
            this._dataContext = dataContext;
            this._combosHelper = combosHelper;
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





        public IActionResult Register()
        {
            var model = new AddUserViewModel
            {
                Roles = _combosHelper.GetComboRoles()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = "Owner";
                if (model.RoleId == 1)
                {
                    role = "Lessee";
                }

                var user = await _userHelper.AddUser(model, role);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already used.");
                    model.Roles = _combosHelper.GetComboRoles();
                    return View(model);
                }
                if (model.RoleId == 1)
                {
                    var lessee = new Lessee
                    {
                        Contracts = new List<Contract>(),
                        User = user
                    };
                    _dataContext.Lessees.Add(lessee);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    var owner = new Owner
                    {
                        Contracts = new List<Contract>(),
                        Properties = new List<Property>(),
                        User = user
                    };
                    _dataContext.Owners.Add(owner);
                    await _dataContext.SaveChangesAsync();
                }

                var loginViewModel = new LoginViewModel
                {
                    Password = model.Password,
                    RememberMe = false,
                    UserName = model.Username
                };

                var result2 = await _userHelper.LoginAsync(loginViewModel);

                if (result2.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            model.Roles = _combosHelper.GetComboRoles();
            return View(model);
        }



    }
}
