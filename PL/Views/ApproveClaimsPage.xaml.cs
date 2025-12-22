using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows.Controls;

namespace PL
{
    public partial class ApproveClaimsPage : Page
    {
        public ApproveClaimsPage()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetRequiredService<ApproveClaimsViewModel>();
        }
    }
}