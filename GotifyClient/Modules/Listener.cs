using System;
using System.Net.WebSockets;
using Websocket.Client;
using Websocket.Client.Models;

namespace GotifyClient.Modules
{
    public class Listener
    {
        private readonly WebsocketClient _client;

        public readonly string Token;
        
        public Listener(string hostAndPort, string token, 
            Action<ResponseMessage> onMessage = null,
            Action<ReconnectionInfo> onReconnected = null,
            Action<DisconnectionInfo> onDisconnected = null)
        {
            var url = new Uri($"ws://{hostAndPort}/stream");
            Token = token;

            var factory = new Func<ClientWebSocket>(() =>
            {
                var c = new ClientWebSocket();
                c.Options.SetRequestHeader("X-Gotify-Key", token);
                return c;
            });
            _client = new WebsocketClient(url, factory);
            
            _client.IsReconnectionEnabled = false;
            _client.ReconnectTimeout = TimeSpan.FromSeconds(30);

            if (onMessage != null) _client.MessageReceived.Subscribe(onMessage);
            if (onReconnected != null) _client.ReconnectionHappened.Subscribe(onReconnected);
            if (onDisconnected != null) _client.DisconnectionHappened.Subscribe(onDisconnected);
        }

        ~Listener()
        {
            if (_client.IsStarted)
            {
                _client.Stop(WebSocketCloseStatus.Empty, null);
            }
        }

        public void Start()
        {
            if (_client.IsStarted) return;
            _client.Start();
        }

        public void Stop()
        {
            if (!_client.IsStarted) return;
            _client.Stop(WebSocketCloseStatus.NormalClosure, null);
        }
        
        public override string ToString()
        {
            return $"Listener: {Token}";
        }
    }
}