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

    private static AppBuilder ConfigureLinuxX11Options(AppBuilder builder)
    {
        if (!OperatingSystem.IsLinux())
            return builder;

        try
        {
            var x11Assembly = Assembly.Load("Avalonia.X11");
            var x11OptionsType = x11Assembly.GetType("Avalonia.X11PlatformOptions")
                                 ?? x11Assembly.GetType("Avalonia.X11.X11PlatformOptions")
                                 ?? x11Assembly.GetTypes().FirstOrDefault(t => t.Name == "X11PlatformOptions");
            if (x11OptionsType == null)
                return builder;

            var x11Options = Activator.CreateInstance(x11OptionsType);
            x11OptionsType.GetProperty("UseGLibMainLoop")?.SetValue(x11Options, true);

            var withMethod = typeof(AppBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == "With"
                                     && m.IsGenericMethodDefinition
                                     && m.GetGenericArguments().Length == 1
                                     && m.GetParameters().Length == 1
                                     && !m.GetParameters()[0].ParameterType.IsGenericType);

            if (withMethod != null && x11Options != null)
            {
                var configuredBuilder = withMethod.MakeGenericMethod(x11OptionsType)
                    .Invoke(builder, [x11Options]) as AppBuilder;
                if (configuredBuilder != null)
                    builder = configuredBuilder;
            }
        }
        catch
        {
            // Ignore: if X11 options are unavailable, keep default platform behavior.
        }

        return builder;
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        var builder = AppBuilder.Configure<App>()
            .UsePlatformDetect();

        builder = ConfigureLinuxX11Options(builder);

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

internal static class GtkBootstrap
{
    [DllImport("libc.so.6", CallingConvention = CallingConvention.Cdecl)]
    private static extern int setenv(string name, string value, int overwrite);

    [DllImport("libgdk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr gdk_display_get_default();

    [DllImport("libgdk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr gdk_set_allowed_backends(IntPtr backends);

    [DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool gtk_init_check(int argc, IntPtr argv);

    public static void TryInitializeGtkX11()
    {
        try
        {
            setenv("GDK_BACKEND", "x11", 1);
            setenv("WAYLAND_DISPLAY", "/proc/fake-display-to-prevent-wayland-initialization-by-gtk3", 1);

            Trace.WriteLine($"GtkBootstrap: DISPLAY={Environment.GetEnvironmentVariable("DISPLAY")}; GDK_BACKEND={Environment.GetEnvironmentVariable("GDK_BACKEND")}; WAYLAND_DISPLAY={Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")}");

            if (gdk_display_get_default() != IntPtr.Zero)
            {
                Trace.WriteLine("GtkBootstrap: display already exists");
                return;
            }

            var x11 = Marshal.StringToHGlobalAnsi("x11");
            try
            {
                gdk_set_allowed_backends(x11);
            }
            finally
            {
                Marshal.FreeHGlobal(x11);
            }

            Environment.SetEnvironmentVariable("WAYLAND_DISPLAY", "/proc/fake-display-to-prevent-wayland-initialization-by-gtk3");
            var ok = gtk_init_check(0, IntPtr.Zero);
            Trace.WriteLine($"GtkBootstrap: gtk_init_check={ok}");
        }
        catch
        {
            // Keep startup resilient; Avalonia WebView will report detailed GTK errors if this fails.
        }
    }
}
