using Prism.Navigation;

namespace MyLeasing.Prism.ViewModels
{
    public class ModifyUserPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ModifyUserPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "ModifyUser ";
        }
    }
}
