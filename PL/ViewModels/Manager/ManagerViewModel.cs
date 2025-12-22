using Interfaces.Services;
using System.Windows.Input;

namespace PL.ViewModels
{
    public class ManagerViewModel 
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPageNavigationWindowService _navigationPageService;
        private readonly INavigationWindowService _navigationWindowService;
        private readonly ICommand _logoutCommand;
        private readonly ICommand _goBackCommand;
        private readonly ICommand _navigateToReportsCommand;
        private readonly ICommand _navigateToApproveClaimsCommand;
        private readonly ICommand _navigateToAnnualRevenueReportCommand;

        public ManagerViewModel(ICurrentUserService currentUserService, IPageNavigationWindowService navigationPageService, INavigationWindowService navigationWindowService)
        {
            _currentUserService = currentUserService;
            _navigationPageService = navigationPageService;
            _navigationWindowService = navigationWindowService;
            _logoutCommand = new RelayCommand(_ => Logout(), (Func<bool>?)null);
            _goBackCommand = new RelayCommand(_ => _navigationPageService.GoBack(), () => _navigationPageService.CanGoBack);
            _navigateToReportsCommand = new RelayCommand(_ => _navigationPageService.NavigateTo<ReportsPage>(), (Func<bool>?)null); 
            _navigateToApproveClaimsCommand = new RelayCommand(_ => _navigationPageService.NavigateTo<ApproveClaimsPage>(), (Func<bool>?)null);
            _navigateToAnnualRevenueReportCommand = new RelayCommand(_ => _navigationPageService.NavigateTo<AnnualRevenueReportPage>(), (Func<bool>?)null); 
        }

        public ICommand LogoutCommand => _logoutCommand;
        public ICommand GoBackCommand => _goBackCommand;
        public ICommand NavigateToReportsCommand => _navigateToReportsCommand;
        public ICommand NavigateToApproveClaimsCommand => _navigateToApproveClaimsCommand;
        public ICommand NavigateToAnnualRevenueReportCommand => _navigateToAnnualRevenueReportCommand;


        private void Logout()
        {
            _currentUserService.ClearCurrentUser();
            _navigationWindowService.NavigateTo<LoginWindow>();
        }
    }
}