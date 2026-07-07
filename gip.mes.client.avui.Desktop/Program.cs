using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia;
using gip.mes.wpfservices.avui;
using ReactiveUI.Avalonia;

namespace gip.iplus.client.avui.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            if (OperatingSystem.IsLinux())
            {
                // Avalonia WebView GTK initialization expects X11 backend.
                // On Wayland sessions we may inherit GDK_BACKEND=wayland, which makes gtk_init_check fail.
                var gdkBackend = Environment.GetEnvironmentVariable("GDK_BACKEND");
                if (string.IsNullOrWhiteSpace(gdkBackend)
                    || gdkBackend.Contains("wayland", StringComparison.OrdinalIgnoreCase))
                {
                    Environment.SetEnvironmentVariable("GDK_BACKEND", "x11");
                }

                GtkBootstrap.TryInitializeGtkX11();
            }

            App.WpfServicesFactory = static () => new WPFServicesMES();
            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception)
        {
            // here we can work with the exception, for example add it to our log file
            //Log.Fatal(e, "Something very bad happened");            
        }
        finally
        {
            // This block is optional. 
            // Use the finally-block if you need to clean things up or similar
            //Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        var builder = AppBuilder.Configure<App>()
            .UsePlatformDetect();

        builder = AppBuilderHelper.ConfigureLinuxX11Options(builder);

        return builder
            .UseReactiveUI(rxui =>
            {
                // Optional: add custom registration here via rxui.WithRegistration(...)
            })
            .RegisterReactiveUIViewsFromEntryAssembly()
            .WithInterFont()
            .LogToTrace();
    }
}

