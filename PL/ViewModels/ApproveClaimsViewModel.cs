using Interfaces.DTO;
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
    public class ApproveClaimsViewModel : INotifyPropertyChanged
    {
        private readonly IClaimService _claimService;
        private readonly IPaymentService _paymentService; 
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

        private ObservableCollection<ClaimWithCommands> _claims = new ObservableCollection<ClaimWithCommands>();
        public ObservableCollection<ClaimWithCommands> Claims
        {
            get => _claims;
            set { _claims = value; OnPropertyChanged(); }
        }

        public ApproveClaimsViewModel(
            IClaimService claimService,
            IPaymentService paymentService, 
            ICurrentUserService currentUserService,
            INavigationService navigationService)
        {
            _claimService = claimService;
            _paymentService = paymentService;
            _currentUserService = currentUserService;
            _navigationService = navigationService;

            LoadClaimsCommand = new RelayCommand(async _ => await LoadClaimsAsync(), (Func<bool>?)null);
            GoBackCommand = new RelayCommand(_ => _navigationService.GoBack(), () => _navigationService.CanGoBack);

            _ = LoadClaimsAsync(); 
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
                var allClaims = await _claimService.GetAllClaimsAsync(); 
                var pendingClaims = allClaims.Where(c => c.StatusId == 1).ToList(); 

                Claims.Clear();
                foreach (var claim in pendingClaims)
                {
                    Claims.Add(new ClaimWithCommands(claim, this)); 
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки страховых случаев: {ex.Message}";
            }
        }

        public async Task ProcessClaimAsync(int claimId, bool approve)
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
                await _claimService.DecideClaimAsync(claimId, user.Id, approve ? 1.0 : (double?)null); 

                if (approve)
                {
                    var claim = await _claimService.GetClaimByIdAsync(claimId);
                    if (claim != null)
                    {
                        var paymentDto = new PaymentForClaimDto 
                        {
                            ClaimId = claimId,
                            Amount = claim.EstimatedDamage, 
                            PaymentDate = DateTime.Now,
                            AuthorizedBy = user.Id 
                        };

                        await _paymentService.CreatePaymentAsync(paymentDto);
                    }
                }

                await LoadClaimsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка обработки страхового случая: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class ClaimWithCommands : INotifyPropertyChanged
    {
        private readonly Claim _originalClaim;
        private readonly ApproveClaimsViewModel _parentViewModel;

        public ClaimWithCommands(Claim originalClaim, ApproveClaimsViewModel parentViewModel)
        {
            _originalClaim = originalClaim;
            _parentViewModel = parentViewModel;
            ApproveCommand = new RelayCommand(_ => _parentViewModel.ProcessClaimAsync(_originalClaim.Id, true), _ => true);
            RejectCommand = new RelayCommand(_ => _parentViewModel.ProcessClaimAsync(_originalClaim.Id, false), _ => true);
        }

        public int Id => _originalClaim.Id;
        public string PolicyNumber => _originalClaim.PolicyNumber;
        public DateTime ClaimDate => _originalClaim.ClaimDate;
        public string StatusName => _originalClaim.StatusName;
        public string Description => _originalClaim.Description;
        public string Location => _originalClaim.Location;
        public double EstimatedDamage => _originalClaim.EstimatedDamage;

        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}