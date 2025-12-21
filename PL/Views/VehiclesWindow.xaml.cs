// PL/Views/VehiclesWindow.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows;

namespace PL
{
    public partial class VehiclesWindow : Window
    {
        public VehiclesWindow()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetRequiredService<VehiclesViewModel>();

            Loaded += async (s, e) =>
            {
                if (DataContext is VehiclesViewModel vm)
                {
                    await vm.LoadVehiclesAsync();
                }
            };
        }

        protected override void OnClosed(System.EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("VehiclesWindow.OnClosed вызван.");
            base.OnClosed(e);
        }
    }
}