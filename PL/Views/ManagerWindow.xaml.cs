using Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows;

namespace PL
{
    public partial class ManagerWindow : Window
    {
        private readonly ManagerViewModel _viewModel;
        private readonly IPageNavigationService _navigationService;

        public ManagerWindow()
        {
            InitializeComponent();

            _navigationService = App.ServiceProvider.GetRequiredService<IPageNavigationService>();
            _viewModel = App.ServiceProvider.GetRequiredService<ManagerViewModel>();

            _navigationService.InitializeFrame(MainFrame); // <-- Передаём Frame в сервис

            this.DataContext = _viewModel;
        }
    }
}