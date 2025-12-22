using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows;

namespace PL
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            var viewModel = App.ServiceProvider.GetRequiredService<RegisterViewModel>();
            this.DataContext = viewModel;
        }
    }
}