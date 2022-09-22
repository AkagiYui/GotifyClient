using System;
using System.Net.WebSockets;
using GotifyClient.Entities;
using Microsoft.WindowsAPICodePack.Dialogs;
using Websocket.Client;
using Websocket.Client.Models;

namespace GotifyClient.Modules
{
    public class Listener
    {
        public string Name;
        public string Host;
        public string Port;
        public string Token;
        
        private readonly Action<ResponseMessage> _onMessage;
        private readonly Action<ReconnectionInfo> _onReconnected;
        private readonly Action<DisconnectionInfo> _onDisconnected;
        
        private WebsocketClient _client;

        public Listener(ServerEntity server, 
            Action<ResponseMessage> onMessage = null,
            Action<ReconnectionInfo> onReconnected = null,
            Action<DisconnectionInfo> onDisconnected = null)
        {
            Name = server.Name;
            Host = server.Host;
            Port = server.Port;
            Token = server.Token;
            
            _onMessage = onMessage;
            _onReconnected = onReconnected;
            _onDisconnected = onDisconnected;
        }

        ~Listener()
        {
            if (_client == null) return;
            if (_client.IsStarted)
            {
                _client.Stop(WebSocketCloseStatus.Empty, null);
            }
        }

        public void Start()
        {
            try
            {
                if (_client != null)
                {
                    if (_client.IsStarted) return;
                }
                var url = new Uri($"ws://{Host}:{Port}/stream");
                var factory = new Func<ClientWebSocket>(() =>
                {
                    var c = new ClientWebSocket();
                    c.Options.SetRequestHeader("X-Gotify-Key", Token);
                    return c;
                });
                _client = new WebsocketClient(url, factory);
            
                _client.IsReconnectionEnabled = false;
                _client.ReconnectTimeout = TimeSpan.FromSeconds(30);

                if (_onMessage != null) _client.MessageReceived.Subscribe(_onMessage);
                if (_onReconnected != null) _client.ReconnectionHappened.Subscribe(_onReconnected);
                if (_onDisconnected != null) _client.DisconnectionHappened.Subscribe(_onDisconnected);
                
                _client.Start();
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

        public void Stop()
        {
            if (_client == null) return;
            if (!_client.IsStarted) return;
            _client.Stop(WebSocketCloseStatus.NormalClosure, null);
        }
        
        public override string ToString()
        {
            return $"Listener: {Name}";
        }
    }
}