using Interfaces.DTO;
using Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using PL;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PL.ViewModels
{
    public class VehiclesViewModel : INotifyPropertyChanged
    {
        private readonly IVehicleService _vehicleService;
        private readonly ICurrentUserService _currentUserService;
        private string _errorMessage = string.Empty;


        public VehiclesViewModel(
            IVehicleService vehicleService,
            ICurrentUserService currentUserService)
        {
            _vehicleService = vehicleService;
            _currentUserService = currentUserService;

            LoadVehiclesCommand = new RelayCommand(async _ => await LoadVehiclesAsync(), (Func<bool>?)null);
            AddVehicleCommand = new RelayCommand(_ => AddVehicle(), (Func<bool>?)null);
            EditVehicleCommand = new RelayCommand(async _ => await EditVehicleAsync(), (Func<bool>?)null);
            DeleteVehicleCommand = new RelayCommand(async _ => await DeleteVehicleAsync(), (Func<bool>?)null);
            InsureVehicleCommand = new RelayCommand(async _ => await InsureVehicleAsync(), (Func<bool>?)null);
        }

        private ObservableCollection<VehicleDto> _vehicles = new ObservableCollection<VehicleDto>();
        public ObservableCollection<VehicleDto> Vehicles
        {
            get => _vehicles;
            set { _vehicles = value; OnPropertyChanged(); }
        }

        private VehicleDto? _selectedVehicle;
        public VehicleDto? SelectedVehicle
        {
            get => _selectedVehicle;
            set { _selectedVehicle = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private string _selectedFilterType = "Все"; 
        public string SelectedFilterType
        {
            get => _selectedFilterType;
            set { _selectedFilterType = value; OnPropertyChanged(); _ = LoadVehiclesAsync(); } 
        }

        public ObservableCollection<string> FilterTypes { get; } = new() { "Все", "Застрахованные", "Незастрахованные" };


        public ICommand LoadVehiclesCommand { get; }
        public ICommand AddVehicleCommand { get; }
        public ICommand EditVehicleCommand { get; }
        public ICommand DeleteVehicleCommand { get; }
        public ICommand InsureVehicleCommand { get; }

        public async Task LoadVehiclesAsync()
        {
            ErrorMessage = string.Empty;
            var user = _currentUserService.GetCurrentUser();
            if (user == null) return;

            try
            {
                var vehicles = await _vehicleService.GetVehiclesByClientIdAsync(user.Id);
                IEnumerable<VehicleDto> filteredVehicles = vehicles;
                switch (SelectedFilterType)
                {
                    case "Застрахованные":
                        filteredVehicles = vehicles.Where(v => v.IsInsured);
                        break;
                    case "Незастрахованные":
                        filteredVehicles = vehicles.Where(v => !v.IsInsured);
                        break;
                    case "Все":
                    default:
                        break; 
                }

                Vehicles.Clear();
                foreach (var vehicle in filteredVehicles)
                {
                    Vehicles.Add(vehicle);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки автомобилей: {ex.Message}";
            }
        }

        private void AddVehicle()
        {
            try
            {
                var vehicleService = App.ServiceProvider.GetRequiredService<IVehicleService>();
                var currentUserService = App.ServiceProvider.GetRequiredService<ICurrentUserService>();
                var catalogService = App.ServiceProvider.GetRequiredService<ICatalogService>();

                var viewModel = new AddEditVehicleViewModel(
                    vehicleService,
                    currentUserService,
                    catalogService);

                var addWindow = new AddEditVehicleWindow
                {
                    DataContext = viewModel,
                };

                if (addWindow.Owner != null)
                {
                    addWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }

                bool? result = addWindow.ShowDialog();

                if (result == true)
                {
                    _ = LoadVehiclesAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Не удалось открыть окно: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task EditVehicleAsync()
        {
            if (SelectedVehicle == null)
            {
                ErrorMessage = "Выберите автомобиль для редактирования";
                return;
            }

            try
            {
                var vehicleService = App.ServiceProvider.GetRequiredService<IVehicleService>();
                var currentUserService = App.ServiceProvider.GetRequiredService<ICurrentUserService>();
                var catalogService = App.ServiceProvider.GetRequiredService<ICatalogService>();

                var viewModel = new AddEditVehicleViewModel(
                    vehicleService,
                    currentUserService,
                    catalogService,
                    SelectedVehicle);

                var editWindow = new AddEditVehicleWindow
                {
                    DataContext = viewModel,
                };

                if (editWindow.Owner != null)
                {
                    editWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }

                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    await LoadVehiclesAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Не удалось открыть окно: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteVehicleAsync()
        {
            if (SelectedVehicle == null)
            {
                ErrorMessage = "Выберите автомобиль для удаления";
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить автомобиль {SelectedVehicle.PlateNum}?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _vehicleService.DeleteVehicleAsync(SelectedVehicle.Id);
                    await LoadVehiclesAsync();
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ошибка удаления: {ex.Message}";
                }
            }
        }
        private async Task InsureVehicleAsync()
        {

            if (SelectedVehicle == null)
            {
                ErrorMessage = "Выберите автомобиль для страхования";
                return;
            }


            try
            {
                var policyService = App.ServiceProvider.GetRequiredService<IPolicyService>();
                var vehicleService = App.ServiceProvider.GetRequiredService<IVehicleService>();
                var currentUserService = App.ServiceProvider.GetRequiredService<ICurrentUserService>();
                var catalogService = App.ServiceProvider.GetRequiredService<ICatalogService>();
                var clientProfileService = App.ServiceProvider.GetRequiredService<IClientProfileService>();
                var claimService = App.ServiceProvider.GetRequiredService<IClaimService>();

                var viewModel = new CreatePolicyViewModel(
                    policyService,
                    vehicleService,
                    currentUserService,
                    catalogService,
                    clientProfileService,
                    claimService,
                    SelectedVehicle.Id);

                var insureWindow = new CreatePolicyWindow(viewModel);
                if (insureWindow.Owner != null)
                {
                    insureWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }

                bool? result = insureWindow.ShowDialog();
                if (result == true)
                {
                    await LoadVehiclesAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при оформлении страховки: {ex.Message}";
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}