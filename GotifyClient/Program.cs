using System;
using GotifyClient.Modules;

namespace GotifyClient
{
    internal static class Program
    {
        private static readonly App App = new App();
    
        [STAThread]
        private static void Main(string[] args)
        {
            var debug = Array.IndexOf(args, "--debug") != -1;
            if (debug)
            {
                Utils.AllocConsole();
            }

            App.InitializeComponent();
            App.Run();

            if (debug)
            {
                Utils.FreeConsole();
            }
        }
    }
}