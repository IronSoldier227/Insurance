using Microsoft.Extensions.DependencyInjection;
using PL.ViewModels;
using System.Windows.Controls;

namespace PL
{
    public partial class PoliciesPage : Page
    {
        public PoliciesPage()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider.GetRequiredService<PoliciesViewModel>();
        }
    }
}