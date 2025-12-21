// PL/NavigationService.cs
// ...
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

public interface INavigationService
{
    bool CanGoBack { get; }
    void InitializeMainWindow<TWindow>(TWindow window) where TWindow : Window; // Новый метод
    void NavigateTo<TWindow>() where TWindow : Window;
    void ShowDialog<TWindow>() where TWindow : Window;
    void GoBack();
}

public class NavigationService : INavigationService, INotifyPropertyChanged
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<Type> _windowHistory;
    private Window? _currentWindow;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _windowHistory = new Stack<Type>();
    }

    private bool _canGoBack;
    public bool CanGoBack
    {
        get => _canGoBack;
        private set
        {
            if (_canGoBack != value)
            {
                _canGoBack = value;
                OnPropertyChanged();
            }
        }
    }

    // Новый метод для инициализации начального окна
    public void InitializeMainWindow<TWindow>(TWindow window) where TWindow : Window
    {
        // Устанавливаем переданное окно как текущее, не закрывая предыдущее (его нет)
        _currentWindow = window;
        // Устанавливаем DataContext, если он не установлен
        if (window.DataContext == null)
        {
            var viewModelTypeName = typeof(TWindow).Name.Replace("Window", "ViewModel");
            var viewModelType = AppDomain.CurrentDomain.GetAssemblies()
                                   .SelectMany(x => x.GetTypes())
                                   .FirstOrDefault(x => x.Name == viewModelTypeName && x.Namespace == "PL.ViewModels");

            if (viewModelType != null)
            {
                var viewModel = _serviceProvider.GetRequiredService(viewModelType);
                window.DataContext = viewModel;
            }
        }
        // window.Show() должен быть вызван вне NavigationService
    }

    // PL/NavigationService.cs
    // ...
    // PL/NavigationService.cs
    // ...
    private void UpdateCanGoBack()
    {
        CanGoBack = _windowHistory.Count > 0;
    }

    public void NavigateTo<TWindow>() where TWindow : Window
    {
        Debug.WriteLine($"NavigationService.NavigateTo<{typeof(TWindow).Name}> вызван.");
        Debug.WriteLine($"Текущее окно перед закрытием: {_currentWindow?.GetType().Name ?? "null"}");
        Debug.WriteLine($"История окон до добавления: [{string.Join(", ", _windowHistory.Select(t => t.Name))}]");

        var viewModelTypeName = typeof(TWindow).Name.Replace("Window", "ViewModel");
        var viewModelType = AppDomain.CurrentDomain.GetAssemblies()
                               .SelectMany(x => x.GetTypes())
                               .FirstOrDefault(x => x.Name == viewModelTypeName && x.Namespace == "PL.ViewModels");

        Debug.WriteLine($"Поиск ViewModel: {viewModelTypeName} -> {(viewModelType != null ? viewModelType.Name : "не найдена")}");

        if (viewModelType != null)
        {
            if (_currentWindow != null)
            {
                _windowHistory.Push(_currentWindow.GetType());
                UpdateCanGoBack(); // <-- Обновляем CanGoBack
                Debug.WriteLine($"Текущее окно {_currentWindow.GetType().Name} добавлено в историю.");
                Debug.WriteLine($"История окон после добавления: [{string.Join(", ", _windowHistory.Select(t => t.Name))}]");
            }

            _currentWindow?.Close();
            Debug.WriteLine($"Текущее окно {_currentWindow?.GetType().Name} закрыто.");

            var newWindow = (TWindow)Activator.CreateInstance(typeof(TWindow));
            var viewModel = _serviceProvider.GetRequiredService(viewModelType);
            newWindow.DataContext = viewModel;
            Debug.WriteLine($"Установлен DataContext: {viewModel.GetType().Name}");

            newWindow.Show();
            _currentWindow = newWindow;
            Debug.WriteLine($"Новое окно {newWindow.GetType().Name} открыто и установлено как текущее.");
        }
        else
        {
            if (_currentWindow != null)
            {
                _windowHistory.Push(_currentWindow.GetType());
                UpdateCanGoBack(); // <-- Обновляем CanGoBack
                Debug.WriteLine($"Текущее окно (без ViewModel) {_currentWindow.GetType().Name} добавлено в историю.");
                Debug.WriteLine($"История окон после добавления: [{string.Join(", ", _windowHistory.Select(t => t.Name))}]");
            }

            _currentWindow?.Close();
            Debug.WriteLine($"Текущее окно (без ViewModel) {_currentWindow?.GetType().Name} закрыто.");

            var newWindow = (TWindow)Activator.CreateInstance(typeof(TWindow));
            Debug.WriteLine($"Создан экземпляр окна (без ViewModel): {newWindow.GetType().Name}");

            newWindow.Show();
            _currentWindow = newWindow;
            Debug.WriteLine($"Новое окно (без ViewModel) {newWindow.GetType().Name} открыто и установлено как текущее.");
        }
        Debug.WriteLine($"--- Конец NavigateTo<{typeof(TWindow).Name}> ---");
    }

    public void GoBack()
    {
        Debug.WriteLine("NavigationService.GoBack вызван.");
        Debug.WriteLine($"CanGoBack: {CanGoBack}");
        Debug.WriteLine($"История окон перед извлечением: [{string.Join(", ", _windowHistory.Select(t => t.Name))}]");

        if (!CanGoBack)
        {
            Debug.WriteLine("Невозможно вернуться назад: история пуста.");
            return;
        }

        _currentWindow?.Close();
        Debug.WriteLine($"Текущее окно {_currentWindow?.GetType().Name} закрыто.");

        var previousWindowType = _windowHistory.Pop();
        UpdateCanGoBack(); // <-- Обновляем CanGoBack
        Debug.WriteLine($"Извлечён тип предыдущего окна: {previousWindowType.Name}");
        Debug.WriteLine($"История окон после извлечения: [{string.Join(", ", _windowHistory.Select(t => t.Name))}]");

        var previousWindowTypeName = previousWindowType.Name;
        var viewModelTypeName = previousWindowTypeName.Replace("Window", "ViewModel");
        var viewModelType = AppDomain.CurrentDomain.GetAssemblies()
                               .SelectMany(x => x.GetTypes())
                               .FirstOrDefault(x => x.Name == viewModelTypeName && x.Namespace == "PL.ViewModels");

        if (viewModelType != null)
        {
            var previousWindow = (Window)Activator.CreateInstance(previousWindowType);
            var viewModel = _serviceProvider.GetRequiredService(viewModelType);
            previousWindow.DataContext = viewModel;
            Debug.WriteLine($"Установлен DataContext для предыдущего окна: {viewModel.GetType().Name}");

            previousWindow.Show();
            _currentWindow = previousWindow;
            Debug.WriteLine($"Предыдущее окно {previousWindow.GetType().Name} открыто и установлено как текущее.");
        }
        else
        {
            var previousWindow = (Window)Activator.CreateInstance(previousWindowType);
            Debug.WriteLine($"Создано предыдущее окно (без ViewModel): {previousWindow.GetType().Name}");
            previousWindow.Show();
            _currentWindow = previousWindow;
            Debug.WriteLine($"Предыдущее окно (без ViewModel) {previousWindow.GetType().Name} открыто и установлено как текущее.");
        }
    }

    public void ShowDialog<TWindow>() where TWindow : Window
    {
        var viewModelTypeName = typeof(TWindow).Name.Replace("Window", "ViewModel");
        var viewModelType = AppDomain.CurrentDomain.GetAssemblies()
                               .SelectMany(x => x.GetTypes())
                               .FirstOrDefault(x => x.Name == viewModelTypeName && x.Namespace == "PL.ViewModels");

        if (viewModelType != null)
        {
            var dialog = (TWindow)Activator.CreateInstance(typeof(TWindow));
            var viewModel = _serviceProvider.GetRequiredService(viewModelType);
            dialog.DataContext = viewModel;
            System.Diagnostics.Debug.WriteLine($"ShowDialog: Set DataContext for {typeof(TWindow).Name} to {viewModel.GetType().Name}");

            dialog.Owner = _currentWindow;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.ShowDialog();
        }
        else
        {
            var dialog = (TWindow)Activator.CreateInstance(typeof(TWindow));
            System.Diagnostics.Debug.WriteLine($"ShowDialog: Creating dialog of type: {typeof(TWindow).Name} WITHOUT ViewModel (not found by namespace)");
            dialog.Owner = _currentWindow;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.ShowDialog();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}