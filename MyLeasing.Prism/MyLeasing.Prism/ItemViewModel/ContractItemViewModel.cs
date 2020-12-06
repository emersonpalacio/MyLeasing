using MyLeasing.Common.Models;
using Prism.Commands;
using Prism.Navigation;

namespace MyLeasing.Prism.ItemViewModel
{
    public class ContractItemViewModel : ContractResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectContractCommand;

        public ContractItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectContractCommand => 
            _selectContractCommand ?? (_selectContractCommand = new DelegateCommand(SelectContract));

        private async void SelectContract()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { "contract", this }
            };

            await _navigationService.NavigateAsync("ContractPage", parameters);
        }
    }
}
