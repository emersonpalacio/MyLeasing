using MyLeasing.Common.Helpers;
using MyLeasing.Common.Models;
using MyLeasing.Common.Services;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace MyLeasing.Prism.ViewModels
{
    public class EditPropertyViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private PropertyResponse _property;
        private ImageSource _imageSource;
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isEdit;

        private ObservableCollection<PropertyTypeResponse> _propertyTypes;
        private PropertyTypeResponse _propertyType;
        private ObservableCollection<Stratum> _stratums;
        private Stratum _stratum;
        private DelegateCommand _editPropertyCommand;



        public EditPropertyViewModel(INavigationService navigationService,
                                    IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Edit property";
            IsEnabled = true;
        }

        public DelegateCommand EditPropertyCommand => _editPropertyCommand ?? (_editPropertyCommand = new DelegateCommand(EditProperty));

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public PropertyResponse Property
        {
            get => _property;
            set => SetProperty(ref _property, value);
        }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }


        public ObservableCollection<PropertyTypeResponse> PropertyTypes
        {
            get => _propertyTypes;
            set => SetProperty(ref _propertyTypes, value);
        }

        public PropertyTypeResponse PropertyType
        {
            get => _propertyType;
            set => SetProperty(ref _propertyType, value);
        }

        public ObservableCollection<Stratum> Stratums
        {
            get => _stratums;
            set => SetProperty(ref _stratums, value);
        }

        public Stratum Stratum
        {
            get => _stratum;
            set => SetProperty(ref _stratum, value);
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("property"))
            {
                Property = parameters.GetValue<PropertyResponse>("property");
                ImageSource = Property.Firstimage;
                IsEdit = true;
                Title = "EditProperty";
            }
            else
            {
                Property = new PropertyResponse { IsAvailable = true };
                ImageSource = "noimage";
                IsEdit = false;
                Title = "NewProperty";
            }

            LoadPropertyTypes();
            LoadStratums();

        }

        private void LoadStratums()
        {
            Stratums = new ObservableCollection<Stratum>();
            for (int i = 1; i <= 6; i++)
            {
                Stratums.Add(new Stratum { Id = i, Name = $"{i}" });
            }

            Stratum = Stratums.FirstOrDefault(s => s.Id == Property.Stratum);
        }

        private async void LoadPropertyTypes()
        {
            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.GetListAsync<PropertyTypeResponse>(url,
                "/api",
                "/PropertyTypes",
                "bearer",
                token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            var propertyTypes = (List<PropertyTypeResponse>)response.Result;
            PropertyTypes = new ObservableCollection<PropertyTypeResponse>(propertyTypes);

            if (!string.IsNullOrEmpty(Property.PropertyType))
            {
                PropertyType = PropertyTypes.FirstOrDefault(pt => pt.Name == Property.PropertyType);
            }
        }


        private async void EditProperty()
        {
            var parameters = new NavigationParameters
            {
                { "property", _property }
            };

            await _navigationService.NavigateAsync("EditProperty", parameters);
        }


    }
}
