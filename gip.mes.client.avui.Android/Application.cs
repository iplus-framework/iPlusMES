using Android.App;
using Avalonia;
using Avalonia.Android;
using Android.Runtime;

namespace gip.iplus.client.avui.Android
{
     #if AVALONIAFORK
    [Application]
    public class Application : AvaloniaAndroidApplication<App>
    {
        protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
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
    #endif
}
