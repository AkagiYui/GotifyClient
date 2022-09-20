using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GotifyClient.Modules
{
    public class Utils
    {
        public static IntPtr GetHwnd(Window window) //获取窗口句柄
        {
            return new System.Windows.Interop.WindowInteropHelper(window).Handle;
        }
        
        public static object GetElementFromPoint(ItemsControl itemsControl, Point point)
        {
            UIElement element = itemsControl.InputHitTest(point) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                    return null;
                var item = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (!item.Equals(DependencyProperty.UnsetValue))
                    return item;
                element = (UIElement)VisualTreeHelper.GetParent(element);
            }
            return null;
        }
        
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
    
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
    }
}