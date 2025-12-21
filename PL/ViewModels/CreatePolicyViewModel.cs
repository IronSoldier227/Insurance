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
using System.Windows.Threading;

namespace PL.ViewModels
{
    public class CreatePolicyViewModel : INotifyPropertyChanged
    {
        private readonly IPolicyService _policyService;
        private readonly IVehicleService _vehicleService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICatalogService _catalogService;
        private readonly IClientProfileService _clientProfileService;
        private readonly IClaimService _claimService;
        private readonly int _vehicleId; // <-- Id автомобиля, передаётся из VehiclesViewModel
        private VehicleDto? _originalVehicleData; // <-- Сохраняем оригинальные данные автомобиля

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand CreatePolicyCommand { get; }
        public ICommand CancelCommand { get; }
        // --- НОВАЯ КОМАНДА ---
        public ICommand GoBackCommand { get; } // <-- Добавим команду "Назад", если нужно
        // ---

        // --- НОВЫЕ СВОЙСТВА ---
        private VehicleDto? _vehicle; // <-- Это VehicleDto, а не Vehicle из Core.Entities
        public VehicleDto? Vehicle // <-- Это свойство для XAML
        {
            get => _vehicle;
            set { _vehicle = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _availablePolicyTypes = new ObservableCollection<string>();
        public ObservableCollection<string> AvailablePolicyTypes // <-- Это свойство для XAML
        {
            get => _availablePolicyTypes;
            set { _availablePolicyTypes = value; OnPropertyChanged(); }
        }

        private string? _selectedPolicyType;
        public string? SelectedPolicyType // <-- Это свойство для XAML
        {
            get => _selectedPolicyType;
            set { _selectedPolicyType = value; OnPropertyChanged(); }
        }
        // ---

        private ObservableCollection<string> _allPolicyTypes = new ObservableCollection<string>();
        public ObservableCollection<string> AllPolicyTypes
        {
            get => _allPolicyTypes;
            set { _allPolicyTypes = value; OnPropertyChanged(); }
        }

        private string? _selectedCategory;
        public string? SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }

        private string _color = string.Empty;
        public string Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); }
        }

        private string _yearOfProduction = string.Empty;
        public string YearOfProduction
        {
            get => _yearOfProduction;
            set { _yearOfProduction = value; OnPropertyChanged(); }
        }

        private string _vin = string.Empty;
        public string Vin
        {
            get => _vin;
            set { _vin = value; OnPropertyChanged(); }
        }

        private string _plateNum = string.Empty;
        public string PlateNum
        {
            get => _plateNum;
            set { _plateNum = value; OnPropertyChanged(); }
        }

        private string _powerHp = string.Empty;
        public string PowerHp
        {
            get => _powerHp;
            set { _powerHp = value; OnPropertyChanged(); }
        }

        private int _policyDurationMonths = 12;
        public int PolicyDurationMonths
        {
            get => _policyDurationMonths;
            set { _policyDurationMonths = value; OnPropertyChanged(); }
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

        public CreatePolicyViewModel(
            IPolicyService policyService,
            IVehicleService vehicleService,
            ICurrentUserService currentUserService,
            ICatalogService catalogService,
            IClientProfileService clientProfileService,
            IClaimService claimService,
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
            // --- Установим команду GoBack ---
            // Предположим, что INavigationService доступен через DI или передаётся в конструктор
            // Но если нет, можно просто закрыть окно
            GoBackCommand = new RelayCommand(_ => CloseWindow(false), (Func<bool>?)null); // <-- Просто закрывает окно
            // Или, если у вас есть INavigationService:
            // var navService = App.ServiceProvider.GetRequiredService<INavigationService>();
            // GoBackCommand = new RelayCommand(_ => navService.GoBack(), () => navService.CanGoBack);
            // ---

            _ = LoadDataAsync(); // Загружаем при создании
        }

        private async Task LoadDataAsync()
        {
            ErrorMessage = string.Empty;
            try
            {
                // Загружаем данные об автомобиле
                _originalVehicleData = await _vehicleService.GetVehicleByIdAsync(_vehicleId);
                if (_originalVehicleData == null)
                {
                    ErrorMessage = "Автомобиль не найден";
                    return;
                }

                Vehicle = _originalVehicleData; // <-- Устанавливаем VehicleDto

                // Загружаем *все* возможные типы полисов из справочника
                var allTypesList = await _catalogService.GetPolicyTypesAsync(); // <-- Предполагаем, что возвращает IEnumerable<string>
                AllPolicyTypes.Clear();
                foreach (var type in allTypesList)
                {
                    AllPolicyTypes.Add(type);
                }

                // Загружаем *активные* полисы для этого автомобиля
                var activePolicies = await _policyService.GetActivePoliciesByVehicleIdAsync(_vehicleId); // <-- Нужен метод в IPolicyService
                var activePolicyTypeNames = activePolicies.Select(p => p.TypeName).ToHashSet(); // <-- TypeName из Insurance DTO

                // Определяем доступные типы (все минус активные)
                AvailablePolicyTypes.Clear();
                foreach (var type in AllPolicyTypes)
                {
                    if (!activePolicyTypeNames.Contains(type)) // <-- Если тип НЕ в списке активных
                    {
                        AvailablePolicyTypes.Add(type);
                    }
                }

                // Выбираем первый доступный тип, если есть
                if (AvailablePolicyTypes.Any())
                {
                    SelectedPolicyType = AvailablePolicyTypes.First();
                }
                else
                {
                    ErrorMessage = "Для этого автомобиля уже оформлены все доступные типы полисов.";
                    // Кнопка "Создать" будет неактивна из-за CanCreatePolicy
                }

                // Рассчитываем начальную цену (если тип выбран)
                if (SelectedPolicyType != null)
                {
                    await CalculatePriceAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки данных: {ex.Message}";
            }
        }

        // ...
        private async Task CalculatePriceAsync()
        {
            try
            {
                if (Vehicle == null || string.IsNullOrEmpty(SelectedPolicyType)) return;

                double baseRate = GetBaseRate(SelectedPolicyType);
                BasePrice = baseRate;

                PowerCoefficient = CalculatePowerCoefficient(Vehicle.PowerHp);

                var currentUser = _currentUserService.GetCurrentUser();
                if (currentUser != null)
                {
                    var clientProfile = await _clientProfileService.GetByUserIdAsync(currentUser.Id);
                    int experienceYears = clientProfile?.DrivingExperience ?? 0;
                    ExperienceCoefficient = CalculateExperienceCoefficient(experienceYears);
                }
                else
                {
                    ExperienceCoefficient = CalculateExperienceCoefficient(0);
                }

                // --- Получаем предыдущий КБМ или устанавливаем базовый (1.17) для нового клиента ---
                double previousKBM = 1.17; // <-- БАЗОВЫЙ КБМ = 1.17, как вы сказали
                bool firstAssuring = true;
                if (Vehicle != null)
                {
                    var allPolicies = await _policyService.GetClientPoliciesAsync(Vehicle.ClientId);
                    if (allPolicies.Any())
                    {
                        // Если у клиента есть *предыдущие* полисы, ищем последний *активный* или *последний по дате окончания*
                        // Для упрощения, возьмём последний по дате начала (или окончания)
                        // Предположим, что BonusMalusCoefficient - это КБМ, который был применён к *этому* полису при его создании
                        // и он не меняется в течение срока действия полиса.
                        // Тогда для НОВОГО полиса, КБМ зависит от СТРАХОВЫХ СЛУЧАЕВ за ПРЕДЫДУЩИЙ ГОД.
                        // Но для расчёта КБМ НОВОГО полиса, мы берём КБМ ПОСЛЕДНЕГО *ЗАВЕРШЁННОГО* полиса или БАЗОВЫЙ (1.17), если нет завершённых.
                        // Это сложная логика.
                        // Для простоты: если у клиента есть *любые* полисы, берём КБМ ПОСЛЕДНЕГО из НИХ (предполагая, что он был рассчитан корректно).
                        // Но если это *первый* полис, то КБМ = 1.17.
                        // Тогда логика: если полисов НЕТ -> 1.17. Если ЕСТЬ -> берём КБМ последнего.

                        // ВАЖНО: Предыдущий КБМ - это КБМ, который был *до* наступления страховых случаев в *предыдущем* году.
                        // Для *нового* полиса, мы используем КБМ *после* учёта случаев *предыдущего* года.
                        // Т.е. если у клиента был полис в 2024 году, и за этот год не было случаев, то его КБМ для 2025 года будет ниже.
                        // Мы не можем рассчитать КБМ "после" прошлого года здесь, потому что не знаем, *сколько* случаев было в прошлом году для ПРЕДЫДУЩЕГО полиса.
                        // Мы знаем только *текущие* активные полисы и *все* прошлые.
                        // Для *нового* полиса (первого) -> КБМ = 1.17.
                        // Для *очередного* полиса -> КБМ = КБМ_предыдущего_полиса_после_учёта_случаев_за_его_период.

                        // ПОКА ЧТО: Просто возьмём КБМ последнего полиса как "предыдущий КБМ" для расчёта нового.
                        // Это не совсем правильно, но ближе к реальности.
                        var lastPolicy = allPolicies.OrderByDescending(p => p.EndDate).FirstOrDefault(); // Сортируем по дате окончания
                        if (lastPolicy != null)
                        {
                            firstAssuring = false;
                            previousKBM = lastPolicy.BonusMalusCoefficient; // <-- Используем BM последнего завершённого (или активного) полиса
                            System.Diagnostics.Debug.WriteLine($"CalculatePriceAsync: Найден предыдущий КБМ: {previousKBM} из полиса ID: {lastPolicy.Id}");
                        }
                        else
                        {
                            // Если полисы есть, но ни одного завершённого/последнего не найдено (все активны?), всё равно базовый?
                            // Нет, если есть активный полис, значит, это не "новый клиент".
                            // Но если все активны, и ни одного завершённого, то КБМ берётся из *активного* полиса.
                            // Пусть будет так: если ни одного полиса не найдено по сортировке, используем базовый.
                            // Но если есть активные, и мы не можем определить "последний завершённый", возьмём последний активный.
                            // Упрощаем: берём любой последний.
                            var anyLastPolicy = allPolicies.OrderByDescending(p => p.StartDate).FirstOrDefault(); // Попробуем по дате начала
                            if (anyLastPolicy != null)
                            {
                                firstAssuring = false;
                                previousKBM = anyLastPolicy.BonusMalusCoefficient;
                                System.Diagnostics.Debug.WriteLine($"CalculatePriceAsync: Найден КБМ из активного полиса: {previousKBM} из полиса ID: {anyLastPolicy.Id}");
                            }
                            // Если всё равно не нашли, оставляем 1.17 (хотя это маловероятно, если allPolicies.Any())
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"CalculatePriceAsync: Полисов для клиента {Vehicle.ClientId} не найдено. Используем базовый КБМ 1.17.");
                    }
                }
                // ---

                    var claimCountLastYearForNewPolicy = await GetClaimCountLastYear(Vehicle.ClientId);

                    BonusMalusCoefficient = CalculateBonusMalusCoefficient(claimCountLastYearForNewPolicy, previousKBM, firstAssuring);
                

                double calculatedTotalPrice = BasePrice * PowerCoefficient * ExperienceCoefficient * BonusMalusCoefficient;

                double durationMultiplier = PolicyDurationMonths switch
                {
                    12 => 1.0,
                    6 => 0.7,
                    _ => PolicyDurationMonths / 12.0
                };

                FinalPriceForUser = Math.Round(calculatedTotalPrice * durationMultiplier, 2);
                TotalPrice = Math.Round(calculatedTotalPrice, 2);

                System.Diagnostics.Debug.WriteLine($"CalculatePriceAsync: Base={BasePrice}, PowerCoeff={PowerCoefficient}, ExpCoeff={ExperienceCoefficient}, BM-Coeff={BonusMalusCoefficient}, DurationMult={durationMultiplier}, Final={FinalPriceForUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка расчета цены: {ex.Message}");
                TotalPrice = 0.0;
                FinalPriceForUser = 0.0;
            }
        }

        // --- Изменяем CalculateBonusMalusCoefficient ---
        private double CalculateBonusMalusCoefficient(int claimCountLastYear, double currentKBM, bool firstAssuring)
        {
            System.Diagnostics.Debug.WriteLine($"CalculateBonusMalusCoefficient вызван. ClaimCount={claimCountLastYear}, CurrentKBM={currentKBM}");

            if (currentKBM == 1.17 && firstAssuring)
            {
                System.Diagnostics.Debug.WriteLine($"CalculateBonusMalusCoefficient: Это первый полис (предположительно). Возвращаем базовый 1.17.");
                return 1.17; // <-- Используем 1.17 для первого полиса
            }
            // Используем таблицу КБМ
            var kbmTable = new Dictionary<double, Dictionary<int, double>>
    {
        // ... (ваша таблица КБМ)
        // Упрощённый пример (реальность сложнее):
        // Строки - предыдущий КБМ, Столбцы - кол-во страховых случаев за предыдущий год
        { 3.92, new Dictionary<int, double> { {0, 2.94}, {1, 3.92}, {2, 3.92}, {3, 3.92}, {4, 3.92} } },
        { 2.94, new Dictionary<int, double> { {0, 2.25}, {1, 3.92}, {2, 3.92}, {3, 3.92}, {4, 3.92} } },
        { 2.25, new Dictionary<int, double> { {0, 1.76}, {1, 2.25}, {2, 2.25}, {3, 2.25}, {4, 2.25} } },
        { 1.76, new Dictionary<int, double> { {0, 1.17}, {1, 2.25}, {2, 2.25}, {3, 2.25}, {4, 2.25} } },
        { 1.17, new Dictionary<int, double> { {0, 1.0}, {1, 1.76}, {2, 2.25}, {3, 2.25}, {4, 2.25} } }, // <-- Вот тут: если предыдущий КБМ был 1.17 и 0 случаев, то новый = 1.0
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
            // Это нужно, если в DTO/БД КБМ хранится с плавающей точкой, не совпадающей точно с ключом таблицы
            var closestKBM = kbmTable.Keys.OrderBy(k => Math.Abs(k - currentKBM)).FirstOrDefault();

            System.Diagnostics.Debug.WriteLine($"CalculateBonusMalusCoefficient: Найден ближайший КБМ в таблице: {closestKBM}");

            if (kbmTable[closestKBM].TryGetValue(claimCountLastYear, out double newKBM))
            {
                System.Diagnostics.Debug.WriteLine($"CalculateBonusMalusCoefficient: Рассчитан новый КБМ: {newKBM}");
                return newKBM;
            }

            // Если не нашли, возвращаем 1.17 (базовый)
            System.Diagnostics.Debug.WriteLine($"CalculateBonusMalusCoefficient: КБМ для (prev={closestKBM}, claims={claimCountLastYear}) не найден. Возвращаем базовый 1.17.");
            return 1.17;
        }
        // ...

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

        

        private async Task<int> GetClaimCountLastYear(int clientId)
        {
            var claims = await _claimService.GetByClientIdAsync(clientId);
            var lastYear = DateTime.Now.AddYears(-1);
            return claims.Count(c => c.ClaimDate >= lastYear && c.ClaimDate <= DateTime.Now);
        }

        private bool CanCreatePolicy()
        {
            // Проверяем, что выбран тип из доступных и нет ошибки
            return SelectedPolicyType != null && AvailablePolicyTypes.Contains(SelectedPolicyType) && string.IsNullOrEmpty(ErrorMessage);
        }

        private async Task CreatePolicyAsync()
        {
            ErrorMessage = string.Empty;

            if (Vehicle == null)
            {
                ErrorMessage = "Автомобиль не выбран";
                return;
            }

            if (string.IsNullOrEmpty(SelectedPolicyType))
            {
                ErrorMessage = "Выберите тип полиса";
                return;
            }

            // --- ПРОВЕРКА: Убедимся, что выбранный тип всё ещё доступен (защита от гонки) ---
            var activePolicies = await _policyService.GetActivePoliciesByVehicleIdAsync(_vehicleId);
            var activePolicyTypeNames = activePolicies.Select(p => p.TypeName).ToHashSet();
            if (activePolicyTypeNames.Contains(SelectedPolicyType))
            {
                ErrorMessage = $"Для автомобиля {Vehicle.PlateNum} уже есть активный полис типа '{SelectedPolicyType}'.";
                return;
            }
            // ---

            var user = _currentUserService.GetCurrentUser();
            if (user == null)
            {
                ErrorMessage = "Пользователь не авторизован";
                return;
            }

            try
            {
                var policyNumber = GeneratePolicyNumber();

                // Получаем ID типа по имени
                var allTypesList = await _catalogService.GetPolicyTypesAsync();
                var selectedType = allTypesList.FirstOrDefault(t => t == SelectedPolicyType);
                if (selectedType == null)
                {
                    ErrorMessage = "Выбранный тип полиса не найден.";
                    return;
                }
                var typeId = await _catalogService.GetPolicyTypeIdByNameAsync(SelectedPolicyType);

                var policyDto = new Insurance
                {
                    VehicleId = Vehicle.Id, // <-- Используем Id из VehicleDto
                    TypeId = typeId,
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