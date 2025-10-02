using Hardcodet.Wpf.TaskbarNotification;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace NGINXCP
{
   
    public partial class MainWindow : Window
    {
        private TaskbarIcon notifyIcon; 
        private Window mainWindow;
        internal static readonly string base_path = @$"{AppDomain.CurrentDomain.BaseDirectory}\PATH"; 
        
        internal static string path; 
        public MainWindow()
        {
            InitializeComponent();


            try
            {
                path = File.ReadAllText(base_path);
                if (path=="" || path==" ")
                {
                    MessageBox.Show($"Add nginx path to {base_path}", "Error");
                    System.Windows.Application.Current.Shutdown();

                }
            }
            catch (Exception e) {
                MessageBox.Show($"Add nginx path to {base_path}", "Error");
                System.Windows.Application.Current.Shutdown();
            }


            notifyIcon = new TaskbarIcon
            {
                Icon = LoadIconFromResource("icon.ico"),  
                ToolTipText = "NGINXSCP", 
                Visibility = Visibility.Visible
            };

            var contextMenu = new ContextMenu();
            var openItem = new MenuItem { Header = "Show" };
            openItem.Click += (s, args) => ShowMainWindow();
            var exitItem = new MenuItem { Header = "Exit" };
            exitItem.Click += (s, args) => System.Windows.Application.Current.Shutdown();
            var hideItem = new MenuItem { Header = "Hide" };
            hideItem.Click += (s, args) => this.Hide() ;
            contextMenu.Items.Add(openItem);
            contextMenu.Items.Add(hideItem);
            contextMenu.Items.Add(exitItem);
            notifyIcon.ContextMenu = contextMenu;  // Привязываем меню

            notifyIcon.TrayMouseDoubleClick += (s, args) => ShowMainWindow();
            notifyIcon.TrayLeftMouseUp += (s, args) => ShowMainWindow();
            

        }
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void ShowMainWindow()
        {
            this.Show();
            
        }

        protected void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose();

            this.Hide();
        }
        private Icon LoadIconFromResource(string iconName)
        {
            try
            {
                Uri uri = new Uri($"pack://application:,,,/{iconName}");
                StreamResourceInfo resourceInfo = System.Windows.Application.GetResourceStream(uri);
                if (resourceInfo != null)
                {
                    using (Stream stream = resourceInfo.Stream)
                    {
                        return new System.Drawing.Icon(stream);
                    }
                }
                Trace.WriteLine("404");
                return System.Drawing.SystemIcons.Application;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"err");
                return System.Drawing.SystemIcons.Application;
            }
        }

        private void StartNginx(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = path,
                WorkingDirectory = System.IO.Path.GetDirectoryName(path),
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(psi);
        }

        private void StopNginx(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = path,
                Arguments = "-s stop",
                WorkingDirectory = System.IO.Path.GetDirectoryName(path),
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(psi);

        }

        private void ReloadNginx(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = path,
                Arguments = "-s reload",
                WorkingDirectory = System.IO.Path.GetDirectoryName(path),
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(psi);

        }
    }
}