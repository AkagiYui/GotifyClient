using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GotifyClient.Entities;
using GotifyClient.Modules;
using log4net;
using log4net.Repository.Hierarchy;
using Websocket.Client.Models;

namespace GotifyClient.Windows
{
    
    public partial class MainWindow
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        private static readonly Connections Connections = Application.Current.Properties["connections"] as Connections;
        public MainWindow()
        {
            InitializeComponent();
            ConnectionBox.ItemsSource = Connections.Listeners;
            Connections.CallbackOnMessage += OnMessage;
            Connections.CallbackOnReconnected += OnReconnected;
        }

        private void ConnectButton_OnClick(object sender, RoutedEventArgs e)
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
            object item = GetElementFromPoint((ItemsControl)sender, e.GetPosition((ItemsControl)sender));
            Logger.Info(item != null);
        }
        
        private object GetElementFromPoint(ItemsControl itemsControl, Point point)
        {
            UIElement element = itemsControl.InputHitTest(point) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                    return null;
                object item = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (!item.Equals(DependencyProperty.UnsetValue))
                    return item;
                element = (UIElement)VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            var addListenerWindow = new AddListenerWindow()
            {
                Owner = this
            };
            addListenerWindow.ShowDialog();
        }
    }
}