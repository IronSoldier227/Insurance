// PL/ViewModels/LoginViewModel.cs
using Interfaces.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls; // Для PasswordBox
using System.Windows.Input;
using System.Diagnostics;

namespace PL.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationService _navigationService; // Добавляем
        private string _errorMessage = string.Empty; // Добавим свойство для ошибки

        public LoginViewModel(
        IAuthService authService,
        ICurrentUserService currentUserService,
        INavigationService navigationService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
            _navigationService = navigationService;

            LoginCommand = new RelayCommand(async param => await LoginAsync(param as PasswordBox), _ => CanLogin());
            // Убираем команду NavigateToRegisterCommand, если она больше не нужна как отдельная
            // NavigateToRegisterCommand = new RelayCommand(_ => NavigateToRegister());
            // Или оставим, но изменим её логику
            NavigateToRegisterCommand = new RelayCommand(_ => NavigateToRegister(), (Func<bool>?)null);
        }

        private string _login = string.Empty;
        public string Login { get => _login; set { _login = value; OnPropertyChanged(); } }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; } // Новая команда

        private bool CanLogin() => !string.IsNullOrEmpty(Login); // Пароль проверим при выполнении

        public async Task LoginAsync(PasswordBox? passwordBox)
        {
            ErrorMessage = string.Empty;

            if (passwordBox == null || string.IsNullOrEmpty(passwordBox.Password))
            {
                ErrorMessage = "Пароль не может быть пустым.";
                return;
            }

            try
            {
                var user = await _authService.AuthenticateAsync(Login, passwordBox.Password);
                if (user != null)
                {
                    _currentUserService.SetCurrentUser(user);
                    // Переходим на главное окно. LoginWindow закроется.
                    _navigationService.NavigateTo<MainWindow>();
                    passwordBox.Clear(); // Опционально
                }
                else
                {
                    ErrorMessage = "Неверный логин или пароль.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка аутентификации: {ex.Message}";
            }
        }

        // PL/ViewModels/LoginViewModel.cs
        // ...
        private void NavigateToRegister()
        {
            Debug.WriteLine("LoginViewModel.NavigateToRegister вызван.");
            // ИСПОЛЬЗУЕМ NavigateTo, а не ShowDialog
            _navigationService.NavigateTo<RegisterWindow>();
        }
        // ...


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}