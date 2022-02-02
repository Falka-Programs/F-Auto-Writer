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
using System.IO;
using System.Windows.Threading;
namespace AutoWriter.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static readonly float version = 0.2f;
        MessageSender ms;
        Process[] Processes { get; set; }
        List<string> messages;

        int timeout;
        public int Timeout {
            get {
                return timeout;
            }
            set
            {
                if (value <= 0 || value >= 3000)
                {
                    MessageBox.Show("Wrong value.\r\n Value must be beetwen 0 and 3000","Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    timeout = value;
                    OnProperyChanged("Timeout");
                }
            }
        }

        

        public MainWindow()
        {
            InitializeComponent();
            ms = new MessageSender();
            messages = new List<string>();
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
        }


        //Property changed implement
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnProperyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private List<string> SyncFileRead(FileStream fs)
        {
            //List<string> messages = new List<string>();
            List<byte> bytes = new List<byte>();
            for(int i= 0; i < fs.Length; i++)
            {
                bytes.Add((byte)fs.ReadByte());
            }
            string tmp = Encoding.UTF8.GetString(bytes.ToArray());
            PhrasesTextBox.Dispatcher.Invoke(DispatcherPriority.Normal,
            new Action(() => { PhrasesTextBox.Text = tmp; }));

            return tmp.Split('\n').ToList();
        }

        private async void FileSelectButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog of = new Microsoft.Win32.OpenFileDialog()
            {
                FileName = "Select a text file",
                Filter = "Text files (*.txt)|*.txt",
                Title = "Open text file"
            };
            if (of.ShowDialog() == true)
            {
                string filePath = of.FileName;
                try
                {
                    FileStream file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    messages = await Task.Run(() => SyncFileRead(file));
                }
                catch(Exception err) { 
                    MessageBox.Show($"Opening error\r\n{err.Message}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public void WaitThread()
        {
            try
            {
                for (int i = 5; i >= 0; i--)
                {
                    StatusLabel.Dispatcher.Invoke(() => { StatusLabel.Content = i; StatusLabel.Foreground = Brushes.Yellow; });
                    Thread.Sleep(1000);
                }
            }catch(Exception)
            {
                StatusLabel.Dispatcher.Invoke(() => { StatusLabel.Content = "CT"; StatusLabel.Foreground = Brushes.Magenta; });
                return;
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if(messages == null || messages.Count==0)
            {
                MessageBox.Show("Wrong messages file", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (Processes == null ||Processes.Length == 0)
            {
                MessageBox.Show("No processes detected!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                await Task.Run(() => WaitThread());
                try
                {
                    ms.SendMessages(messages, Timeout, Processes);
                    StatusLabel.Content = "ON";
                    StatusLabel.Foreground = Brushes.LightGreen;
                    StartButton.IsEnabled = false;
                    StopButton.IsEnabled = true;
                }catch(Exception err)
                {
                    MessageBox.Show($"Error\r\n{err.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            ms.StopSending();
            StatusLabel.Content = "OFF";
            StatusLabel.Foreground = Brushes.Red;
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdatesCheckMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("In development...","Information",MessageBoxButton.OK,MessageBoxImage.Information);
        }
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aw = new AboutWindow(version,MessageSender.version);
            aw.Owner = this;
            aw.ShowDialog();
        }
    }
}
