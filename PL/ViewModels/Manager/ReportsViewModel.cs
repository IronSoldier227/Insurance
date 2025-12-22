using Interfaces.Services; 
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PL.ViewModels
{
    public class ReportsViewModel : INotifyPropertyChanged
    {
        private readonly IReportService _reportService; 
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationWindowService _NavigationWindowService;

        private string _totalAmount = "0.00 ₽";
        public string TotalAmount
        {
            get => _totalAmount;
            private set { _totalAmount = value; OnPropertyChanged(); }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            private set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand GenerateReportCommand { get; }

        private readonly ICommand _goBackCommand;

        public ReportsViewModel(
            IReportService reportService, 
            ICurrentUserService currentUserService,
            INavigationWindowService NavigationWindowService)
        {
            _reportService = reportService; 
            _currentUserService = currentUserService;
            _NavigationWindowService = NavigationWindowService;

            _goBackCommand = new RelayCommand(_ => _NavigationWindowService.GoBack(), () => _NavigationWindowService.CanGoBack);
            GenerateReportCommand = new RelayCommand(async param => await GenerateReportAsync(param as string), _ => true);
        }

        private async Task GenerateReportAsync(string yearInput)
        {
            ErrorMessage = string.Empty;
            TotalAmount = "0.00 ₽";

            if (string.IsNullOrWhiteSpace(yearInput) || !int.TryParse(yearInput, out int year))
            {
                ErrorMessage = "Пожалуйста, введите корректный год.";
                return;
            }

            try
            {
                var totalAmount = await _reportService.GetTotalPayoutsForYearAsync(year);
                TotalAmount = totalAmount.ToString("N2") + " ₽";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при формировании отчёта: {ex.Message}";
            }
        }

        public ICommand GoBack => _goBackCommand;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}