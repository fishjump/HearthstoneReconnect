using HearthstoneReconntect.Model;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace HearthstoneReconntect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Model.AddLog(Application.Current.FindResource("WarningLogText").ToString());

            TryFindHearthstoneProcess();

            InitializeComponent();

            DataContext = Model;
            Timer = new Timer(ReconnectHearthstone, null, Timeout.Infinite, Timeout.Infinite);
        }

        private MainWindowModel Model { get; set; } = new MainWindowModel();
        
        private Timer Timer { get; set; }
        private const string cmd_path = @"C:\Windows\System32\cmd.exe";

        public static string RunCmd(string cmd)
        {
            cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            using (Process cmd_process = new Process())
            {
                cmd_process.StartInfo.FileName = cmd_path;
                cmd_process.StartInfo.UseShellExecute = false;
                cmd_process.StartInfo.RedirectStandardInput = true;
                cmd_process.StartInfo.RedirectStandardOutput = true;
                cmd_process.StartInfo.RedirectStandardError = true;
                cmd_process.StartInfo.CreateNoWindow = true;
                cmd_process.Start();

                cmd_process.StandardInput.WriteLine(cmd);
                cmd_process.StandardInput.AutoFlush = true;

                string output = cmd_process.StandardOutput.ReadToEnd();
                cmd_process.WaitForExit();
                cmd_process.Close();

                return output;
            }
        }

        private string FindHearthstoneProcess()
        {
            var processes = Process.GetProcessesByName("Hearthstone");
            if (processes.Length > 1)
            {
                throw new Exception(Application.Current.FindResource("TooManyHearthstoneProcessesExceptionText").ToString());
            }
            else if (processes.Length == 0)
            {
                throw new Exception(Application.Current.FindResource("NoHearthstoneProcessExceptionText").ToString());
            }
            else
            {
                var hs_process = processes[0];
                return hs_process.MainModule.FileName;
            }
        }

        private void TryFindHearthstoneProcess()
        {
            try
            {
                string hs_path = FindHearthstoneProcess();
                Model.AddLog(string.Format(Application.Current.FindResource("FindHearthstoneLogTemplate").ToString(), hs_path));
                Model.HearthstonePath = hs_path;

                AddFirewallRule();
            }
            catch (Exception exception)
            {
                Model.AddLog(string.Format(Application.Current.FindResource("CannotFindHearthstoneLogTemplate").ToString(), exception.Message));
            }
        }

        private void DisconnectHearthstone(object sender, RoutedEventArgs e)
        {
            RunCmd(Model.EnableFirewallRuleCmd);
            
            Model.IsReconnectButtonEnabled = false;

            Timer.Change(Model.DelayInMs, Timeout.Infinite);

            Model.AddLog(string.Format(Application.Current.FindResource("DisconnectLogTemplate").ToString(), Model.Delay));
        }

        private void ReconnectHearthstone(object state)
        {
            RunCmd(Model.DisableFirewallRuleCmd);
            Model.IsReconnectButtonEnabled = true;

            Dispatcher.Invoke(() => { Model.AddLog(string.Format(Application.Current.FindResource("ReconnectLogText").ToString(), Model.Delay)); });
        }

        private void AddFirewallRule()
        {
            RunCmd(Model.AddFirewallRuleCmd);
            Model.AddLog(Application.Current.FindResource("AddFirewallRuleText").ToString());
        }

        private void RemoveFirewallRule(object sender, EventArgs e)
        {
            RunCmd(Model.RemoveFirewallRuleCmd);

            Model.AddLog(Application.Current.FindResource("RemoveFirewallRuleText").ToString());
        }

        private void FindHearthstoneProcess(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(Model.HearthstonePath))
            {
                switch(MessageBox.Show(Application.Current.FindResource("EnsureRefindHearthstoneProcessMessageboxText").ToString(),
                    Application.Current.FindResource("WinTitleText").ToString(), MessageBoxButton.YesNo))
                {
                    case MessageBoxResult.Yes:
                        TryFindHearthstoneProcess();
                        break;
                }
            }
            else
            {
                TryFindHearthstoneProcess();
            }
        }

        private void ClearLog(object sender, RoutedEventArgs e)
        {
            Model.ClearLog();
        }
    }
}
