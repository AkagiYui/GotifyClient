using System.Windows;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace GotifyClient.Modules
{
    public class NotifyIconViewModel
    {
        /// <summary>
        /// 如果窗口没显示，就显示窗口
        /// </summary>
        public static ICommand ToggleWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Application.Current.MainWindow != null,
                    CommandAction = () =>
                    {
                        var window = Application.Current.MainWindow;
                        if (window == null)
                        {
                            return;
                        }
                        switch (window.IsVisible)
                        {
                            case false:
                            {
                                window.Show();
                                var isTop = window.Topmost;
                                window.Topmost = true;
                                window.Topmost = isTop;
                                break;
                            }
                            case true:
                            {
                                window.Visibility = Visibility.Hidden;
                                break;
                            }
                        }
                    }
                };
            }
        }

        /// <summary>
        /// 关闭软件
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () =>
                {
                    var msg = new TaskDialog()
                    {
                        Icon = TaskDialogStandardIcon.Warning,
                        Caption = "GotifyClient",
                        DefaultButton = TaskDialogDefaultButton.Yes,
                        StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No,
                        Text = "Sure to exit?"
                    };
                    
                    if (msg.Show() == TaskDialogResult.No) return;
                    var t = (TaskbarIcon)Application.Current.Resources["Taskbar"];
                    t.Visibility = Visibility.Collapsed;
                    t.Dispose();
                    Application.Current.Shutdown();
                } };
            }
        }
    }
}