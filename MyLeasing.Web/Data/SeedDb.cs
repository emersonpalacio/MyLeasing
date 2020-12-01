using MyLeasing.Web.Data.Entities;
using MyLeasing.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLeasing.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext  dataContext,
                      IUserHelper userHelper)
        {
            this._dataContext = dataContext;
            this._userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();

            await CheckRolesAsync();
            var manager = await CheckUserAsync("1010", "emerson", "palacio", "emersonpalaciootalvaro@gmail.com", "350 634 2747", "Calle Luna Calle Sol", "Manager");
            User owner = await CheckUserAsync("2020", "emmanuel", "palacio", "emersonpalaciootalvaro@hotmail.com", "350 634 2747", "Calle Luna Calle Sol", "Owner");
            User lessee = await CheckUserAsync("2020", "sara", "palacio", "sara@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", "Lessee");

            await CheckPropertyTypesAsync();
            await CheckManagerAsync(manager);
            await CheckOwnersAsync(owner);
            await CheckLesseesAsync(lessee);
            await CheckPropertiesAsync();
            await CheckContractsAsync();

        }


        //****************************************************************
        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync("Manager");
            await _userHelper.CheckRoleAsync("Owner");
            await _userHelper.CheckRoleAsync("Lessee");
        }
        //****************************************************************

        private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, string role)
        {
            User user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, role);
            }

            return user;
        }

        //****************************************************************
        private async Task CheckPropertyTypesAsync()
        {
            if (!_dataContext.PropertyTypes.Any())
            {
                _dataContext.PropertyTypes.Add(new PropertyType { Name = "Apartment" });
                _dataContext.PropertyTypes.Add(new PropertyType { Name = "House" });
                _dataContext.PropertyTypes.Add(new PropertyType { Name = "Deal" });
                await _dataContext.SaveChangesAsync();
            }
        }

        //****************************************************************
        private async Task CheckManagerAsync(User user)
        {
            if (!_dataContext.Managers.Any())
            {
                _dataContext.Managers.Add(new Manager { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        //****************************************************************
        private async Task CheckOwnersAsync(User user)
        {
            if (!_dataContext.Owners.Any())
            {
                _dataContext.Owners.Add(new Owner { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }
        //****************************************************************
        private async Task CheckLesseesAsync(User user)
        {
            if (!_dataContext.Lessees.Any())
            {
                _dataContext.Lessees.Add(new Lessee { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        //****************************************************************
        private async Task CheckPropertiesAsync()
        {
            Owner owner = _dataContext.Owners.FirstOrDefault();
            PropertyType propertyType = _dataContext.PropertyTypes.FirstOrDefault();
            if (!_dataContext.Properties.Any())
            {
                AddProperty("Calle 43 #23 32", "Poblado", owner, propertyType, 800000M, 2, 72, 4);
                AddProperty("Calle 12 Sur #2 34", "Envigado", owner, propertyType, 950000M, 3, 81, 3);
                await _dataContext.SaveChangesAsync();
            }
        }
        private void AddProperty(string address, string neighborhood, Owner owner, PropertyType propertyType, decimal price, int rooms, int squareMeters, int stratum)
        {
            _dataContext.Properties.Add(new Property
            {
                Address = address,
                HasParkingLot = true,
                IsAvailable = true,
                Neighborhood = neighborhood,
                Owner = owner,
                Price = price,
                PropertyType = propertyType,
                Rooms = rooms,
                SquareMeters = squareMeters,
                Stratum = stratum
            });
        }

        //****************************************************************
        private async Task CheckContractsAsync()
        {
            Owner owner = _dataContext.Owners.FirstOrDefault();
            Lessee lessee = _dataContext.Lessees.FirstOrDefault();
            Property property = _dataContext.Properties.FirstOrDefault();
            if (!_dataContext.Contracts.Any())
            {
                _dataContext.Contracts.Add(new Contract
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddYears(1),
                    IsActive = true,
                    Lessee = lessee,
                    Owner = owner,
                    Price = 800000M,
                    Property = property,
                    Remarks = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris nec iaculis ex. Nullam gravida nunc eleifend, placerat tellus a, eleifend metus. Phasellus id suscipit magna. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Nullam volutpat ultrices ex, sed cursus sem tincidunt ut. Nullam metus lorem, convallis quis dignissim quis, porttitor quis leo. In hac habitasse platea dictumst. Duis pharetra sed arcu ac viverra. Proin dapibus lobortis commodo. Vivamus non commodo est, ac vehicula augue. Nam enim felis, rutrum in tortor sit amet, efficitur hendrerit augue. Cras pellentesque nisl eu maximus tempor. Curabitur eu efficitur metus. Sed ultricies urna et auctor commodo."
                });

                await _dataContext.SaveChangesAsync();
            }
        }


    }
}
