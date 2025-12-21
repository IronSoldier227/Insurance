// PL/ViewModels/CreatePolicyViewModel.cs
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
    public class CreatePolicyViewModel : INotifyPropertyChanged
    {
        private readonly IPolicyService _policyService;
        private readonly IVehicleService _vehicleService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICatalogService _catalogService;
        private readonly IClientProfileService _clientProfileService; // <-- Новый сервис
        private readonly IClaimService _claimService; // <-- Для расчёта КБМ
        private readonly int _vehicleId;

        private string _errorMessage = string.Empty;
        private VehicleDto? _vehicle;

        public CreatePolicyViewModel(
            IPolicyService policyService,
            IVehicleService vehicleService,
            ICurrentUserService currentUserService,
            ICatalogService catalogService,
            IClientProfileService clientProfileService, // <-- Добавлен
            IClaimService claimService, // <-- Добавлен
            int vehicleId)
        {
            _policyService = policyService;
            _vehicleService = vehicleService;
            _currentUserService = currentUserService;
            _catalogService = catalogService;
            _clientProfileService = clientProfileService;
            _claimService = claimService;
            _vehicleId = vehicleId;

            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync(), (Func<bool>?)null);
            CreatePolicyCommand = new RelayCommand(async _ => await CreatePolicyAsync(), _ => CanCreatePolicy());
            CancelCommand = new RelayCommand(_ => Cancel(), (Func<bool>?)null);

            // Загружаем данные сразу
            _ = LoadDataAsync();
        }

        private VehicleDto? _selectedVehicle;
        public VehicleDto? SelectedVehicle
        {
            get => _selectedVehicle;
            set { _selectedVehicle = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _policyTypes = new ObservableCollection<string>();
        public ObservableCollection<string> PolicyTypes
        {
            get => _policyTypes;
            set { _policyTypes = value; OnPropertyChanged(); }
        }

        private string? _selectedPolicyType;
        public string? SelectedPolicyType
        {
            get => _selectedPolicyType;
            set { _selectedPolicyType = value; OnPropertyChanged(); CalculatePriceAsync(); }
        }

        private int _policyDurationMonths = 12;
        public int PolicyDurationMonths
        {
            get => _policyDurationMonths;
            set { _policyDurationMonths = value; OnPropertyChanged(); CalculatePriceAsync(); }
        }

        private double _basePrice;
        public double BasePrice
        {
            get => _basePrice;
            set { _basePrice = value; OnPropertyChanged(); }
        }

        private double _powerCoefficient = 1.0;
        public double PowerCoefficient
        {
            get => _powerCoefficient;
            set { _powerCoefficient = value; OnPropertyChanged(); }
        }

        private double _experienceCoefficient = 1.0;
        public double ExperienceCoefficient
        {
            get => _experienceCoefficient;
            set { _experienceCoefficient = value; OnPropertyChanged(); }
        }

        private double _bonusMalusCoefficient = 1.0;
        public double BonusMalusCoefficient
        {
            get => _bonusMalusCoefficient;
            set { _bonusMalusCoefficient = value; OnPropertyChanged(); }
        }

        private double _totalPrice;
        public double TotalPrice
        {
            get => _totalPrice;
            set { _totalPrice = value; OnPropertyChanged(); }
        }

        private double _finalPriceForUser;
        public double FinalPriceForUser
        {
            get => _finalPriceForUser;
            set { _finalPriceForUser = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand CreatePolicyCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadDataAsync()
        {
            ErrorMessage = string.Empty;

            try
            {
                // Загружаем данные об автомобиле
                _vehicle = await _vehicleService.GetVehicleByIdAsync(_vehicleId);
                if (_vehicle == null)
                {
                    ErrorMessage = "Автомобиль не найден";
                    return;
                }

                SelectedVehicle = _vehicle;

                // Загружаем типы страховок (пока хардкод)
                PolicyTypes.Clear();
                PolicyTypes.Add("ОСАГО");
                PolicyTypes.Add("КАСКО");
                PolicyTypes.Add("ДСАГО");
                PolicyTypes.Add("ОСГОП");

                if (PolicyTypes.Any())
                    SelectedPolicyType = PolicyTypes.First();

                // Рассчитываем начальную цену
                await CalculatePriceAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки данных: {ex.Message}";
            }
        }

        private double GetBaseRate(string policyType)
        {
            return policyType switch
            {
                "ОСАГО" => 5000.0,
                "КАСКО" => 10000.0,
                "ОСГОП" => 3000.0,
                "ДСАГО" => 3000.0,
                _ => 5000.0
            };
        }

        private async Task CalculatePriceAsync()
        {
            try
            {
                if (_vehicle == null) return;

                // Базовые ставки по типам страховок
                double baseRate = GetBaseRate(SelectedPolicyType ?? "ОСАГО");
                BasePrice = baseRate;

                // Коэффициент мощности
                PowerCoefficient = CalculatePowerCoefficient(_vehicle.PowerHp);

                // Коэффициент стажа (реальный)
                var currentUser = _currentUserService.GetCurrentUser();
                if (currentUser != null)
                {
                    var clientProfile = await _clientProfileService.GetByUserIdAsync(currentUser.Id);
                    if (clientProfile != null)
                    {
                        ExperienceCoefficient = CalculateExperienceCoefficient(clientProfile.DrivingExperience);
                    }
                    else
                    {
                        ExperienceCoefficient = CalculateExperienceCoefficient(0);
                    }
                }
                else
                {
                    ExperienceCoefficient = CalculateExperienceCoefficient(0);
                }

                // Коэффициент бонус-малус (КБМ) по количеству выплат за последний год
                var claimCountLastYear = await GetClaimCountLastYear(_vehicle.ClientId);
                BonusMalusCoefficient = CalculateBonusMalusCoefficient(claimCountLastYear);

                // Итоговая цена
                double calculatedTotalPrice = BasePrice * PowerCoefficient * ExperienceCoefficient * BonusMalusCoefficient;

                // Учет длительности (скидка за полгода)
                double durationMultiplier = PolicyDurationMonths switch
                {
                    12 => 1.0,
                    6 => 0.7,
                    _ => PolicyDurationMonths / 12.0
                };

                // Цена для пользователя (с учётом срока)
                FinalPriceForUser = Math.Round(calculatedTotalPrice * durationMultiplier, 2);

                // Цена в БД (по формуле)
                TotalPrice = Math.Round(calculatedTotalPrice, 2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка расчета цены: {ex.Message}");
                TotalPrice = 0.0;
                FinalPriceForUser = 0.0;
            }
        }

        private double CalculatePowerCoefficient(int powerHp)
        {
            if (powerHp <= 50) return 0.6;
            if (powerHp <= 70) return 1.0;
            if (powerHp <= 100) return 1.1;
            if (powerHp <= 120) return 1.2;
            if (powerHp <= 150) return 1.4;
            return 1.6;
        }

        private double CalculateExperienceCoefficient(int experienceYears)
        {
            if (experienceYears == 0) return 2.27;
            if (experienceYears == 1) return 1.92;
            if (experienceYears == 2) return 1.84;
            if (experienceYears < 5) return 1.65;
            if (experienceYears < 7) return 1.1;
            if (experienceYears < 10) return 1.09;
            if (experienceYears < 15) return 0.97;
            return 0.95;
        }

        private double CalculateBonusMalusCoefficient(int claimCountLastYear)
        {
            // Если это первый полис (нет предыдущего), базовый КБМ = 1.17
            if (_vehicle == null || _vehicle.IsInsured == false)
            {
                return 1.17;
            }

            // Иначе, ищем предыдущий полис и его КБМ
            var previousPolicy = GetPreviousPolicy(_vehicle.Id);
            if (previousPolicy == null)
            {
                // Если предыдущий полис не найден, используем базовый КБМ
                return 1.17;
            }

            double currentKBM = previousPolicy.BonusMalusCoefficient;

            // Применяем таблицу КБМ
            return GetNewKBM(currentKBM, claimCountLastYear);
        }

        private Insurance GetPreviousPolicy(int vehicleId)
        {
            // Получаем все полисы для этого автомобиля
            var policies = _policyService.GetPoliciesByVehicleId(vehicleId).Result; // <-- Это синхронный вызов, но можно сделать асинхронным
                                                                                    // Сортируем по дате создания (последний полис)
            var lastPolicy = policies.OrderByDescending(p => p.StartDate).FirstOrDefault();
            return lastPolicy;
        }

        private double GetNewKBM(double currentKBM, int claimCount)
        {
            // Таблица КБМ (строки - текущий КБМ, столбцы - количество выплат)
            // В реальности, таблица может быть более сложной, но для простоты используем точные значения из таблицы

            // Создадим структуру таблицы
            var kbmTable = new Dictionary<double, Dictionary<int, double>>
    {
        { 3.92, new Dictionary<int, double> { {0, 2.94}, {1, 3.92}, {2, 3.92}, {3, 3.92}, {4, 3.92} } },
        { 2.94, new Dictionary<int, double> { {0, 2.25}, {1, 3.92}, {2, 3.92}, {3, 3.92}, {4, 3.92} } },
        { 2.25, new Dictionary<int, double> { {0, 1.76}, {1, 2.25}, {2, 2.25}, {3, 2.25}, {4, 2.25} } },
        { 1.76, new Dictionary<int, double> { {0, 1.17}, {1, 2.25}, {2, 2.25}, {3, 2.25}, {4, 2.25} } },
        { 1.17, new Dictionary<int, double> { {0, 1.0}, {1, 1.76}, {2, 2.25}, {3, 2.25}, {4, 2.25} } },
        { 1.0, new Dictionary<int, double> { {0, 0.91}, {1, 1.76}, {2, 2.25}, {3, 2.25}, {4, 2.25} } },
        { 0.91, new Dictionary<int, double> { {0, 0.83}, {1, 1.17}, {2, 2.25}, {3, 2.25}, {4, 2.25} } },
        { 0.83, new Dictionary<int, double> { {0, 0.78}, {1, 1.0}, {2, 1.76}, {3, 2.25}, {4, 2.25} } },
        { 0.78, new Dictionary<int, double> { {0, 0.74}, {1, 1.0}, {2, 1.76}, {3, 2.25}, {4, 2.25} } },
        { 0.74, new Dictionary<int, double> { {0, 0.68}, {1, 0.91}, {2, 1.76}, {3, 2.25}, {4, 2.25} } },
        { 0.68, new Dictionary<int, double> { {0, 0.63}, {1, 0.91}, {2, 1.76}, {3, 2.25}, {4, 2.25} } },
        { 0.63, new Dictionary<int, double> { {0, 0.57}, {1, 0.83}, {2, 1.17}, {3, 2.25}, {4, 2.25} } },
        { 0.57, new Dictionary<int, double> { {0, 0.52}, {1, 0.83}, {2, 1.17}, {3, 2.25}, {4, 2.25} } },
        { 0.52, new Dictionary<int, double> { {0, 0.46}, {1, 0.78}, {2, 1.17}, {3, 2.25}, {4, 2.25} } },
        { 0.46, new Dictionary<int, double> { {0, 0.46}, {1, 0.78}, {2, 1.17}, {3, 2.25}, {4, 2.25} } }
    };

            // Найдём ближайшее значение текущего КБМ в таблице
            var closestKBM = kbmTable.Keys.OrderBy(k => Math.Abs(k - currentKBM)).First();

            // Получим новое значение КБМ
            if (kbmTable[closestKBM].TryGetValue(claimCount, out double newKBM))
            {
                return newKBM;
            }

            // Если не нашли, вернём базовый КБМ
            return 1.17;
        }

        private async Task<int> GetClaimCountLastYear(int clientId)
        {
            var claims = await _claimService.GetByClientIdAsync(clientId);
            var lastYear = DateTime.Now.AddYears(-1);
            return claims.Count(c => c.ClaimDate >= lastYear && c.ClaimDate <= DateTime.Now);
        }

        private bool CanCreatePolicy()
        {
            return _vehicle != null &&
                   !string.IsNullOrEmpty(SelectedPolicyType) &&
                   PolicyDurationMonths > 0 &&
                   TotalPrice > 0;
        }

        private async Task CreatePolicyAsync()
        {
            ErrorMessage = string.Empty;

            if (_vehicle == null)
            {
                ErrorMessage = "Автомобиль не выбран";
                return;
            }

            try
            {
                var policyNumber = GeneratePolicyNumber();

                // Передаём TotalPrice (по формуле) в DTO
                var policyDto = new Insurance
                {
                    VehicleId = _vehicle.Id,
                    TypeId = GetTypeId(SelectedPolicyType!),
                    StatusId = 1, // Активный
                    PolicyNumber = policyNumber,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(PolicyDurationMonths),
                    BasePrice = BasePrice,
                    PowerCoefficient = PowerCoefficient,
                    ExperienceCoefficient = ExperienceCoefficient,
                    BonusMalusCoefficient = BonusMalusCoefficient
                };

                var policyId = await _policyService.CreatePolicyAsync(policyDto);

                MessageBox.Show(
                    $"Страховой полис успешно создан!\n" +
                    $"Номер полиса: {policyNumber}\n" +
                    $"Сумма к оплате: {FinalPriceForUser:C}\n" +
                    $"Срок действия: {policyDto.StartDate:dd.MM.yyyy} - {policyDto.EndDate:dd.MM.yyyy}",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка создания полиса: {ex.Message}";
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GeneratePolicyNumber()
        {
            return $"POL-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }

        private int GetTypeId(string policyType)
        {
            return policyType switch
            {
                "ОСАГО" => 1,
                "КАСКО" => 2,
                "ДСАГО" => 3,
                "ОСГОП" => 4,
                _ => 1
            };
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
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}