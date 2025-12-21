using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Interfaces.Services;
using Interfaces.DTO;
using System.Threading.Tasks;
using System.Linq;
using Core.Entities;
using System.Windows;

namespace PL.ViewModels
{
    public class AddEditVehicleViewModel : INotifyPropertyChanged
    {
        private readonly IVehicleService _vehicleService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICatalogService _catalogService;
        private readonly VehicleDto? _editingVehicle;

        private bool _isEditingMode;
        public bool IsEditingMode
        {
            get => _isEditingMode;
            private set { _isEditingMode = value; OnPropertyChanged(); }
        }

        public AddEditVehicleViewModel(
            IVehicleService vehicleService,
            ICurrentUserService currentUserService,
            ICatalogService catalogService,
            VehicleDto? editingVehicle = null)
        {
            _vehicleService = vehicleService;
            _currentUserService = currentUserService;
            _catalogService = catalogService;
            _editingVehicle = editingVehicle;

            // Устанавливаем режим
            IsEditingMode = editingVehicle != null;

            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel(), (Func<bool>?)null);

            // Загружаем данные
            _ = LoadInitialDataAsync();
        }

        private ObservableCollection<Brand> _brands = new ObservableCollection<Brand>();
        public ObservableCollection<Brand> Brands
        {
            get => _brands;
            set { _brands = value; OnPropertyChanged(); }
        }

        private Brand? _selectedBrand;
        public Brand? SelectedBrand
        {
            get => _selectedBrand;
            set
            {
                _selectedBrand = value;
                OnPropertyChanged();
                if (!IsEditingMode) // Загружаем модели только при добавлении
                {
                    _ = LoadModelsAsync();
                }
            }
        }

        private ObservableCollection<Model> _models = new ObservableCollection<Model>();
        public ObservableCollection<Model> Models
        {
            get => _models;
            set { _models = value; OnPropertyChanged(); }
        }

