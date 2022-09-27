using System;
using System.Drawing;
using System.Windows;
using GotifyClient.Modules;
using Hardcodet.Wpf.TaskbarNotification;
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
            
            // var iconStream = GetResourceStream(new Uri("pack://application:,,,/icon.ico"))?.Stream;
            // var icon = new Icon(iconStream);
            // var tb = (TaskbarIcon)FindResource("Taskbar");
            // tb.ShowBalloonTip("t.Title1", "t.Message", tb.Icon);
            // tb.ShowBalloonTip("t.Title2", "t.Message", BalloonIcon.Info);
            // tb.ball
            // Application.Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ((TaskbarIcon)FindResource("Taskbar"))?.Dispose();
            Logger.Info("<<<========================End==");
            base.OnExit(e);
        }
    }
}