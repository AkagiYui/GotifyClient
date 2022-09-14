﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using GotifyClient.Entities;
using Hardcodet.Wpf.TaskbarNotification;
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
        public readonly List<Listener> Listeners = new List<Listener>();
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
                var listener = new Listener($"{server.Host}:{server.Port}", server.Token, OnMessage, OnReconnected);
                Listeners.Add(listener);
            }
        }
        
        private void OnReconnected(ReconnectionInfo msg)
        {
            Trace.WriteLine($"{Environment.NewLine}Reconnected: {msg.Type}");
            CallbackOnReconnected?.Invoke(msg);
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
            Trace.Write(log);
            _taskbar.ShowBalloonTip(t.Title, t.Message, BalloonIcon.None);
            
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
    }
    
    
}