using System.Windows;
using GotifyClient.Entities;
using GotifyClient.Modules;

namespace GotifyClient.Windows
{
    public partial class EditListenerWindow
    {
        private static readonly Connections Connections = Application.Current.Properties["connections"] as Connections;
        private static ServerEntity _isEdit;
        public EditListenerWindow(Client client = null)
        {
            InitializeComponent();
            if (client == null) return;
            _isEdit = new ServerEntity
            {
                Name = client.Name,
                Host = client.Host,
                Port = client.Port,
                Token = client.Token
            };
            TextBoxName.Text = client.Name;
            TextBoxHost.Text = client.Host;
            TextBoxPort.Text = client.Port;
            TextBoxToken.Text = client.Token;
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
            }, _isEdit);
            Close();
        }
    }
}