using Interfaces.DTO;
using Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PL.ViewModels
{
    public class ClaimsViewModel : INotifyPropertyChanged
    {
        private readonly IClaimService _claimService;
        private readonly IPolicyService _policyService; 
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationService _navigationService;

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoadClaimsCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand RegisterNewClaimCommand { get; } 

        private string _selectedClaimStatusFilter = "Все";
        public string SelectedClaimStatusFilter
        {
            get => _selectedClaimStatusFilter;
            set { _selectedClaimStatusFilter = value; OnPropertyChanged(); UpdateFilteredClaims(); }
        }

        private int _selectedPolicyFilter = -1; // -1 означает "Все"
        public int SelectedPolicyFilter
        {
            get => _selectedPolicyFilter;
            set { _selectedPolicyFilter = value; OnPropertyChanged(); UpdateFilteredClaims(); }
        }

        public ObservableCollection<string> ClaimStatusFilters { get; } = new() { "Все", "Отправлен", "Одобрено", "Отклонено" };
        private ObservableCollection<Insurance> _allPoliciesForFilter = new ObservableCollection<Insurance>();
        public ObservableCollection<Insurance> AllPoliciesForFilter => _allPoliciesForFilter;

        private ObservableCollection<Claim> _allClaims = new ObservableCollection<Claim>();
        private ObservableCollection<Claim> _filteredClaims = new ObservableCollection<Claim>();
        public ObservableCollection<Claim> Claims => _filteredClaims;

        public ClaimsViewModel(
            IClaimService claimService,
            IPolicyService policyService, 
            ICurrentUserService currentUserService,
            INavigationService navigationService)
        {
            _claimService = claimService;
            _policyService = policyService; 
            _currentUserService = currentUserService;
            _navigationService = navigationService;

            LoadClaimsCommand = new RelayCommand(async _ => await LoadClaimsAsync(), (Func<bool>?)null);
            GoBackCommand = new RelayCommand(_ => _navigationService.GoBack(), () => _navigationService.CanGoBack);
            RegisterNewClaimCommand = new RelayCommand(_ => ShowRegisterClaimWindow(), (Func<bool>?)null); 
            _ = LoadClaimsAsync();
        }

        private void ShowRegisterClaimWindow()
        {
            var registerClaimWindow = new RegisterClaimWindow(); 
            var viewModel = App.ServiceProvider.GetRequiredService<RegisterClaimViewModel>(); 
            registerClaimWindow.DataContext = viewModel;

            bool? result = registerClaimWindow.ShowDialog();

            if (result == true)
            {
                _ = LoadClaimsAsync(); 
            }
        }

        private async Task LoadClaimsAsync()
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
                var userPolicies = await _policyService.GetByClientIdAsync(user.Id);
                _allPoliciesForFilter.Clear();
                _allPoliciesForFilter.Add(new Insurance { Id = -1, PolicyNumber = "Все полисы" }); 
                foreach (var policy in userPolicies)
                {
                    _allPoliciesForFilter.Add(policy);
                }
                SelectedPolicyFilter = -1;

                var allClaims = await _claimService.GetByClientIdAsync(user.Id);

                _allClaims.Clear();
                foreach (var claim in allClaims)
                {
                    var policy = userPolicies.FirstOrDefault(p => p.Id == claim.PolicyId);
                    _allClaims.Add(new Claim
                    {
                        Id = claim.Id,
                        PolicyId = claim.PolicyId,
                        PolicyNumber = policy?.PolicyNumber ?? "N/A",
                        StatusId = claim.StatusId,
                        StatusName = GetStatusName(claim.StatusId),
                        ClaimDate = claim.ClaimDate,
                        Description = claim.Description,
                        Location = claim.Location,
                        EstimatedDamage = claim.EstimatedDamage
                    });
                }

                UpdateFilteredClaims();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки страховых случаев: {ex.Message}";
            }
        }

        private void UpdateFilteredClaims()
        {
            _filteredClaims.Clear();

            var filtered = _allClaims.AsEnumerable();

            switch (SelectedClaimStatusFilter)
            {
                case "Отправлен":
                    filtered = filtered.Where(c => c.StatusId == 1);
                    break;
                case "Одобрено":
                    filtered = filtered.Where(c => c.StatusId == 2);
                    break;
                case "Отклонено":
                    filtered = filtered.Where(c => c.StatusId == 3);
                    break; 
            }

            if (SelectedPolicyFilter != -1)
            {
                filtered = filtered.Where(c => c.PolicyId == SelectedPolicyFilter);
            }

            var sorted = filtered.OrderBy(c => c.ClaimDate);

            foreach (var claim in sorted)
            {
                _filteredClaims.Add(claim);
            }
        }

        private string GetStatusName(int statusId)
        {
            return statusId switch
            {
                1 => "Отправлен",
                2 => "Одобрено",
                3 => "Отклонено",
                _ => "Неизвестно"
            };
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}