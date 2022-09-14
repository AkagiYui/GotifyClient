using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace GotifyClient.Modules
{
    public class Utils
    {
        public static IntPtr GetHwnd(Window window) //获取窗口句柄
        {
            return new System.Windows.Interop.WindowInteropHelper(window).Handle;
        }
        
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
    
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
    }
}