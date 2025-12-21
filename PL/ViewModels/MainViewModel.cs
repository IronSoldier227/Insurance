// PL/ViewModels/MainViewModel.cs
using Interfaces.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PL;

namespace PL.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ICurrentUserService _currentUserService; // <-- Добавим свойство
        private readonly INavigationService _navigationService;
        private readonly ICommand _goBackCommand;
        private readonly ICommand _logoutCommand;

        public MainViewModel(ICurrentUserService currentUserService, INavigationService navigationService)
        {
            _currentUserService = currentUserService; // <-- Сохраняем
            _navigationService = navigationService;
            _goBackCommand = new RelayCommand(_ => _navigationService.GoBack(), () => _navigationService.CanGoBack);
            _logoutCommand = new RelayCommand(_ => Logout(), (Func<bool>?)null);
        }

        public ICurrentUserService CurrentUserService => _currentUserService; // <-- Добавим свойство для доступа из View

        public ICommand GoBackCommand => _goBackCommand;
        public ICommand LogoutCommand => _logoutCommand;

        private void Logout()
        {
            _currentUserService.ClearCurrentUser();
            _navigationService.NavigateTo<LoginWindow>();
        }

        public ICommand MyVehiclesCommand => new RelayCommand(_ => _navigationService.NavigateTo<VehiclesWindow>(), (Func<bool>?)null);
        public ICommand MyPoliciesCommand => new RelayCommand(_ => System.Windows.MessageBox.Show("Открыть список страховок"), (Func<bool>?)null);
        public ICommand NewClaimCommand => new RelayCommand(_ => System.Windows.MessageBox.Show("Создать новую заявку"), (Func<bool>?)null);

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}