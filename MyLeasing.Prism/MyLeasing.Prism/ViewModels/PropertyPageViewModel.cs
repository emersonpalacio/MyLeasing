using MyLeasing.Common.Helpers;
using MyLeasing.Common.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyLeasing.Prism.ViewModels
{
    public class PropertyPageViewModel : ViewModelBase
    {
        private PropertyResponse _property;
        private ObservableCollection<RotatorModel> _imageCollection;
        private DelegateCommand _editPropertyCommand;
        private readonly INavigationService _navigationService;

        public PropertyPageViewModel(INavigationService navigationService):base(navigationService)
        {
            Title ="Details";
            Property = JsonConvert.DeserializeObject<PropertyResponse>(Settings.Property);
            LoadImages();
            this._navigationService = navigationService;
        }

        public DelegateCommand EditPropertyCommand => _editPropertyCommand ?? (_editPropertyCommand =new DelegateCommand(EditPropertyAsync) );

       

        public PropertyResponse Property 
        { 
            get => _property; 
            set => SetProperty(ref _property,value); 
        }

        public ObservableCollection<RotatorModel> ImageCollection
        {
            get => _imageCollection;
            set => SetProperty(ref _imageCollection, value);
        }

     

        //public override void OnNavigatedTo(INavigationParameters parameters)
        //{
        //    base.OnNavigatedTo(parameters);

        //    if (parameters.ContainsKey("property"))
        //    {
        //        Property = parameters.GetValue<PropertyResponse>("property");
        //        Title = $"Propiedad: { Property.Neighborhood}";
        //    }
        //    LoadImages();
        //}

        private void LoadImages()
        {
            var list = new List<RotatorModel>();
            foreach (var propertyImage in Property.PropertyImages)
            {
                list.Add(new RotatorModel { Image = propertyImage.ImageUrl });
            }

            ImageCollection = new ObservableCollection<RotatorModel>(list);
        }

        private async void EditPropertyAsync()
        {
            var parameters = new NavigationParameters();
            parameters.Add("property",Property);
            await _navigationService.NavigateAsync("EditProperty", parameters);
        }

    }
}
