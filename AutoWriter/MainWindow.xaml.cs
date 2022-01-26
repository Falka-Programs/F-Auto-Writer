using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        int timeout;
        public int Timeout {
            get {
                return timeout;
            }
            set
            {
                if (value < 0 || value > 3000)
                {
                    MessageBox.Show("Wrong value.\r\n Value must be beetwen 5 and 3000","Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    timeout = value;
                    OnProperyChanged("Timeout");
                }
            }
        }

        Process[] Processes { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Timeout = 30;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string pName = ProccesTextBox.Text;
            Processes = Process.GetProcessesByName(pName);

            if(Processes.Length == 0)
            {
                processCountLabel.Content = "NF";
                processCountLabel.Foreground = Brushes.Red;
            }
            else
            {
                processCountLabel.Content = Processes.Length;
                processCountLabel.Foreground = Brushes.LightGreen;
            }

            //MessageSender ms = new MessageSender();
            //Thread.Sleep(1000);
            //try
            //{
            //    ms.SendMessage("Hello", processes);
            //    MessageBox.Show("Sended!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //catch (ArgumentException)
            //{
            //    MessageBox.Show($"Procces is invalid!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Unknow error!\r\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}

            //Process[] processes = Process.GetProcessesByName("firefox");
            //MessageBox.Show($"Find {processes.Length} process");

            //foreach (Process proc in processes)
            //    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, VK_F5, 0);
        }


        //Property changed implement
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnProperyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void FileSelectButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
