using Interfaces.Services;
using System.Windows.Input;

namespace PL.ViewModels
{
    public class ManagerViewModel
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationService _navigationService;
        private readonly ICommand _logoutCommand;
        private readonly ICommand _goBackCommand;
        private readonly ICommand _navigateToReportsCommand;
        private readonly ICommand _navigateToApproveClaimsCommand;
        private readonly ICommand _navigateToAnnualRevenueReportCommand;

        public ManagerViewModel(ICurrentUserService currentUserService, INavigationService navigationService)
        {
            _currentUserService = currentUserService;
            _navigationService = navigationService;
            _logoutCommand = new RelayCommand(_ => Logout(), (Func<bool>?)null);
            _goBackCommand = new RelayCommand(_ => _navigationService.GoBack(), () => _navigationService.CanGoBack);
            _navigateToReportsCommand = new RelayCommand(_ => _navigationService.NavigateTo<ReportsWindow>(), (Func<bool>?)null);
            _navigateToApproveClaimsCommand = new RelayCommand(_ => _navigationService.NavigateTo<ApproveClaimsWindow>(), (Func<bool>?)null);
            _navigateToAnnualRevenueReportCommand = new RelayCommand(_ => _navigationService.NavigateTo<AnnualRevenueReportWindow>(), (Func<bool>?)null);
        }

        public ICommand LogoutCommand => _logoutCommand;
        public ICommand GoBackCommand => _goBackCommand;
        public ICommand NavigateToReportsCommand => _navigateToReportsCommand;
        public ICommand NavigateToApproveClaimsCommand => _navigateToApproveClaimsCommand;
        public ICommand NavigateToAnnualRevenueReportCommand => _navigateToAnnualRevenueReportCommand;

        private void Logout()
        {
            _currentUserService.ClearCurrentUser();
            _navigationService.NavigateTo<LoginWindow>();
        }
    }
}