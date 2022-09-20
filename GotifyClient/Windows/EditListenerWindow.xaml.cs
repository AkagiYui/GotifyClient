using System.Windows;
using GotifyClient.Entities;
using GotifyClient.Modules;

namespace GotifyClient.Windows
{
    public partial class EditListenerWindow : Window
    {
        private static readonly Connections Connections = Application.Current.Properties["connections"] as Connections;
        public EditListenerWindow(Listener listener = null)
        {
            InitializeComponent();
            if (listener == null) return;
            TextBoxName.Text = listener.Name;
            TextBoxHost.Text = listener.Host;
            TextBoxPort.Text = listener.Port;
            TextBoxToken.Text = listener.Token;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxName.Text)) return;
            if (string.IsNullOrEmpty(TextBoxHost.Text)) return;
            if (string.IsNullOrEmpty(TextBoxPort.Text)) return;
            if (string.IsNullOrEmpty(TextBoxToken.Text)) return;
            Connections.AddServer(new ServerEntity()
            {
                Name = TextBoxName.Text,
                Host = TextBoxHost.Text,
                Port = TextBoxPort.Text,
                Token = TextBoxToken.Text
            });
            Close();
        }
    }
}