using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows.Controls;

namespace PL
{
    public partial class VehiclesPage : Page
    {
        public VehiclesPage()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetRequiredService<VehiclesViewModel>();
            this.Loaded += async (sender, e) =>
            {
                if (this.DataContext is VehiclesViewModel vm)
                {
                    await vm.LoadVehiclesAsync();
                }
            };
        }
    }
}