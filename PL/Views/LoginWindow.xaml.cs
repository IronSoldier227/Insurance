// PL/Views/LoginWindow.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows;
using System.Windows.Controls; // Необходимо для PasswordBox

namespace PL
{
    public partial class LoginWindow : Window
    {
        // private readonly INavigationService _navigationService; // Больше не нужно хранить, если получаем через DI

        // Конструктор без параметров (для Activator.CreateInstance)
        public LoginWindow()
        {
            InitializeComponent();
            // Получаем NavigationService и ViewModel из DI-контейнера
            var navigationService = App.ServiceProvider.GetRequiredService<INavigationService>();
            var viewModel = App.ServiceProvider.GetRequiredService<LoginViewModel>();
            this.DataContext = viewModel; // Устанавливаем ViewModel как DataContext
            // _navigationService = navigationService; // Не храним, если не нужно
        }

        // Старый конструктор можно оставить для других целей, но NavigationService его не использует
        public LoginWindow(INavigationService navigationService)
        {
            InitializeComponent();
            var viewModel = App.ServiceProvider.GetRequiredService<LoginViewModel>();
            this.DataContext = viewModel;
            // _navigationService = navigationService; // Не храним, если не нужно
        }
    }
}