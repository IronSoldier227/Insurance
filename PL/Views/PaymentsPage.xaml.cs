using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows.Controls;

namespace PL
{
    public partial class PaymentsPage : Page
    {
        public PaymentsPage()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetRequiredService<PaymentsViewModel>();
        }
    }
}