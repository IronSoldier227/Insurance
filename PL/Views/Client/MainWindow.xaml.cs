using Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows;

namespace PL
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly IPageNavigationWindowService _NavigationWindowService;

        public MainWindow()
        {
            InitializeComponent();

            _NavigationWindowService = App.ServiceProvider.GetRequiredService<IPageNavigationWindowService>();
            _viewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();

            _NavigationWindowService.InitializeFrame(MainFrame); // <-- Передаём Frame в сервис

            this.DataContext = _viewModel;
        }
    }
}