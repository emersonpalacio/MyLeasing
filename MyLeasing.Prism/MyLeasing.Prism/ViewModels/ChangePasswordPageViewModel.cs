using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLeasing.Prism.ViewModels
{
    public class ChangePasswordPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ChangePasswordPageViewModel(INavigationService navigationService):base(navigationService)
        {
            this._navigationService = navigationService;
            Title = "ChangePassword";
        }
    }
}
