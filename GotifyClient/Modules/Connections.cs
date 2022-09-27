using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using GotifyClient.Entities;
using Hardcodet.Wpf.TaskbarNotification;
using log4net;
using Newtonsoft.Json;
using Websocket.Client;
using Websocket.Client.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GotifyClient.Modules
{
    public class Connections
    {
        private readonly TaskbarIcon _taskbar = (TaskbarIcon)Application.Current.Resources["Taskbar"];
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public readonly Dictionary<string, Client> Clients = new Dictionary<string, Client>();
        
        private const string ConfigPath = "config.yaml";
        private ConfigEntity _config;
        
        public Action<GotifyMessageEntity> CallbackOnMessage { get; set; }
        public Action<ReconnectionInfo> CallbackOnReconnected { get; set; }
        public Action<DisconnectionInfo> CallbackOnDisconnected { get; set; }

        public Connections()
        {
            LoadConfig();
            foreach (var server in _config.Servers)
            {
                if (Clients.ContainsKey(server.Name)) continue;
                var client = new Client(server, OnMessage, OnReconnected, OnDisconnected);
                Clients.Add(server.Name, client);
            }
        }

        public IEnumerable<string> GetAllNames()
        {
            return Clients.Keys;
        }
        
        public Client GetClient(string name)
        {
            return Clients.TryGetValue(name, out var client) ? client : null;
        }
        
        private void OnReconnected(ReconnectionInfo msg)
        {
            Logger.Info($"{Environment.NewLine}Reconnected: {msg.Type}");
            CallbackOnReconnected?.Invoke(msg);
        }
        
        private void OnDisconnected(DisconnectionInfo msg)
        {
            Logger.Info($"{Environment.NewLine}Disconnected: {msg.Type}");
            CallbackOnDisconnected?.Invoke(msg);
        }
        
        private void OnMessage(ResponseMessage msg)
        {
            var t = JsonConvert.DeserializeObject<GotifyMessageEntity>(msg.Text);
            if (t == null)
            {
                return;
            }

            var log = $"{Environment.NewLine}AppId: {t.Appid}";
            log += $"{Environment.NewLine}MessageId: {t.Id}";
            log += $"{Environment.NewLine}Date: {t.Date}";
            log += $"{Environment.NewLine}Priority: {t.Priority}";
            log += $"{Environment.NewLine}Title: {t.Title}";
            log += $"{Environment.NewLine}Message: {t.Message}";
            log += Environment.NewLine;
            Logger.Info(log);
            // var iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/icon.ico"))?.Stream;
            // var icon = new Icon(iconStream);
            _taskbar.ShowBalloonTip(t.Title, t.Message, BalloonIcon.Info);
            CallbackOnMessage?.Invoke(t);
        }

        private void LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                File.Create(ConfigPath).Close();
            }
            using (TextReader reader = File.OpenText(ConfigPath))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance) // 应用命名规范
                    .IgnoreUnmatchedProperties() // 忽略实体类中没有的字段
                    .Build();
                _config = deserializer.Deserialize<ConfigEntity>(reader) ?? new ConfigEntity();
            }
        }

        private void SaveConfig()
        {
            using (TextWriter writer = File.CreateText(ConfigPath))
            {
                _config.Servers = new List<ServerEntity>();
                foreach (var listener in Clients)
                {
                    var value = listener.Value;
                    var server = new ServerEntity
                    {
                        Name = value.Name,
                        Host = value.Host,
                        Port = value.Port,
                        Token = value.Token
                    };
                    _config.Servers.Add(server);
                }
                
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance) // 应用命名规范
                    .Build();
                serializer.Serialize(writer, _config);
            }
        }
        
        public void AddServer(ServerEntity server, ServerEntity oriServer = null)
        {
            if (string.IsNullOrEmpty(server.Name) || Clients.ContainsKey(server.Name))
            {
                return;
            }

            if (!(oriServer is null))
            {
                var listener = Clients[oriServer.Name];
                if (listener.Name != server.Name)
                {
                    listener.Name = server.Name;
                    Clients.Add(server.Name, listener);
                    Clients.Remove(oriServer.Name);
                }
                listener.Host = server.Host;
                listener.Port = server.Port;
                listener.Token = server.Token;
            }
            else
            {
                var client = new Client(server, OnMessage, OnReconnected, OnDisconnected);
                Clients.Add(server.Name, client);
            }
            SaveConfig();
        }
        
        public void RemoveServer(string name)
        {
            if (string.IsNullOrEmpty(name) || !Clients.ContainsKey(name))
            {
                return;
            }
            Clients[name].StopListener();
            Clients.Remove(name);
            SaveConfig();
        }
    }
}