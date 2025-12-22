using Interfaces.Services;
using System.Windows.Input;

namespace PL.ViewModels
{
    public class MainViewModel
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPageNavigationService _navigationPageService;
        private readonly INavigationService _navigationWindowService;
        private readonly ICommand _goBackCommand;
        private readonly ICommand _logoutCommand;
        private readonly ICommand _navigateToVehiclesCommand;
        private readonly ICommand _navigateToPoliciesCommand;
        private readonly ICommand _navigateToClaimsCommand;
        private readonly ICommand _navigateToPaymentsCommand;

        public MainViewModel(ICurrentUserService currentUserService, IPageNavigationService navigationPageService, INavigationService navigationWindowService)
        {
            _currentUserService = currentUserService;
            _navigationPageService = navigationPageService;
            _goBackCommand = new RelayCommand(_ => _navigationPageService.GoBack(), () => _navigationPageService.CanGoBack);
            _logoutCommand = new RelayCommand(_ => Logout(), (Func<bool>?)null);
            _navigateToVehiclesCommand = new RelayCommand(_ => _navigationPageService.NavigateTo<VehiclesPage>(), (Func<bool>?)null);
            _navigateToPoliciesCommand = new RelayCommand(_ => _navigationPageService.NavigateTo<PoliciesPage>(), (Func<bool>?)null);
            _navigateToClaimsCommand = new RelayCommand(_ => _navigationPageService.NavigateTo<ClaimsPage>(), (Func<bool>?)null);
            _navigateToPaymentsCommand = new RelayCommand(_ => _navigationPageService.NavigateTo<PaymentsPage>(), (Func<bool>?)null);
            _navigationWindowService = navigationWindowService;
        }

        public ICurrentUserService CurrentUserService => _currentUserService;

        public ICommand GoBackCommand => _goBackCommand;
        public ICommand LogoutCommand => _logoutCommand;
        public ICommand NavigateToVehiclesCommand => _navigateToVehiclesCommand;
        public ICommand NavigateToPoliciesCommand => _navigateToPoliciesCommand;
        public ICommand NavigateToClaimsCommand => _navigateToClaimsCommand;
        public ICommand NavigateToPaymentsCommand => _navigateToPaymentsCommand;

        private void Logout()
        {
            _currentUserService.ClearCurrentUser();
            _navigationWindowService.NavigateTo<LoginWindow>();
        }
    }
}