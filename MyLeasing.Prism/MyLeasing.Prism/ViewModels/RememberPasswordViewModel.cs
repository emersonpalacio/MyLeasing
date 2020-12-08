﻿using MyLeasing.Common.Helpers;
using MyLeasing.Common.Models;
using MyLeasing.Common.Services;
using Prism.Commands;
using Prism.Navigation;
using System.Threading.Tasks;

namespace MyLeasing.Prism.ViewModels
{
    public class RememberPasswordViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _recoverCommand;

        public RememberPasswordViewModel(INavigationService navigationService,
                                         IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Remember Password";

            IsEnabled = true;
        }

        public DelegateCommand RecoverCommand => _recoverCommand ?? (_recoverCommand = new DelegateCommand(Recover));

        public string Email { get; set; }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private async void Recover()
        {
            bool isValid = await ValidateData();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            EmailRequest request = new EmailRequest
            {
                Email = Email
            };

            string url = App.Current.Resources["UrlAPI"].ToString();
            Response<object> response = await _apiService.RecoverPasswordAsync(
                url,
                "/api",
                "/Account/RecoverPassword",
                request);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept");
                return;
            }

            await App.Current.MainPage.DisplayAlert(
                "Ok",
                response.Message,
                "Accept");
            await _navigationService.GoBackAsync();
        }

        private async Task<bool> ValidateData()
        {
            if (string.IsNullOrEmpty(Email) || !RegexHelper.IsValidEmail(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Email Error", "Accept");
                return false;
            }
            return true;
        }





    }
}
