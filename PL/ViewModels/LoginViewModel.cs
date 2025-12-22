using Interfaces.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace PL.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        private readonly INavigationWindowService _NavigationWindowService;
        private string _errorMessage = string.Empty;

        public LoginViewModel(
        IAuthService authService,
        ICurrentUserService currentUserService,
        INavigationWindowService NavigationWindowService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
            _NavigationWindowService = NavigationWindowService;

            LoginCommand = new RelayCommand(async param => await LoginAsync(param as PasswordBox), _ => CanLogin());
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
        public ICommand NavigateToRegisterCommand { get; }

        private bool CanLogin() => !string.IsNullOrEmpty(Login);

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

                    if (user.IsClient)
                    {
                        _NavigationWindowService.NavigateTo<MainWindow>();
                    }
                    else 
                    {
                        _NavigationWindowService.NavigateTo<ManagerWindow>(); 
                    }

                    passwordBox.Clear();
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
        private void NavigateToRegister()
        {
            Debug.WriteLine("LoginViewModel.NavigateToRegister вызван.");
            _NavigationWindowService.NavigateTo<RegisterWindow>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}