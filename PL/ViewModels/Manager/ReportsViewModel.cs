// PL.ViewModels/ReportsViewModel.cs
using Interfaces.DTO;
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

        private string _yearInput = "2025"; 
        public string YearInput
        {
            get => _yearInput;
            set { _yearInput = value; OnPropertyChanged(); }
        }
        private ReportDto? _report;
        public ReportDto? Report
        {
            get => _report;
            private set { _report = value; OnPropertyChanged(); }
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
            Report = null; // <-- Очищаем старый отчёт

            if (string.IsNullOrWhiteSpace(yearInput) || !int.TryParse(yearInput, out int year))
            {
                ErrorMessage = "Пожалуйста, введите корректный год.";
                return;
            }

            try
            {
                Report = await _reportService.GetTotalPayoutsForYearAsync(year);
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