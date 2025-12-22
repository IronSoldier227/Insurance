using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows.Controls;

namespace PL
{
    public partial class AnnualRevenueReportPage : Page
    {
        public AnnualRevenueReportPage()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetRequiredService<AnnualRevenueReportViewModel>();
        }
    }
}