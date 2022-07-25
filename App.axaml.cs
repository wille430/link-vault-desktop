using System.ComponentModel;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LinkVault.Stores;
using LinkVault.ViewModels;
using LinkVault.Views;
using Splat;

namespace LinkVault
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(Locator.Current.GetService<CollectionStore>()!),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}