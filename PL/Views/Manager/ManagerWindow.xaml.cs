using Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows;

namespace PL
{
    public partial class ManagerWindow : Window
    {
        private readonly ManagerViewModel _viewModel;
        private readonly IPageNavigationWindowService _NavigationWindowService;

        public ManagerWindow()
        {
            InitializeComponent();

            _NavigationWindowService = App.ServiceProvider.GetRequiredService<IPageNavigationWindowService>();
            _viewModel = App.ServiceProvider.GetRequiredService<ManagerViewModel>();

            _NavigationWindowService.InitializeFrame(MainFrame); // <-- Передаём Frame в сервис

            this.DataContext = _viewModel;
        }
    }
}