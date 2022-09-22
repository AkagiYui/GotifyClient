﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GotifyClient.Entities;
using GotifyClient.Modules;
using log4net;
using Websocket.Client.Models;

namespace GotifyClient.Windows
{
    
    public partial class MainWindow
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        private static readonly Connections Connections = Application.Current.Properties["connections"] as Connections;
        private Listener _listener;
        private Listener SelectedListener
        {
            get => _listener;
            set
            {
                if (_listener == value) return;
                _listener = value;
                if (_listener == null) return;
                NameTextBox.Text = _listener.Name;
                UrlTextBox.Text = $"{_listener.Host}:{_listener.Port}";
                TokenTextBox.Text = _listener.Token;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            ConnectionBox.ItemsSource = Connections.GetAllNames();
            Connections.CallbackOnMessage += OnMessage;
            Connections.CallbackOnReconnected += OnReconnected;
        }

        private void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(Owner);
            foreach(var listener in Connections.Listeners)
            {
                listener.Value.Start();
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
                LogTextBox.AppendText($"{Environment.NewLine}Reconnected: {msg.Type}");
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
                LogTextBox.AppendText(log);
            }));
        }

        private void ConnectionBox_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = Utils.GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender));
            if (item == null)
            {
                ConnectionBox.UnselectAll();
            }
        }
        
        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            var addListenerWindow = new EditListenerWindow()
            {
                Owner = this
            };
            addListenerWindow.ShowDialog();
            ConnectionBox.Items.Refresh();
        }

        private void MenuItemEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(ConnectionBox.SelectedItem is string listenerName)) return;
            var addListenerWindow = new EditListenerWindow(Connections.GetListener(listenerName))
            {
                Owner = this
            };
            addListenerWindow.ShowDialog();
            ConnectionBox.ItemsSource = Connections.GetAllNames();
        }

        private void MenuItemRemove_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(ConnectionBox.SelectedItem is string listenerName)) return;
            Connections.RemoveServer(listenerName);
            ConnectionBox.ItemsSource = Connections.GetAllNames();
        }

        private void ConnectionBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(Utils.GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender)) is string name))
            {
                return;
            }
            SelectedListener = Connections.GetListener(name);
            
        }
    }
}