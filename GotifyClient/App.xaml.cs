using System.Windows;
using GotifyClient.Modules;
using log4net;

namespace GotifyClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger.Info("==Startup=====================>>>");
            Current.Properties.Add("connections", new Connections());
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Logger.Info("<<<========================End==");
            base.OnExit(e);
        }
    }
}