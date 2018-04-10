using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using User;
using User.Windows;

namespace Edit_Community
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG 
            Process thisProc = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(thisProc.ProcessName);
            if (processes.Length > 1)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    if (processes[i] != thisProc)
                    {
                        IntPtr intPtr = processes[i].MainWindowHandle;
                        Current.Shutdown();
                        System.Threading.Thread.Sleep(100);
                        if (Win32API.IsZoomed(intPtr))
                        {
                            Win32API.ShowWindowAsync(intPtr, Win32WindowState.Maximize);
                        }
                        else
                        {
                            Win32API.ShowWindowAsync(intPtr, Win32WindowState.Normal);
                        }
                        Win32API.SetForegroundWindow(intPtr);
                        System.Threading.Thread.Sleep(100);
                        return;
                    }
                }
            }
#endif
            base.OnStartup(e);
        }
    }
}
