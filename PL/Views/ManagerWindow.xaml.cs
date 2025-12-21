// PL/Views/ManagerWindow.xaml.cs
using Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using PL;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Для ICommand

namespace PL
{
    public partial class ManagerWindow : Window, INotifyPropertyChanged
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationService _navigationService;
        private readonly ICommand _goBackCommand;
        private readonly ICommand _logoutCommand;

        public ManagerWindow()
        {
            InitializeComponent();
            // Получаем сервисы из DI-контейнера
            _currentUserService = App.ServiceProvider.GetRequiredService<ICurrentUserService>();
            _navigationService = App.ServiceProvider.GetRequiredService<INavigationService>();

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
                Title = $"Панель менеджера - {user.Login}";
                WelcomeText.Text = $"Добро пожаловать, {user.Login}!";
            }
        }

        private void Logout()
        {
            _currentUserService.ClearCurrentUser();
            _navigationService.NavigateTo<LoginWindow>();
        }

        private void OnReportsClick(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<ReportsWindow>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}