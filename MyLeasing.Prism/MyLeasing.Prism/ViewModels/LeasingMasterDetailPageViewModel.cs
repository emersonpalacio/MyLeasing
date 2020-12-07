using MyLeasing.Common.Models;
using MyLeasing.Prism.ItemViewModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyLeasing.Prism.ViewModels
{
    public class LeasingMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public LeasingMasterDetailPageViewModel(INavigationService navigationService): base(navigationService)
        {
            this._navigationService = navigationService;
            LoadMenus();
        }


        public ObservableCollection<MenuItemViewModel> Menus { get; set; }

        private void LoadMenus()
        {
            var menus = new List<Menu>
            {
                new Menu
                {
                    Icon = "ic_action_home",
                    PageName = "PropertiesPage",
                    Title = "Propierties"
                },

                new Menu
                {
                    Icon = "ic_action_list_alt",
                    PageName = "ContractsPage",
                    Title = "Contracts"
                },

                new Menu
                {
                    Icon = "ic_action_person",
                    PageName = "ModifyUserPage",
                    Title = "Modify User"
                },

                new Menu
                {
                    Icon = "ic_action_map",
                    PageName = "MapPage",
                    Title = "Map"
                },

                new Menu
                {
                    Icon = "ic_action_exit_to_app",
                    PageName = "LoginPage",
                    Title = "Log out"
                }
            };

            Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel(_navigationService)
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title
                }).ToList());
        }

    }
}
