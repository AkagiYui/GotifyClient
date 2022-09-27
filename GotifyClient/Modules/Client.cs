using System;
using GotifyClient.Entities;
using Microsoft.WindowsAPICodePack.Dialogs;
using Websocket.Client;
using Websocket.Client.Models;

namespace GotifyClient.Modules
{
    public class Client
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Token { get; set; }
        
        private readonly Listener _listener;
        
        public Client(ServerEntity server,
            Action<ResponseMessage> onMessage = null,
            Action<ReconnectionInfo> onReconnected = null,
            Action<DisconnectionInfo> onDisconnected = null)
        {
            Name = server.Name;
            Host = server.Host;
            Port = server.Port;
            Token = server.Token;
            _listener = new Listener(server, onMessage, onReconnected, onDisconnected);
        }
        
        public void StartListener()
        {
            try
            {
                _listener.Start();
            }
            catch (Exception e)
            {
                new TaskDialog()
                {
                    Icon = TaskDialogStandardIcon.Error,
                    Caption = "GotifyClient",
                    DefaultButton = TaskDialogDefaultButton.Yes,
                    StandardButtons = TaskDialogStandardButtons.Yes,
                    Text = e.Message
                }.Show();
            }
        }
        
        public void StopListener()
        {
            _listener.Stop();
        }
        
        ~Client()
        {
            StopListener();
        }
        
        
    }
}