using System.Windows;

namespace QuickChecksum.DesktopApp
{
    public partial class App
    {
        public static string StartupArgFile { get; private set; } = string.Empty;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var args = e.Args;
            if (args.Length > 0)
            {
                StartupArgFile = args[0];
            }
        }
    }
}
