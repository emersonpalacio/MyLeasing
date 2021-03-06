﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyLeasing.Web.Data;
using MyLeasing.Web.Data.Entities;
using MyLeasing.Web.Helpers;
using MyLeasing.Web.Models;

namespace MyLeasing.Web.Controllers
{
    public class OwnersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _comboshelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly UserManager<User> _userManager;
        private readonly IMailHelper _mailHelper;

        public OwnersController(DataContext context,
                                IUserHelper userHelper,
                                ICombosHelper comboshelper,
                                IConverterHelper converterHelper,
                                IImageHelper imageHelper,
                                UserManager<User> userManager,
                                IMailHelper  mailHelper)
        {
            _context = context;
            this._userHelper = userHelper;
            this._comboshelper = comboshelper;
            this._converterHelper = converterHelper;
            this._imageHelper = imageHelper;
            this._userManager = userManager;
            this._mailHelper = mailHelper;
        }

        // GET: Owners
        public async Task<IActionResult> Index()
        {
            return View(await _context.Owners.ToListAsync());
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                                             .Include(o => o.User)
                                             .Include(o => o.Properties)
                                             .ThenInclude(p => p.PropertyType)
                                             .Include(o => o.Properties)
                                             .ThenInclude(o => o.PropertyImages)
                                             .Include(o => o.Contracts)
                                             .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create
        public IActionResult Create()
        {
            var model = new AddUserViewModel { RoleId = 2 };
            return View(model);
        }


        // POST: Owners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await CreateUserAsync(model);

                if (user != null)
                {
                    var owner = new Owner
                    {
                        Contracts = new List<Contract>(),
                        Properties = new List<Property>(),
                        User = user
                    };

                    //TODO: Problema al guardars el owner
                    _context.Add(owner);
                    await _context.SaveChangesAsync();

                    var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    var tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    _mailHelper.SendMail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");


                    return RedirectToAction(nameof(Index));

                }
                ModelState.AddModelError(string.Empty, "user meal  ready exist");
            }
            return View(model);
        }

        private async Task<User> CreateUserAsync(AddUserViewModel model)
        {
            var user = new User { 
                Address= model.Address,
                Document =  model.Document,
                Email = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Username
            };

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result.Succeeded)
            {
                user = await _userHelper.GetUserByEmailAsync(model.Username);
                await _userHelper.AddUserToRoleAsync(user, "Owner");
                return user;
            }
            return null;        
        }



       

        

        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();
            await _userHelper.DeleteUserAsync(owner.User.Email);
            return RedirectToAction(nameof(Index));
        }



        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = await _context.Owners.FindAsync(id);
            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OwnerExists(int id)
        {
            return _context.Owners.Any(e => e.Id == id);
        }

        public async Task<IActionResult> AddProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners.FindAsync(id);

            if (owner == null)
            {
                return NotFound();
            }

            var model = new PropertyViewModel
            {
                OwnerId = owner.Id,
                PropertyTypes = _comboshelper.GetComboPropertyTypes()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddProperty(PropertyViewModel model)
        {

            if (ModelState.IsValid)
            {
                var property = await _converterHelper.TopropertyAsync(model, true);
                _context.Properties.Add(property);
                await _context.SaveChangesAsync();
                return RedirectToAction($"Details/{model.OwnerId}");
            }
            model.PropertyTypes = _comboshelper.GetComboPropertyTypes();//no dejar perder combo
            return View(model);
        }



        public async Task<IActionResult> DetailsProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(o => o.Owner)
                .ThenInclude(o => o.User)
                .Include(o => o.Contracts)
                .ThenInclude(c => c.Lessee)
                .ThenInclude(l => l.User)
                .Include(o => o.PropertyType)
                .Include(p => p.PropertyImages)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        public async Task<IActionResult> AddImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties.FindAsync(id.Value);
            if (property == null)
            {
                return NotFound();
            }

            var model = new PropertyImageViewModel
            {
                Id = property.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddImage(PropertyImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile);
                }

                var propertyImage = new PropertyImage
                {
                    ImageUrl = path,
                    Property = await _context.Properties.FindAsync(model.Id)
                };

                _context.PropertyImages.Add(propertyImage);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsProperty)}/{model.Id}");
            }

            return View(model);
        }




        public async Task<IActionResult> EditProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                                                   .Include(p => p.Owner)
                                                   .Include(p => p.PropertyType)
                                                   .FirstOrDefaultAsync(p => p.Id == id);
            if (property == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToPropertyViewModel(property);

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> EditProperty(PropertyViewModel model)
        {

            if (ModelState.IsValid)
            {
                var property = await _converterHelper.TopropertyAsync(model, false);
                _context.Properties.Update(property);
                await _context.SaveChangesAsync();
                return RedirectToAction($"Details/{model.OwnerId}");
            }
            return View(model);
        }



        public async Task<IActionResult> AddContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (property == null)
            {
                return NotFound();
            }

            var model = new ContractViewModel
            {
                OwnerId = property.Owner.Id,
                PropertyId = property.Id,
                Lessees = _comboshelper.GetComboLessees(),
                Price = property.Price,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddContract(ContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                var contract = await _converterHelper.ToContractAsync(model, true);
                _context.Contracts.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsProperty)}/{model.PropertyId}");
            }

            //public async Task<IActionResult> AddContract(ContractViewModel model) //actualize el combo
            return View(model);
        }




        public async Task<IActionResult> EditContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(p => p.Owner)
                .Include(p => p.Lessee)
                .Include(p => p.Property)
                .FirstOrDefaultAsync(p => p.Id == id.Value);
            if (contract == null)
            {
                return NotFound();
            }

            return View(_converterHelper.ToContractViewModel(contract));
        }


        [HttpPost]
        public async Task<IActionResult> EditContract(ContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                var contract = await _converterHelper.ToContractAsync(model, false);
                _context.Contracts.Update(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction($"{nameof(DetailsProperty)}/{model.PropertyId}");
            }

            return View(model);
        }




        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyImage = await _context.PropertyImages
                .Include(pi => pi.Property)
                .FirstOrDefaultAsync(pi => pi.Id == id.Value);
            if (propertyImage == null)
            {
                return NotFound();
            }

            _context.PropertyImages.Remove(propertyImage);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsProperty)}/{propertyImage.Property.Id}");
        }

        public async Task<IActionResult> DeleteContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Property)
                .FirstOrDefaultAsync(c => c.Id == id.Value);
            if (contract == null)
            {
                return NotFound();
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(DetailsProperty)}/{contract.Property.Id}");
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (owner == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Address = owner.User.Address,
                Document = owner.User.Document,
                FirstName = owner.User.FirstName,
                Id = owner.Id,
                LastName = owner.User.LastName,
                PhoneNumber = owner.User.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var owner = await _context.Owners
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                owner.User.Document = model.Document;
                owner.User.FirstName = model.FirstName;
                owner.User.LastName = model.LastName;
                owner.User.Address = model.Address;
                owner.User.PhoneNumber = model.PhoneNumber;

                await _userHelper.UpdateUserAsync(owner.User);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteProperty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.PropertyImages)
                .Include(p => p.Contracts)
                .FirstOrDefaultAsync(pi => pi.Id == id.Value);
            if (property == null)
            {
                return NotFound();
            }
            if (property.Contracts.Count != 0)
            {
                ModelState.AddModelError(string.Empty, "Contan relation register");

            }
            _context.PropertyImages.RemoveRange(property.PropertyImages);
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{property.Owner.Id}");
        }


        public async Task<IActionResult> DetailsContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Owner)
                .ThenInclude(o => o.User)
                .Include(c => c.Lessee)
                .ThenInclude(o => o.User)
                .Include(c => c.Property)
                .ThenInclude(p => p.PropertyType)
                .FirstOrDefaultAsync(pi => pi.Id == id.Value);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }



        ///////////////////////////////////////////////////////////////


        public IActionResult NotAuthorized()
        {
            return View();
        }






    }
}
