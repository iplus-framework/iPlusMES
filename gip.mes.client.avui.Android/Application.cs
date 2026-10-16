using Android.App;
using Avalonia;
using Avalonia.Android;
using Android.Runtime;
using gip.mes.wpfservices.avui;

namespace gip.iplus.client.avui.Android
{
    [Application]
    public class Application : AvaloniaAndroidApplication<App>
    {
        protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            App.WpfServicesFactory = static () => new WPFServicesMES();

            // MES uses a second EF context (iPlusMESV5_Entities) besides the iPlus core
            // context — register it so the login flow applies the SQL Tunnel to it too
            // (see gip.iplus.client.avui.Views.LoginView.AdditionalConnectionContainerNames).
            gip.iplus.client.avui.LoginView.AdditionalConnectionContainerNames.Add(
                gip.mes.datamodel.DatabaseApp.C_DefaultContainerName);
        }


        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            return base.CustomizeAppBuilder(builder);
                //  .AfterSetup(_ =>
                //  {
                //      Pages.EmbedSample.Implementation = new EmbedSampleAndroid();
                //  });
        }
    }
}
