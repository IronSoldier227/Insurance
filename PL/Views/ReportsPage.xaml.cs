using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows.Controls;

namespace PL
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetRequiredService<ReportsViewModel>();
        }
    }
}