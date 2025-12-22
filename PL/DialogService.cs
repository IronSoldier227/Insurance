using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace PL
{
    public interface IDialogService
    {
        bool? ShowDialog<TWindow>() where TWindow : System.Windows.Window;
    }

    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool? ShowDialog<TWindow>() where TWindow : System.Windows.Window
        {
            var window = _serviceProvider.GetRequiredService<TWindow>();
            return window.ShowDialog();
        }
    }
}
