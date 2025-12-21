// PL/ViewModels/RegisterViewModel.cs
using Interfaces.DTO;
using Interfaces.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls; // Для PasswordBox
using System.Windows.Input;
using System.Diagnostics;

namespace PL.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationService _navigationService; // Добавляем
        private string _errorMessage = string.Empty;

        // PL/ViewModels/RegisterViewModel.cs
        // ...
        public RegisterViewModel(
            IUserService userService,
            ICurrentUserService currentUserService,
            INavigationService navigationService)
        {
            Debug.WriteLine("RegisterViewModel.ctor вызван.");
            _userService = userService;
            _currentUserService = currentUserService;
            _navigationService = navigationService;
            // Изменяем команду: RegisterCommand теперь принимает PasswordBox
            RegisterCommand = new RelayCommand(async param => await RegisterAsync(param as PasswordBox), _ => CanRegister());
            Debug.WriteLine("RegisterViewModel.ctor завершён.");
        }
        // ...

        public bool IsClient { get; set; } = true;
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

        private string _login = string.Empty;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged(); // Уведомляем об изменении
                                     // CommandManager.RequerySuggested вызовет CanExecute снова
            }
        }

        // В RegisterViewModel.cs
        private bool CanRegister()
        {
            var result = !string.IsNullOrEmpty(Login);
            Debug.WriteLine($"RegisterViewModel.CanRegister вызван. Login.Length: {Login?.Length ?? 0}, Result: {result}");
            return result;
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            Console.WriteLine($"OnPropertyChanged called for: {name}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // PL/ViewModels/RegisterViewModel.cs
        // ...
        public async Task RegisterAsync(PasswordBox? passwordBox)
        {
            Debug.WriteLine("RegisterViewModel.RegisterAsync вызван.");
            ErrorMessage = string.Empty;

            if (passwordBox == null || string.IsNullOrEmpty(passwordBox.Password))
            {
                ErrorMessage = "Пароль не может быть пустым.";
                return;
            }

            try
            {
                var dto = new UserCreateDto
                {
                    Login = Login,
                    Password = passwordBox.Password,
                    IsClient = true,
                    FirstName = FirstName,
                    LastName = LastName,
                    MiddleName = MiddleName,
                    PhoneNumber = PhoneNumber,
                    Passport = Passport,
                    DriverLicense = DriverLicense,
                    DrivingExperience = DrivingExperience
                };

                var id = await _userService.RegisterAsync(dto);

                var user = await _userService.AuthenticateAsync(Login, passwordBox.Password);
                if (user != null)
                {
                    _currentUserService.SetCurrentUser(user);
                    Debug.WriteLine("Регистрация успешна, вызов NavigateTo<MainWindow>.");
                    // Переходим на главное окно. RegisterWindow закроется.
                    _navigationService.NavigateTo<MainWindow>();
                }
                else
                {
                    ErrorMessage = "Ошибка авторизации после регистрации";
                }
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка регистрации: {ex.Message}";
            }
        }
        // ...

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}