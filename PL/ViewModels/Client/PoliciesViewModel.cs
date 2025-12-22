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
        private readonly INavigationWindowService _NavigationWindowService;

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoadPoliciesCommand { get; }
        public ICommand GoBackCommand { get; } 

        private ObservableCollection<Insurance> _policies = new ObservableCollection<Insurance>();
        private string _selectedStatusFilter = "Все"; 
        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set { _selectedStatusFilter = value; OnPropertyChanged(); UpdateFilteredPolicies(); }
        }

        private string _selectedTypeFilter = "Все";
        public string SelectedTypeFilter
        {
            get => _selectedTypeFilter;
            set { _selectedTypeFilter = value; OnPropertyChanged(); UpdateFilteredPolicies(); }
        }

        public ObservableCollection<string> StatusFilters { get; } = new() { "Все", "Активные", "Завершённые", "Отменённые" };
        public ObservableCollection<string> TypeFilters { get; } = new() { "Все", "ОСАГО", "КАСКО", "ДСАГО", "ОСГОП" };
        // --- 

        private ObservableCollection<Insurance> _allPolicies = new ObservableCollection<Insurance>();
        private ObservableCollection<Insurance> _filteredPolicies = new ObservableCollection<Insurance>(); 
        public ObservableCollection<Insurance> Policies => _filteredPolicies; 

        public PoliciesViewModel(
            IPolicyService policyService,
            ICurrentUserService currentUserService,
            INavigationWindowService NavigationWindowService)
        {
            _policyService = policyService;
            _currentUserService = currentUserService;
            _NavigationWindowService = NavigationWindowService;

            LoadPoliciesCommand = new RelayCommand(async _ => await LoadPoliciesAsync(), (Func<bool>?)null);
            GoBackCommand = new RelayCommand(_ => _NavigationWindowService.GoBack(), () => _NavigationWindowService.CanGoBack);

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

                UpdateFilteredPolicies(); 
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки страховок: {ex.Message}";
            }
        }

        private void UpdateFilteredPolicies()
        {
            _filteredPolicies.Clear();

            var filtered = _allPolicies.AsEnumerable(); 

            // Фильтр по статусу
            switch (SelectedStatusFilter)
            {
                case "Активные":
                    filtered = filtered.Where(p => p.StatusId == 1);
                    break;
                case "Завершённые":
                    filtered = filtered.Where(p => p.StatusId == 2); 
                    break;
                case "Отменённые":
                    filtered = filtered.Where(p => p.StatusId == 3);
                    break;
            }

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
            }

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