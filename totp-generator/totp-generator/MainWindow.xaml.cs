using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace totp_generator
{
    public partial class MainWindow : Window
    {
        private readonly TOTP _totp = new TOTP();
        private MyXML _xmlManager;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            var myTimer = new Timer();
            myTimer.Elapsed += new ElapsedEventHandler(MyEvent);
            myTimer.Interval = 1000;
            myTimer.Enabled = true;
            _xmlManager = new MyXML();
            _xmlManager.Refresh(ListBoxAccount);

            // to select the first
            if (ListBoxAccount.SelectedItem != null)
            {
                Account item = (Account)ListBoxAccount.SelectedItem;
                textKey.Text = item.Key;
            }
        }

        private void MyEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(RefreshFrame);
            }
            catch (TaskCanceledException err) { }
        }

        private void RefreshFrame()
        {
            labelCopied.Visibility = Visibility.Collapsed;
            String code = textKey.Text;
            if (code.Length < 3)
            {
                labelCode.Content = "Please enter a key";
                imgClipboard.Visibility = Visibility.Collapsed;
                labelTimeLeft.Content = "( ͡° ͜ʖ ͡°)";
            }
            else
            {
                _totp.Key = code;
                _totp.Refresh();
                labelCode.Content = _totp.Code;
                imgClipboard.Visibility = Visibility.Visible;
                labelTimeLeft.Content = "Code expires in " + _totp.TimeLeft + " second";
                if (_totp.TimeLeft != "1")
                    labelTimeLeft.Content += "s";
            }
        }

        private void topDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void closeApp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SaveCurrent(object sender, RoutedEventArgs e)
        {
            if (textKey.Text.Length < 3)
                MessageBox.Show("Key seem to be incorrect, fix that.", "Error adding key");
            else
            {
                String name = MyDialog.Prompt("Adding account", "Enter your desired name for this key :",
                    MyDialog.Size.Big);
                if (name.Length < 2)
                {
                    MessageBox.Show("Name must be 2 characters minimum", "Error adding key");
                    return;
                }
                _xmlManager.AddElem(name, textKey.Text);
                _xmlManager.Refresh(ListBoxAccount);
            }
        }

        private void DeleteSelected(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.No)
                return;
            if (ListBoxAccount.SelectedItem != null)
            {
                Account item = (Account)ListBoxAccount.SelectedItem;
                _xmlManager.DeleteElem(item.Name, item.Key);
                _xmlManager.Refresh(ListBoxAccount);
            }
        }

        private void Selected(object sender, RoutedEventArgs e)
        {
            if (ListBoxAccount.SelectedItem != null)
            {
                Account item = (Account)ListBoxAccount.SelectedItem;
                textKey.Text = item.Key;
            }
        }

        private void CopyToClipboard(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(_totp.Code);
            labelCopied.Visibility = Visibility.Visible;
        }
    }
}
