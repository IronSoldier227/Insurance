using Interfaces.Services;
using System.Windows.Input;

namespace PL.ViewModels
{
    public class MainViewModel
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationService _navigationService;
        private readonly ICommand _goBackCommand;
        private readonly ICommand _logoutCommand;
        private readonly ICommand _navigateToVehiclesCommand;
        private readonly ICommand _navigateToPoliciesCommand;
        private readonly ICommand _navigateToClaimsCommand;
        private readonly ICommand _navigateToPaymentsCommand;

        public MainViewModel(ICurrentUserService currentUserService, INavigationService navigationService)
        {
            _currentUserService = currentUserService;
            _navigationService = navigationService;
            _goBackCommand = new RelayCommand(_ => _navigationService.GoBack(), () => _navigationService.CanGoBack);
            _logoutCommand = new RelayCommand(_ => Logout(), (Func<bool>?)null);
            _navigateToVehiclesCommand = new RelayCommand(_ => _navigationService.NavigateTo<VehiclesWindow>(), (Func<bool>?)null);
            _navigateToPoliciesCommand = new RelayCommand(_ => _navigationService.NavigateTo<PoliciesWindow>(), (Func<bool>?)null);
            _navigateToClaimsCommand = new RelayCommand(_ => _navigationService.NavigateTo<ClaimsWindow>(), (Func<bool>?)null);
            _navigateToPaymentsCommand = new RelayCommand(_ => _navigationService.NavigateTo<PaymentsWindow>(), (Func<bool>?)null);
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
            _navigationService.NavigateTo<LoginWindow>();
        }
    }
}