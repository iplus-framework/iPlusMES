using System;
using Avalonia;
using Avalonia.ReactiveUI;

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
            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
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
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI()
            .WithInterFont()
            .LogToTrace();
}
