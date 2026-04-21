using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using Avalonia.Controls;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Configuration;
using System.Linq;

namespace gip.iplus.client.avui.Android;

[Activity(
    Label = "gip.iplus.client.avui.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode,
    WindowSoftInputMode = SoftInput.AdjustResize)]
public class MainActivity : AvaloniaMainActivity
{
    // protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    // {
    //     return base.CustomizeAppBuilder(builder)
    //         .WithInterFont();
    // }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        BackRequested += MainActivity_BackRequested;

        if (this.Content != null && this.Content is UserControl)
        {
            UserControl rootControl = (UserControl)this.Content;
            LoginView? loginView = rootControl.Content as LoginView;

            if (loginView != null)
            {
                loginView.LoginStarted += LoginView_LoginStarted;

                loginView.LoginCancelled += (s, e) =>
                {
                    FinishAffinity();
                    Java.Lang.JavaSystem.Exit(0);
                };
            }
        }
    }

    private void LoginView_LoginStarted(object? sender, EventArgs e)
    {
        ACValueItemList settings = CommandLineHelper.Settings;

        string? dbSource = settings.Where(c => c.ACCaptionTranslation == "DatabaseSource").FirstOrDefault()?.Value as string;
        string? dbName = settings.Where(c => c.ACCaptionTranslation == "DatabaseName").FirstOrDefault()?.Value as string;
        string? dbUser = settings.Where(c => c.ACCaptionTranslation == "DatabaseUser").FirstOrDefault()?.Value as string;
        string? dbPass = settings.Where(c => c.ACCaptionTranslation == "DatabasePassword").FirstOrDefault()?.Value as string;

        var config = CommandLineHelper.ConfigCurrentDir;
        var existingConnection = config?.ConnectionStrings?.ConnectionStrings["iPlusV5_Entities"];

        string connSettingsFormat = @"Integrated Security=True; Encrypt=False; data source={0}; initial catalog={1}; Trusted_Connection=False; persist security info=True; user id={2}; password={3}; multipleactiveresultsets=True; application name=iPlus";

        var setting = new ConnectionStringSettings(
                "iPlusV5_Entities",
                string.Format(connSettingsFormat, dbSource, dbName, dbUser, dbPass),
                "System.Data.SqlClient");

        if (existingConnection != null)
        {
            existingConnection.ProviderName = setting.ProviderName;
            existingConnection.ConnectionString = setting.ConnectionString;
        }
        else if (config != null)
        {
            config.ConnectionStrings.ConnectionStrings.Add(setting);
        }

        existingConnection = config?.ConnectionStrings?.ConnectionStrings["iPlusMESV5_Entities"];

        setting = new ConnectionStringSettings(
                "iPlusMESV5_Entities",
                string.Format(connSettingsFormat, dbSource, dbName, dbUser, dbPass),
                "System.Data.SqlClient");

        if (existingConnection != null)
        {
            existingConnection.ProviderName = setting.ProviderName;
            existingConnection.ConnectionString = setting.ConnectionString;
        }
        else if (config != null)
        {
            config.ConnectionStrings.ConnectionStrings.Add(setting);
        }

        if (sender is LoginView lw)
        {
            lw.LoginStarted -= LoginView_LoginStarted;
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

        if (this.Content != null && this.Content is UserControl)
        {
            UserControl rootControl = (UserControl)this.Content;
            MainSingleView? mainSingleView = rootControl.Content as MainSingleView;

            if (mainSingleView != null)
            {
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
}
