using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Interfaces.Services;
using Interfaces.DTO;
using System.Threading.Tasks;
using System.Windows.Controls; 
using System;

namespace PL.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationWindowService _NavigationWindowService;
        private string _errorMessage = string.Empty;

        public RegisterViewModel(
            IUserService userService,
            ICurrentUserService currentUserService,
            INavigationWindowService NavigationWindowService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
            _NavigationWindowService = NavigationWindowService;
            RegisterCommand = new RelayCommand(async param => await RegisterAsync(param as PasswordBox), _ => CanRegister());
        }

        private bool _isClientType = true; 
        public bool IsClientType
        {
            get => _isClientType;
            set { _isClientType = value; OnPropertyChanged(); }
        }

        public bool IsManagerType
        {
            get => !_isClientType;
            set { IsClientType = !value; OnPropertyChanged(); }
        }

        public string Login { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Passport { get; set; } = string.Empty;
        public string DriverLicense { get; set; } = string.Empty; 
        public int DrivingExperience { get; set; } = 0; 

        public ICommand RegisterCommand { get; }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private bool CanRegister() => !string.IsNullOrEmpty(Login);

        public async Task RegisterAsync(PasswordBox? passwordBox)
        {
            System.Diagnostics.Debug.WriteLine("RegisterViewModel.RegisterAsync вызван.");
            ErrorMessage = string.Empty;

            if (passwordBox == null || string.IsNullOrEmpty(passwordBox.Password))
            {
                ErrorMessage = "Пароль не может быть пустым.";
                System.Diagnostics.Debug.WriteLine(ErrorMessage);
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine("Начинаем процесс регистрации...");

                var dto = new UserCreateDto
                {
                    Login = Login,
                    Password = passwordBox.Password,
                    IsClient = IsClientType,
                    FirstName = FirstName,
                    LastName = LastName,
                    MiddleName = MiddleName,
                    PhoneNumber = PhoneNumber,
                    Passport = IsClientType ? Passport : string.Empty,
                    DriverLicense = IsClientType ? DriverLicense : string.Empty,
                    DrivingExperience = IsClientType ? DrivingExperience : 0
                };

                System.Diagnostics.Debug.WriteLine($"Отправляем DTO: Login={dto.Login}, IsClient={dto.IsClient}");

                var id = await _userService.RegisterAsync(dto);
                System.Diagnostics.Debug.WriteLine($"Регистрация успешна, получен ID: {id}");

                var user = await _userService.AuthenticateAsync(Login, passwordBox.Password);
                System.Diagnostics.Debug.WriteLine($"Результат аутентификации: {(user != null ? "Успешно" : "Неудача")}");

                if (user != null)
                {
                    _currentUserService.SetCurrentUser(user);
                    System.Diagnostics.Debug.WriteLine("Пользователь установлен как текущий.");

                    if (user.IsClient)
                    {
                        System.Diagnostics.Debug.WriteLine("Перенаправление на MainWindow.");
                        _NavigationWindowService.NavigateTo<MainWindow>();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Перенаправление на ManagerWindow.");
                        _NavigationWindowService.NavigateTo<ManagerWindow>();
                    }
                    passwordBox.Clear();
                    System.Diagnostics.Debug.WriteLine("Пароль очищен.");
                }
                else
                {
                    ErrorMessage = "Ошибка авторизации после регистрации";
                    System.Diagnostics.Debug.WriteLine(ErrorMessage);
                }
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                System.Diagnostics.Debug.WriteLine($"InvalidOperationException: {ex.Message}");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка регистрации: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Общее исключение: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Стек вызова: {ex.StackTrace}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}