using Avalonia;
using Avalonia.ReactiveUI;
using LinkVault.Services;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using System;

namespace LinkVault
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);


        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            Bootstrapper.Register(Locator.CurrentMutable);

            // Start server
            var serverService = Locator.Current.GetService<ServerService>();

            if (serverService is not null)
                serverService.RestartServer();
            else
                throw new ApplicationException("Could not start web server");

            return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
        }
    }
}
