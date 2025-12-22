using Interfaces.DTO;
using Interfaces.Services;
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
    public class RegisterClaimViewModel : INotifyPropertyChanged
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

        public ICommand SubmitClaimCommand { get; }
        public ICommand CancelCommand { get; }

        private ObservableCollection<Insurance> _policies = new ObservableCollection<Insurance>();
        public ObservableCollection<Insurance> Policies
        {
            get => _policies;
            set { _policies = value; OnPropertyChanged(); }
        }

        private Insurance? _selectedPolicy;
        public Insurance? SelectedPolicy
        {
            get => _selectedPolicy;
            set { _selectedPolicy = value; OnPropertyChanged(); }
        }

        private DateTime _claimDate = DateTime.Now;
        public DateTime ClaimDate
        {
            get => _claimDate;
            set { _claimDate = value; OnPropertyChanged(); }
        }

        private string _location = string.Empty;
        public string Location
        {
            get => _location;
            set { _location = value; OnPropertyChanged(); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private string _estimatedDamage = string.Empty;
        public string EstimatedDamage
        {
            get => _estimatedDamage;
            set { _estimatedDamage = value; OnPropertyChanged(); }
        }

        public RegisterClaimViewModel(
            IClaimService claimService,
            IPolicyService policyService,
            ICurrentUserService currentUserService,
            INavigationService navigationService)
        {
            _claimService = claimService;
            _policyService = policyService;
            _currentUserService = currentUserService;
            _navigationService = navigationService;

            SubmitClaimCommand = new RelayCommand(async _ => await SubmitClaimAsync(), _ => CanSubmitClaim());
            CancelCommand = new RelayCommand(_ => Cancel(), (Func<bool>?)null);

            _ = LoadPoliciesAsync();
        }

        private async Task LoadPoliciesAsync()
        {
            var user = _currentUserService.GetCurrentUser();
            if (user == null) return;

            try
            {
                var userPolicies = await _policyService.GetByClientIdAsync(user.Id);
                var activePolicies = userPolicies.Where(p => p.StatusId == 1).ToList();

                Policies.Clear();
                foreach (var policy in activePolicies)
                {
                    Policies.Add(policy);
                }

                if (Policies.Any())
                {
                    SelectedPolicy = Policies.First();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки полисов: {ex.Message}";
            }
        }

        private bool CanSubmitClaim()
        {
            return SelectedPolicy != null &&
                   !string.IsNullOrWhiteSpace(Location) &&
                   !string.IsNullOrWhiteSpace(Description) &&
                   double.TryParse(EstimatedDamage, out var damage) &&
                   damage > 0;
        }

        private async Task SubmitClaimAsync()
        {
            ErrorMessage = string.Empty;

            if (SelectedPolicy == null)
            {
                ErrorMessage = "Выберите полис.";
                return;
            }

            if (!double.TryParse(EstimatedDamage, out var damage) || damage <= 0)
            {
                ErrorMessage = "Введите корректную сумму ущерба.";
                return;
            }

            try
            {
                var claimDto = new Claim
                {
                    PolicyId = SelectedPolicy.Id,
                    StatusId = 1, 
                    ClaimDate = ClaimDate,
                    Description = Description,
                    Location = Location,
                    EstimatedDamage = damage
                };

                await _claimService.CreateClaimAsync(claimDto);

                MessageBox.Show("Заявка на страховой случай успешно отправлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка отправки заявки: {ex.Message}";
            }
        }

        private void Cancel()
        {
            CloseWindow(false);
        }

        private void CloseWindow(bool dialogResult)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = dialogResult; 
                    window.Close();
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}