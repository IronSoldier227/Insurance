// PL/Views/PoliciesWindow.xaml.cs
using System.Windows;

namespace PL
{
    public partial class PoliciesWindow : Window
    {
        public PoliciesWindow()
        {
            // Не пытаемся получать сервисы здесь.
            // DataContext будет установлен извне.
            InitializeComponent();
            // Дополнительная логика, если нужна, после InitializeComponent
        }
    }
}