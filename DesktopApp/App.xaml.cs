using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

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
                var firstArg = args[0];
                if (firstArg.Equals("/TRAY"))
                {
                    var subArray = new string[args.Length - 1];
                    Array.Copy(args, 1, subArray, 0, subArray.Length);
                    var (success, message) = CommandLineHelper.ProcessCommand(subArray);
                    var notifyIcon = new NotifyIcon()
                    {
                        Text = "QuickChecksum",
                        BalloonTipTitle = "QuickChecksum",
                        BalloonTipIcon = success ? ToolTipIcon.Info : ToolTipIcon.Error,
                        BalloonTipText = message,
                        Icon = Icon.FromHandle(new Bitmap(16, 16, PixelFormat.Format16bppRgb555).GetHicon()),
                        Visible = true
                    };

                    notifyIcon.ShowBalloonTip(5_000);
                    
                    Thread.Sleep(10_000);
                    notifyIcon.Visible = false;
                    
                    Environment.Exit(0);
                    
                    return;
                }

                StartupArgFile = firstArg;
            }
            
            new MainWindow().Show();
        }
    }
}
