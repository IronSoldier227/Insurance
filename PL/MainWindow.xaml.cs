// PL/MainWindow.xaml.cs
using Interfaces.Services;
using PL;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Для ICommand
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using Microsoft.Extensions.DependencyInjection; // Для LINQ

namespace PL
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationService _navigationService;
        private readonly ICommand _goBackCommand;
        private readonly ICommand _logoutCommand;

        public MainWindow()
        {
            InitializeComponent();
            _currentUserService = App.ServiceProvider.GetRequiredService<ICurrentUserService>();
            _navigationService = App.ServiceProvider.GetRequiredService<INavigationService>();

            // Используем Func<bool> для CanExecute
            _goBackCommand = new RelayCommand(_ => _navigationService.GoBack(), () => _navigationService.CanGoBack);
            _logoutCommand = new RelayCommand(_ => Logout(), (Func<bool>?)null);

            Loaded += OnLoaded;
        }

        public ICommand GoBackCommand => _goBackCommand;
        public ICommand LogoutCommand => _logoutCommand;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Показать приветствие
            var user = _currentUserService.GetCurrentUser();
            if (user != null)
            {
                Title = $"Страхование автомобилей - {user.Login}";
                WelcomeText.Text = $"Добро пожаловать, {user.Login}!";
            }
        }

        private void Logout()
        {
            _currentUserService.ClearCurrentUser();
            _navigationService.NavigateTo<LoginWindow>();
        }

        private void OnMyVehiclesClick(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<VehiclesWindow>();
        }

        // --- Новый обработчик ---
        private void OnMyPoliciesClick(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<PoliciesWindow>();
        }
        // --- 

        private void OnMyClaimsClick(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<ClaimsWindow>();
        }

        private void OnNewClaimClick(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<RegisterClaimWindow>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}