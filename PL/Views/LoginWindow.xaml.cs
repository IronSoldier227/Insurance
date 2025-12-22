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
            var NavigationWindowService = App.ServiceProvider.GetRequiredService<INavigationWindowService>();
            var viewModel = App.ServiceProvider.GetRequiredService<LoginViewModel>();
            this.DataContext = viewModel; 
        }

        public LoginWindow(INavigationWindowService NavigationWindowService)
        {
            InitializeComponent();
            var viewModel = App.ServiceProvider.GetRequiredService<LoginViewModel>();
            this.DataContext = viewModel;
        }
    }
}