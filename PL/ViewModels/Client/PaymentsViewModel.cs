using Core.Entities; 
using Interfaces.DTO;
using Interfaces.Repository; 
using Interfaces.Services; 
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PL.ViewModels
{
    public class PaymentsViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<PaymentForClaim> _paymentRepository;
        private readonly IClaimService _claimService;
        private readonly IPolicyService _policyService; 
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationWindowService _NavigationWindowService;

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoadPaymentsCommand { get; }
        public ICommand GoBackCommand { get; }

        private ObservableCollection<PaymentDto> _payments = new ObservableCollection<PaymentDto>();
        public ObservableCollection<PaymentDto> Payments
        {
            get => _payments;
            set { _payments = value; OnPropertyChanged(); }
        }

        public PaymentsViewModel(
            IRepository<PaymentForClaim> paymentRepository,
            IClaimService claimService,
            IPolicyService policyService,
            IUserService userService,
            ICurrentUserService currentUserService,
            INavigationWindowService NavigationWindowService)
        {
            _paymentRepository = paymentRepository;
            _claimService = claimService;
            _policyService = policyService;
            _userService = userService;
            _currentUserService = currentUserService;
            _NavigationWindowService = NavigationWindowService;

            LoadPaymentsCommand = new RelayCommand(async _ => await LoadPaymentsAsync(), (Func<bool>?)null);
            GoBackCommand = new RelayCommand(_ => _NavigationWindowService.GoBack(), () => _NavigationWindowService.CanGoBack);

            _ = LoadPaymentsAsync();
        }

        private async Task LoadPaymentsAsync()
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
                var userClaims = await _claimService.GetByClientIdAsync(user.Id);
                var claimIds = userClaims.ToDictionary(c => c.Id);

                var allPayments = await _paymentRepository.GetAllAsync();
                var userPayments = allPayments.Where(p => claimIds.ContainsKey(p.ClaimId)).ToList();

                var userPolicies = (await _policyService.GetByClientIdAsync(user.Id)).ToDictionary(p => p.Id); 

                Payments.Clear();
                foreach (var payment in userPayments)
                {
                    var claim = claimIds.TryGetValue(payment.ClaimId, out var foundClaim) ? foundClaim : null;
                    string policyNumber = "N/A";
                    DateTime claimDate = DateTime.MinValue;

                    if (claim != null)
                    {
                        claimDate = claim.ClaimDate; 
                        var policy = userPolicies.TryGetValue(claim.PolicyId, out var foundPolicy) ? foundPolicy : null;
                        policyNumber = policy?.PolicyNumber ?? "N/A";
                    }
                    string managerName = "N/A";
                        var managerUser = await _userService.GetByIdAsync(payment.AuthorizedBy);
                        if (managerUser != null)
                        {
                            managerName = $"{managerUser.FirstName} {managerUser.LastName} {managerUser.MiddleName}".Trim();
                        }

                    Payments.Add(new PaymentDto
                    {
                        Id = payment.Id,
                        ClaimId = payment.ClaimId,
                        Amount = payment.Amount,
                        PaymentDate = payment.PaymentDate,
                        AuthorizedByManagerName = managerName,
                        ClaimDate = claimDate,
                        PolicyNumber = policyNumber
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки выплат: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}