// PL/ViewModels/ManagerViewModel.cs
using Interfaces.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PL;

namespace PL.ViewModels
{
    public class ManagerViewModel : INotifyPropertyChanged
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationService _navigationService;
        private readonly ICommand _logoutCommand;

        public ManagerViewModel(ICurrentUserService currentUserService, INavigationService navigationService)
        {
            _currentUserService = currentUserService;
            _navigationService = navigationService;
            _logoutCommand = new RelayCommand(_ => Logout(), (Func<bool>?)null);
        }

        public ICommand LogoutCommand => _logoutCommand;

        private void Logout()
        {
            _currentUserService.ClearCurrentUser();
            _navigationService.NavigateTo<LoginWindow>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}