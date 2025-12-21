// PL/ViewModels/PoliciesViewModel.cs
using Interfaces.DTO;
using Interfaces.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using PL.ViewModels;

namespace PL.ViewModels
{
    public class PoliciesViewModel : INotifyPropertyChanged
    {
        private readonly IPolicyService _policyService;
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationService _navigationService;

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoadPoliciesCommand { get; }
        public ICommand GoBackCommand { get; } // Команда "Назад"

        private ObservableCollection<Insurance> _policies = new ObservableCollection<Insurance>();
        private string _selectedStatusFilter = "Все"; // По умолчанию "Все"
        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set { _selectedStatusFilter = value; OnPropertyChanged(); UpdateFilteredPolicies(); }
        }

        private string _selectedTypeFilter = "Все"; // По умолчанию "Все"
        public string SelectedTypeFilter
        {
            get => _selectedTypeFilter;
            set { _selectedTypeFilter = value; OnPropertyChanged(); UpdateFilteredPolicies(); }
        }

        public ObservableCollection<string> StatusFilters { get; } = new() { "Все", "Активные", "Завершённые", "Отменённые" };
        public ObservableCollection<string> TypeFilters { get; } = new() { "Все", "ОСАГО", "КАСКО", "ДСАГО", "ОСГОП" };
        // --- 

        private ObservableCollection<Insurance> _allPolicies = new ObservableCollection<Insurance>(); // Храним все загруженные полисы
        private ObservableCollection<Insurance> _filteredPolicies = new ObservableCollection<Insurance>(); // Отфильтрованные и отсортированные
        public ObservableCollection<Insurance> Policies => _filteredPolicies; // Привязываем DataGrid к этому списку

        public PoliciesViewModel(
            IPolicyService policyService,
            ICurrentUserService currentUserService,
            INavigationService navigationService)
        {
            _policyService = policyService;
            _currentUserService = currentUserService;
            _navigationService = navigationService;

            LoadPoliciesCommand = new RelayCommand(async _ => await LoadPoliciesAsync(), (Func<bool>?)null);
            // Используем Func<bool> для CanExecute, чтобы он вызывал _navigationService.CanGoBack
            GoBackCommand = new RelayCommand(_ => _navigationService.GoBack(), () => _navigationService.CanGoBack);

            _ = LoadPoliciesAsync();
        }

        private async Task LoadPoliciesAsync()
        {
            ErrorMessage = string.Empty;
            var user = _currentUserService.GetCurrentUser();
            if (user == null)
            {
                ErrorMessage = "Пользователь не авторизован";
                return;
            }

            try
            {
                var allPolicies = await _policyService.GetByClientIdAsync(user.Id);

                _allPolicies.Clear();
                foreach (var policy in allPolicies)
                {
                    _allPolicies.Add(policy);
                }

                UpdateFilteredPolicies(); // Применяем фильтрацию и сортировку
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки страховок: {ex.Message}";
            }
        }

        // --- Метод для обновления отфильтрованного списка ---
        private void UpdateFilteredPolicies()
        {
            _filteredPolicies.Clear();

            var filtered = _allPolicies.AsEnumerable(); // Начинаем с полного списка

            // Фильтр по статусу
            switch (SelectedStatusFilter)
            {
                case "Активные":
                    filtered = filtered.Where(p => p.StatusId == 1); // Предположим, 1 = Активен
                    break;
                case "Завершённые":
                    filtered = filtered.Where(p => p.StatusId == 2); // Предположим, 2 = Завершён
                    break;
                case "Отменённые":
                    filtered = filtered.Where(p => p.StatusId == 3); // Предположим, 3 = Отменён
                    break;
                    // "Все" означает, что фильтр не применяется
            }

            // Фильтр по типу
            switch (SelectedTypeFilter)
            {
                case "ОСАГО":
                    filtered = filtered.Where(p => p.TypeId == 1);
                    break;
                case "КАСКО":
                    filtered = filtered.Where(p => p.TypeId == 2);
                    break;
                case "ДСАГО":
                    filtered = filtered.Where(p => p.TypeId == 3);
                    break;
                case "ОСГОП":
                    filtered = filtered.Where(p => p.TypeId == 4);
                    break;
                    // "Все" означает, что фильтр не применяется
            }

            // Сортировка (например, по дате начала)
            var sorted = filtered.OrderBy(p => p.StartDate);

            foreach (var policy in sorted)
            {
                _filteredPolicies.Add(policy);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}