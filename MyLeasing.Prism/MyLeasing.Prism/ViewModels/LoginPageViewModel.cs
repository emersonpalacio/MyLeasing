using MyLeasing.Common.Models;
using MyLeasing.Common.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLeasing.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {        
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private string _password;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _loginCommand;



        public LoginPageViewModel(INavigationService navigationService,
                                  IApiService apiService ) :base(navigationService)
        {
            this._navigationService = navigationService;
            this._apiService = apiService;
            Title = "login";

            IsEnabled = true;

            Email = "emersonpalaciootalvaro@gmail.com";
            Password = "123456";

        }

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(Login));

       
        public string Email { get; set; }

        public string Password 
        { 
            get => _password; 
            set => SetProperty(ref _password, value); 
        }

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

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("error","You must enter your email","Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("error", "You must enter your password", "Accept");
                return;
            }

            IsRunning = true;
            IsEnabled = false;


            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            var url = App.Current.Resources["UrlAPI"].ToString();
            var response = await _apiService.GetTokenAsync(url, "Account", "/CreateToken", request);

            if (!response.IsSuccess)
            {
                IsEnabled = true;
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert("Error", "User or password incorrect.", "Accept");
                Password = string.Empty;
                return;
            }

            IsRunning = false;
            IsEnabled = true;
            await App.Current.MainPage.DisplayAlert("Ok", "We are making progress!", "Accept");

        }


    }
}
