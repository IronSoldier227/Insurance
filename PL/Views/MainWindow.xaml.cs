using Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows;

namespace PL
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly IPageNavigationService _navigationService;

        public MainWindow()
        {
            InitializeComponent();

            _navigationService = App.ServiceProvider.GetRequiredService<IPageNavigationService>();
            _viewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();

            _navigationService.InitializeFrame(MainFrame); // <-- Передаём Frame в сервис

            this.DataContext = _viewModel;
        }
    }
}