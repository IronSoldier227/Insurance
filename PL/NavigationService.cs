using Microsoft.Extensions.DependencyInjection;
using PL;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

public interface INavigationWindowService
{
    bool CanGoBack { get; }
    void InitializeMainWindow<TWindow>(TWindow window) where TWindow : Window; 
    void NavigateTo<TWindow>() where TWindow : Window;
    void ShowDialog<TWindow>() where TWindow : Window;
    void GoBack();
}

public class NavigationWindowService : INavigationWindowService, INotifyPropertyChanged
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<Type> _windowHistory;
    private Window? _currentWindow;

    public NavigationWindowService(IServiceProvider serviceProvider)
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

    public void InitializeMainWindow<TWindow>(TWindow window) where TWindow : Window
    {
        _currentWindow = window;
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
    }

    private void UpdateCanGoBack()
    {
        CanGoBack = _windowHistory.Count > 0;
    }

    public void NavigateTo<TWindow>() where TWindow : Window
    {
        System.Diagnostics.Debug.WriteLine($"NavigationWindowService.NavigateTo<{typeof(TWindow).Name}> вызван.");
        System.Diagnostics.Debug.WriteLine($"Текущее окно перед закрытием: {_currentWindow?.GetType().Name ?? "null"}");
        System.Diagnostics.Debug.WriteLine($"История окон до добавления: [{string.Join(", ", _windowHistory.Select(t => t.Name))}]");

        var viewModelTypeName = typeof(TWindow).Name.Replace("Window", "ViewModel");
        var viewModelType = AppDomain.CurrentDomain.GetAssemblies()
                               .SelectMany(x => x.GetTypes())
                               .FirstOrDefault(x => x.Name == viewModelTypeName && x.Namespace == "PL.ViewModels");

        System.Diagnostics.Debug.WriteLine($"Поиск ViewModel: {viewModelTypeName} -> {(viewModelType != null ? viewModelType.Name : "не найдена")}");

        if (viewModelType != null)
        {
            if (_currentWindow != null)
            {
                _windowHistory.Push(_currentWindow.GetType());
                UpdateCanGoBack();
                System.Diagnostics.Debug.WriteLine($"Текущее окно {_currentWindow.GetType().Name} добавлено в историю.");
                System.Diagnostics.Debug.WriteLine($"История окон после добавления: [{string.Join(", ", _windowHistory.Select(t => t.Name))}]");
            }

            _currentWindow?.Close();
            System.Diagnostics.Debug.WriteLine($"Текущее окно {_currentWindow?.GetType().Name} закрыто.");

            var newWindow = (TWindow)Activator.CreateInstance(typeof(TWindow));
            var viewModel = _serviceProvider.GetRequiredService(viewModelType);

            newWindow.DataContext = viewModel;
            System.Diagnostics.Debug.WriteLine($"Установлен DataContext: {viewModel.GetType().Name} для окна {newWindow.GetType().Name}");

            newWindow.Show();
            _currentWindow = newWindow;
            System.Diagnostics.Debug.WriteLine($"Новое окно {newWindow.GetType().Name} открыто и установлено как текущее.");
        }
        System.Diagnostics.Debug.WriteLine($"--- Конец NavigateTo<{typeof(TWindow).Name}> ---");
    }
    public void GoBack()
    {
        Debug.WriteLine("NavigationWindowService.GoBack вызван.");
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
        UpdateCanGoBack(); 
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

public interface IPageNavigationWindowService
{
    bool CanGoBack { get; }
    void NavigateTo<TPage>() where TPage : Page;
    void GoBack();
    void InitializeFrame(Frame frame); 
}
public class PageNavigationWindowService : IPageNavigationWindowService
{
    private Frame? _frame;

    public bool CanGoBack => _frame?.CanGoBack == true;

    public void InitializeFrame(Frame frame)
    {
        _frame = frame ?? throw new ArgumentNullException(nameof(frame));
    }


    public void NavigateTo<TPage>() where TPage : Page
    {
        if (_frame == null)
            throw new InvalidOperationException("Frame не инициализирован. Вызовите InitializeFrame сначала.");

        var page = App.ServiceProvider.GetRequiredService<TPage>();
        _frame.Navigate(page);
    }

    public void GoBack()
    {
        if (_frame != null && _frame.CanGoBack)
        {
            _frame.GoBack();
        }
    }
}