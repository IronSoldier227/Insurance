using Interfaces.DTO;
using Interfaces.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PL.ViewModels
{
    public class AnnualRevenueReportViewModel : INotifyPropertyChanged
    {
        private readonly IReportService _reportService;
        private readonly INavigationService _navigationService;

        private string _yearInput = "2025"; 
        public string YearInput
        {
            get => _yearInput;
            set { _yearInput = value; OnPropertyChanged(); }
        }

        private AnnualPolicyRevenueReportDto? _report;
        public AnnualPolicyRevenueReportDto? Report
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

        public ICommand LoadReportCommand { get; }
        public ICommand GoBackCommand { get; }

        public AnnualRevenueReportViewModel(IReportService reportService, INavigationService navigationService)
        {
            _reportService = reportService;
            _navigationService = navigationService;

            LoadReportCommand = new RelayCommand(async _ => await LoadReportAsync(), _ => CanLoadReport());
            GoBackCommand = new RelayCommand(_ => _navigationService.GoBack(), () => _navigationService.CanGoBack);
        }

        private bool CanLoadReport()
        {
            return int.TryParse(YearInput, out var year) && year > 0;
        }

        private async Task LoadReportAsync()
        {
            ErrorMessage = string.Empty;
            Report = null;

            if (!int.TryParse(YearInput, out var year))
            {
                ErrorMessage = "Введите корректный год.";
                return;
            }

            try
            {
                Report = await _reportService.GetAnnualRevenueReportAsync(year);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки отчёта: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}