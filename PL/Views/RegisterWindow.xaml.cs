// PL/Views/RegisterWindow.xaml.cs
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
            // Получаем ViewModel из DI-контейнера
            var viewModel = App.ServiceProvider.GetRequiredService<RegisterViewModel>();
            this.DataContext = viewModel;
        }
    }
}