using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.ObjectModel;

namespace HearthstoneReconntect.Model
{
    class MainWindowModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Log { get; set; } = new ObservableCollection<string>();
        public void AddLog(string log)
        {
            Log.Add(log);
        }
        public void ClearLog()
        {
            Log.Clear();
        }
        public uint Delay { get; set; } = 6;
        public uint DelayInMs 
        { 
            get
            {
                return Delay * 1000;
            } 
        }
        public bool IsReconnectButtonEnabled { get; set; } = true;
        public string HearthstonePath { get; set; } = string.Empty;
        public const string RuleName = "hearthstone_reconnect";

        public string EnableFirewallRuleCmd
        {
            get
            {
                return "netsh advfirewall firewall set rule name=\""+ RuleName + "\" new program=\"" + HearthstonePath + "\" enable=yes > nul";
            }
        }

        public string DisableFirewallRuleCmd 
        { 
            get 
            {
                return "netsh advfirewall firewall set rule name=\"" + RuleName + "\" new enable=no > nul"; 
            } 
        }

        public string AddFirewallRuleCmd 
        {
            get 
            { 
                return "netsh advfirewall firewall add rule name=\"" + RuleName + "\" dir=out program=\"" + HearthstonePath + "\" action=block enable=no > nul";
            } 
        }

        public string RemoveFirewallRuleCmd
        {
            get
            {
                return "netsh advfirewall firewall delete rule name=\"" + RuleName + "\" > nul";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
