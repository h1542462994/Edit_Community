using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Edit_CommunityUpdater
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public SoftWareService service;
        public string path = Environment.CurrentDirectory + @"\";
        public MainWindow()
        {
            InitializeComponent();
            service = new SoftWareService(new Version(), "Edit_Community") { Root = path };
            service.UpdateCompleted += Service_UpdateCompleted;

            TbxDescription.Text = path + @"SoftWareCache\";
            Console.WriteLine(Environment.CurrentDirectory);
        }
        private void Service_UpdateCompleted(object sender, bool e)
        {
            Process.Start(path + "Edit Community.exe");
            Close();
        }
        private void TriggerButtonUpdate_Tapped(object sender, RoutedEventArgs e)
        {
            if (CheckUpdate())
            {
                ApplyUpdate();
                TbxDescription.Text = "";
            }
            else
            {
                TriggerButtonUpdate.IsOpened = true;
                TriggerButtonUpdate.InnerContent = "重试";
                TbxDescription.Text = "找不到Edit_Community.exe";
            }
        }
        private void TriggerButtonExit_Tapped(object sender, RoutedEventArgs e)
        {
            Close();
        }
        bool CheckUpdate()
        {
            if (File.Exists(path + "Edit Community.exe"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        async void ApplyUpdate()
        {
            TriggerButtonUpdate.IsOpened = false;
            TriggerButtonUpdate.InnerContent = "应用中";
            await Task.Run(() => service.ApplyUpdate() ); 
        }
        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