        private Model? _selectedModel;
        public Model? SelectedModel
        {
            get => _selectedModel;
            set { _selectedModel = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _categories = new ObservableCollection<string>();
        public ObservableCollection<string> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }

        // SelectedCategory используется как для добавления, так и для отображения при редактировании
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

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        // Свойства только для чтения для отображения неизменяемых данных при редактировании
        public string BrandName { get; private set; } = string.Empty;
        public string ModelName { get; private set; } = string.Empty;
        public string YearOfProductionText { get; private set; } = string.Empty;
        public string VinText { get; private set; } = string.Empty;
        public string CategoryText { get; private set; } = string.Empty; // Для отображения при редактировании

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadInitialDataAsync()
        {
            try
            {
                if (!IsEditingMode) // Загружаем только при добавлении
                {
                    // Загружаем марки
                    var brands = await _catalogService.GetAllBrandsAsync();
                    Brands.Clear();
                    foreach (var brand in brands)
                    {
                        Brands.Add(brand);
                    }

                    // Загружаем категории
                    var categories = await _catalogService.GetCategoriesAsync();
                    Categories.Clear();
                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }
                }
                else // Режим редактирования
                {
                    await LoadEditingDataAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки данных: {ex.Message}";
            }
        }

        private async Task LoadEditingDataAsync()
        {
            if (_editingVehicle == null) return;

            try
            {
                // Устанавливаем значения для редактируемых полей
                Color = _editingVehicle.Color ?? "";
                PlateNum = _editingVehicle.PlateNum ?? "";
                PowerHp = _editingVehicle.PowerHp.ToString("0"); // Устанавливаем как строку

                // Устанавливаем значения для НЕ редактируемых полей (только для отображения)
                BrandName = _editingVehicle.Brand ?? "Неизвестно";
                ModelName = _editingVehicle.Model ?? "Неизвестно";
                YearOfProductionText = _editingVehicle.YearOfProduction.ToString();
                VinText = _editingVehicle.Vin ?? "Неизвестен";
                CategoryText = _editingVehicle.Category ?? "Неизвестно";

                // Устанавливаем SelectedCategory для отображения в ComboBox (если он видим)
                // Это значение будет использоваться при валидации, но НЕ будет отправлено в Update DTO
                SelectedCategory = _editingVehicle.Category;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки данных автомобиля: {ex.Message}";
            }
        }

        private async Task LoadModelsAsync()
        {
            if (SelectedBrand == null) return;

            try
            {
                var models = await _catalogService.GetModelsByBrandIdAsync(SelectedBrand.Id);
                Models.Clear();
                foreach (var model in models)
                {
                    Models.Add(model);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки моделей: {ex.Message}";
            }
        }

        private bool CanSave()
        {
            // Можно улучшить, но базовая проверка
            if (!IsEditingMode)
            {
                return !string.IsNullOrWhiteSpace(Color) &&
                       !string.IsNullOrWhiteSpace(YearOfProduction) &&
                       !string.IsNullOrWhiteSpace(Vin) &&
                       !string.IsNullOrWhiteSpace(PlateNum) &&
                       !string.IsNullOrWhiteSpace(SelectedCategory) &&
                       !string.IsNullOrWhiteSpace(PowerHp) &&
                       SelectedModel != null;
            }
            else
            {
                // Только редактируемые поля
                return !string.IsNullOrWhiteSpace(Color) &&
                       !string.IsNullOrWhiteSpace(PlateNum) &&
                       !string.IsNullOrWhiteSpace(PowerHp);
            }
        }

        public async Task SaveAsync()
        {
            ErrorMessage = string.Empty;
            System.Diagnostics.Debug.WriteLine($"AddEditVehicleViewModel.SaveAsync вызван. IsEditingMode: {IsEditingMode}");
            ErrorMessage = string.Empty;

            if (!IsEditingMode) // Режим добавления
            {
                // Валидация для добавления (включая SelectedCategory)
                if (SelectedModel == null)
                {
                    ErrorMessage = "Выберите модель автомобиля";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Color))
                {
                    ErrorMessage = "Введите цвет автомобиля";
                    return;
                }

                if (!int.TryParse(YearOfProduction, out int yearAdd) || yearAdd < 1900 || yearAdd > DateTime.Now.Year + 1)
                {
                    ErrorMessage = "Введите корректный год выпуска (1900-" + (DateTime.Now.Year + 1) + ")";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Vin) || Vin.Length != 17)
                {
                    ErrorMessage = "VIN номер должен содержать 17 символов";
                    return;
                }

                if (string.IsNullOrWhiteSpace(PlateNum))
                {
                    ErrorMessage = "Введите государственный номер";
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedCategory))
                {
                    ErrorMessage = "Выберите категорию ТС";
                    return;
                }

                if (!int.TryParse(PowerHp, out int powerAdd) || powerAdd <= 0)
                {
                    ErrorMessage = "Введите корректную мощность (больше 0)";
                    return;
                }

                var user = _currentUserService.GetCurrentUser();
                if (user == null)
                {
                    ErrorMessage = "Пользователь не авторизован";
                    return;
                }

                try
                {
                    var dto = new VehicleCreateDto
                    {
                        ModelId = SelectedModel.Id,
                        ClientId = user.Id,
                        Color = Color,
                        YearOfProduction = yearAdd,
                        Vin = Vin.ToUpper(),
                        PlateNum = PlateNum.ToUpper(),
                        Category = SelectedCategory,
                        PowerHp = powerAdd
                    };

                    await _vehicleService.AddVehicleAsync(dto);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ошибка сохранения: {ex.Message}";
                    return; // Важно выйти из метода, если произошла ошибка при добавлении
                }
            }
            else // Режим редактирования
            {
                System.Diagnostics.Debug.WriteLine($"Режим редактирования. _editingVehicle.Id: {_editingVehicle?.Id}");
                System.Diagnostics.Debug.WriteLine($"Режим редактирования. ViewModel Color: '{Color}', PlateNum: '{PlateNum}', PowerHp: '{PowerHp}'");

                // Валидация только для изменяемых полей: Color, PlateNum, PowerHp
                if (string.IsNullOrWhiteSpace(Color))
                {
                    ErrorMessage = "Введите цвет автомобиля";
                    System.Diagnostics.Debug.WriteLine(ErrorMessage);
                    return;
                }

                if (string.IsNullOrWhiteSpace(PlateNum))
                {
                    ErrorMessage = "Введите государственный номер";
                    System.Diagnostics.Debug.WriteLine(ErrorMessage);
                    return;
                }

                if (!int.TryParse(PowerHp, out int powerEdit) || powerEdit <= 0)
                {
                    ErrorMessage = "Введите корректную мощность (больше 0)";
                    System.Diagnostics.Debug.WriteLine(ErrorMessage);
                    return;
                }

                var user = _currentUserService.GetCurrentUser();
                if (user == null)
                {
                    ErrorMessage = "Пользователь не авторизован";
                    System.Diagnostics.Debug.WriteLine(ErrorMessage);
                    return;
                }

                try
                {
                    // Используем _editingVehicle.Id для обновления
                    var dto = new VehicleUpdateDto
                    {
                        Id = _editingVehicle.Id, // Важно!
                                                 // НЕ присваиваем Category!
                    };

                    // Присваиваем только те поля, которые можно изменить
                    dto.Color = Color;
                    dto.PlateNum = PlateNum.ToUpper();
                    dto.PowerHp = powerEdit; // <-- Присваиваем мощность

                    System.Diagnostics.Debug.WriteLine($"VehicleUpdateDto создан: Id={dto.Id}, Color='{dto.Color}', PlateNum='{dto.PlateNum}', PowerHp={dto.PowerHp}");

                    await _vehicleService.UpdateVehicleAsync(dto);
                    System.Diagnostics.Debug.WriteLine($"VehicleService.UpdateVehicleAsync завершён успешно для Id={dto.Id}");
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ошибка сохранения: {ex.Message}";
                    System.Diagnostics.Debug.WriteLine($"Ошибка в SaveAsync: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Стек вызова: {ex.StackTrace}");
                    return; // Важно выйти из метода, если произошла ошибка при редактировании
                }
            }

            // Если дошли до этого места, значит всё прошло успешно
            System.Diagnostics.Debug.WriteLine("SaveAsync завершён успешно, закрываем окно.");
            CloseWindow(true);

        }

        private void Cancel()
        {
            CloseWindow(false);
        }

        private void CloseWindow(bool dialogResult)
        {
            // Находим окно, к которому привязана эта ViewModel
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