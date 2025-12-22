using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows.Controls;

namespace PL
{
    public partial class ClaimsPage : Page
    {
        public ClaimsPage()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetRequiredService<ClaimsViewModel>();
        }
    }
}