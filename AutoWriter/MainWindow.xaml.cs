using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace AutoWriter
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageSender ms = new MessageSender();
            Process[] processes = Process.GetProcessesByName("firefox");
            try
            {
                ms.SendMessage("Hello", processes);
            }catch(ArgumentException)
            {
                MessageBox.Show($"Procces is invalid!","Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
            }catch(Exception ex)
            {
                MessageBox.Show($"Unknow error!\r\n{ex.Message}","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            //Process[] processes = Process.GetProcessesByName("firefox");
            //MessageBox.Show($"Find {processes.Length} process");

            //foreach (Process proc in processes)
            //    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, VK_F5, 0);
        }
    }
}
