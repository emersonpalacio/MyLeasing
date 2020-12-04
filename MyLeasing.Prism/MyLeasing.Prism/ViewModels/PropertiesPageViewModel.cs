using MyLeasing.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyLeasing.Prism.ViewModels
{
    public class PropertiesPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private OwnerResponse _owner;
        private ObservableCollection<PropertyResponse> _properties;

        public PropertiesPageViewModel(INavigationService navigationService):base(navigationService)
        {
            this._navigationService = navigationService;
            Title = "Properties";
        }

        public ObservableCollection<PropertyResponse> Properties
        {
            get => _properties;
            set => SetProperty(ref _properties, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("owner"))
            {
                _owner = parameters.GetValue<OwnerResponse>("owner");
                Title = $"properties of : {_owner.FullName}";
                Properties = new ObservableCollection<PropertyResponse>(_owner.Properties);
            }
        }
    }
}
