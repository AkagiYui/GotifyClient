using System.Windows;
using GotifyClient.Modules;

namespace GotifyClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Current.Properties.Add("connections", new Connections());
            base.OnStartup(e);
        }
    }
}