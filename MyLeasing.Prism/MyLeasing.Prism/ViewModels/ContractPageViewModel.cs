using MyLeasing.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLeasing.Prism.ViewModels
{
    public class ContractPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private ContractResponse _contrat;

        public ContractPageViewModel(INavigationService navigationService): base(navigationService)
        {
            this._navigationService = navigationService;

            Title = "Contrat";
        }

        public ContractResponse Contract 
        { 
            get => _contrat; 
            set => SetProperty(ref _contrat, value); 
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("contract"))
            {
                Contract = parameters.GetValue<ContractResponse>("contract");
                Title = $"Contract to: {Contract.Lessee.FullName}";
            }
        }


    }
}
