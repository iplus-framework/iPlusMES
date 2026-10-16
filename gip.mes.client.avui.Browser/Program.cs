using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using gip.iplus.client.avui;
using gip.mes.wpfservices.avui;

internal sealed partial class Program
{
    static Program()
    {
        App.WpfServicesFactory = static () => new WPFServicesMES();

        // Browser/WASM has no ConnectionStrings.config: register the app-specific
        // EF context (iPlusMESV5_Entities) so the SQL Tunnel login derives its
        // connection string from the default container (the relay is a blind proxy
        // to ONE SQL server — see LoginView.AdditionalConnectionContainerNames).
        gip.iplus.client.avui.LoginView.AdditionalConnectionContainerNames.Add(
            gip.mes.datamodel.DatabaseApp.C_DefaultContainerName);
    }

    private static Task Main(string[] args) => BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}
