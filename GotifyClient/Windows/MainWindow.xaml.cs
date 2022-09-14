using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using GotifyClient.Entities;
using GotifyClient.Modules;
using Websocket.Client.Models;

namespace GotifyClient.Windows
{
    public partial class MainWindow
    {
        private static readonly Connections Connections = Application.Current.Properties["connections"] as Connections;
        public MainWindow()
        {
            InitializeComponent();
            ConnectionBox.ItemsSource = Connections.Listeners;
            Connections.CallbackOnMessage += OnMessage;
            Connections.CallbackOnReconnected += OnReconnected;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(Owner);
            foreach(var listener in Connections.Listeners)
            {
                listener.Start();
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
        
        private void OnReconnected(ReconnectionInfo msg)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Log.AppendText($"{Environment.NewLine}Reconnected: {msg.Type}");
            }));
        }
        
        private void OnMessage(GotifyMessageEntity t)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var log = $"{Environment.NewLine}AppId: {t.Appid}";
                log += $"{Environment.NewLine}MessageId: {t.Id}";
                log += $"{Environment.NewLine}Date: {t.Date}";
                log += $"{Environment.NewLine}Priority: {t.Priority}";
                log += $"{Environment.NewLine}Title: {t.Title}";
                log += $"{Environment.NewLine}Message: {t.Message}";
                log += Environment.NewLine;
                Log.AppendText(log);
            }));
        }
    }
}