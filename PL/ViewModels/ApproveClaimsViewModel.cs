// PL/ViewModels/ApproveClaimsViewModel.cs
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
        private readonly IPaymentService _paymentService; // <-- Новый сервис для создания выплат
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
            IPaymentService paymentService, // <-- Внедряем IPaymentService
            ICurrentUserService currentUserService,
            INavigationService navigationService)
        {
            _claimService = claimService;
            _paymentService = paymentService; // <-- Сохраняем
            _currentUserService = currentUserService;
            _navigationService = navigationService;

            LoadClaimsCommand = new RelayCommand(async _ => await LoadClaimsAsync(), (Func<bool>?)null);
            GoBackCommand = new RelayCommand(_ => _navigationService.GoBack(), () => _navigationService.CanGoBack);

            _ = LoadClaimsAsync(); // Загружаем при создании
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
                // Получаем все случаи со статусом "Отправлен" (Id = 1)
                var allClaims = await _claimService.GetAllClaimsAsync(); // Нужен метод, возвращающий ВСЕ
                var pendingClaims = allClaims.Where(c => c.StatusId == 1).ToList(); // Предположим, 1 = Отправлен

                Claims.Clear();
                foreach (var claim in pendingClaims)
                {
                    Claims.Add(new ClaimWithCommands(claim, this)); // Передаём ViewModel для вызова команд
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки страховых случаев: {ex.Message}";
            }
        }

        // --- Команда для одобрения случая ---
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
                // Обновляем статус случая
                await _claimService.DecideClaimAsync(claimId, user.Id, approve ? 1.0 : (double?)null); // approve = 2, reject = 3

                // Если одобрено, создаём выплату
                if (approve)
                {
                    // Получаем сумму ущерба из случая
                    var claim = await _claimService.GetClaimByIdAsync(claimId);
                    if (claim != null)
                    {
                        // Создаём DTO для выплаты
                        var paymentDto = new PaymentForClaimDto // Убедитесь, что у вас есть такой DTO
                        {
                            ClaimId = claimId,
                            Amount = claim.EstimatedDamage, // Используем оценённый ущерб как сумму выплаты
                            PaymentDate = DateTime.Now,
                            AuthorizedBy = user.Id // Id менеджера
                        };

                        await _paymentService.CreatePaymentAsync(paymentDto);
                    }
                }

                // Перезагружаем список
                await LoadClaimsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка обработки страхового случая: {ex.Message}";
            }
        }
        // --- 

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // --- Класс-обёртка для Claim с командами ---
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
    // --- 
}