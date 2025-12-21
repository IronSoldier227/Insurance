// PL/ViewModels/ReportsViewModel.cs
using Interfaces.Services; // Для ICurrentUserService, INavigationService, IReportService
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PL.ViewModels
{
    public class ReportsViewModel : INotifyPropertyChanged
    {
        private readonly IReportService _reportService; // <-- Теперь используем IReportService
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationService _navigationService;

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

        public ReportsViewModel(
            IReportService reportService, // <-- Внедряем IReportService
            ICurrentUserService currentUserService,
            INavigationService navigationService)
        {
            _reportService = reportService; // <-- Сохраняем
            _currentUserService = currentUserService;
            _navigationService = navigationService;

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
                // Вызываем сервис
                var totalAmount = await _reportService.GetTotalPayoutsForYearAsync(year);
                TotalAmount = totalAmount.ToString("N2") + " ₽";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при формировании отчёта: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}