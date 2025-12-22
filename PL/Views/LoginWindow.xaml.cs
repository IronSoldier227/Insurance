using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows;
using System.Windows.Controls; 

namespace PL
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            var navigationService = App.ServiceProvider.GetRequiredService<INavigationService>();
            var viewModel = App.ServiceProvider.GetRequiredService<LoginViewModel>();
            this.DataContext = viewModel; 
        }

        public LoginWindow(INavigationService navigationService)
        {
            InitializeComponent();
            var viewModel = App.ServiceProvider.GetRequiredService<LoginViewModel>();
            this.DataContext = viewModel;
        }
    }
}