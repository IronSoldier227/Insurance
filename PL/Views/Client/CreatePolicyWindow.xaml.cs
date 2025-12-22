// PL/Views/CreatePolicyWindow.xaml.cs
using PL.ViewModels;
using System.Windows;

namespace PL
{
    public partial class CreatePolicyWindow : Window
    {
        public CreatePolicyWindow()
        {
            InitializeComponent();
        }

        public CreatePolicyWindow(CreatePolicyViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}