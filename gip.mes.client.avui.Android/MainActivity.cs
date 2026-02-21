using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using System;

namespace gip.iplus.client.avui.Android;

[Activity(
    Label = "gip.iplus.client.avui.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode,
    WindowSoftInputMode = SoftInput.AdjustResize)]
 #if AVALONIAFORK
public class MainActivity : AvaloniaMainActivity
 #else   
public class MainActivity : AvaloniaMainActivity<App>
#endif
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        BackRequested += MainActivity_BackRequested;
        
        if (this.Content != null && this.Content is LoginView loginView) 
        {
            loginView.LoginCancelled += (s, e) =>
            {
                FinishAffinity();
                Java.Lang.JavaSystem.Exit(0);
            };
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        BackRequested -= MainActivity_BackRequested;
    }

    private void MainActivity_BackRequested(object? sender, AndroidBackRequestedEventArgs e)
    {
        e.Handled = true;

        if (this.Content != null && this.Content is MainSingleView)
        {
            MainSingleView mainSingleView = (MainSingleView)this.Content;

            if (mainSingleView.CanClose)
            {
                FinishAffinity();
                Java.Lang.JavaSystem.Exit(0);
                return;
            }

            mainSingleView.BackButton_Click(sender, new Avalonia.Interactivity.RoutedEventArgs());
        }
    }
}
