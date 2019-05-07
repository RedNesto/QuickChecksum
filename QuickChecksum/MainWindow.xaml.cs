using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Clipboard = System.Windows.Forms.Clipboard;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;

namespace QuickChecksum
{
    public partial class MainWindow
    {
        public ChecksumAlgorithm SelectedAlgorithm => (ChecksumAlgorithm) AlgorithmCombobox.SelectedItem;

        private bool _isComputingInProgress;

        public bool IsComputingInProgress
        {
            get => _isComputingInProgress;
            set
            {
                _isComputingInProgress = value;

                SelectFilePathButton.IsEnabled = !value;
                FilePathTextBox.IsEnabled = !value;
                AlgorithmCombobox.IsEnabled = !value;
                ExpectedHashTextBox.IsEnabled = !value;
                ComputedHashTextBox.IsEnabled = !value;
                RefreshCheckButtonState();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RefreshCheckButtonState()
        {
            if (IsComputingInProgress || string.IsNullOrWhiteSpace(FilePathTextBox.Text))
            {
                CheckButton.IsEnabled = false;
                return;
            }

            var expectedHash = ExpectedHashTextBox.Text.Trim();
            CheckButton.IsEnabled = expectedHash.Length == ChecksumChecker.GetHashStringSize(SelectedAlgorithm);
        }

        private async Task<string> ComputeFileHashStringAsync(string filePath, ChecksumAlgorithm algorithm)
        {
            IsComputingInProgress = true;
            ShowStatus("Computing Hash...", Colors.Black);
            var computedHash = await Task.Run(() =>
            {
                var hash = ChecksumChecker.ComputeFileHash(filePath, algorithm);
                return hash != null ? HexConvert.ToString(hash) : string.Empty;
            });
            if (string.IsNullOrEmpty(computedHash))
            {
                ShowStatus("Unable to compute hash", Colors.Red, 20_000);
            }
            else
            { 
                HideStatus();
            }
            
            IsComputingInProgress = false;

            return computedHash;
        }

        private void ShowStatus(string statusText, Color textColor)
        {
            StatusLabel.Content = statusText;
            StatusLabel.Foreground = new SolidColorBrush(textColor);
        }

        private async void ShowStatus(string statusText, Color textColor, int showMillis)
        {
            ShowStatus(statusText, textColor);
            await Task.Delay(showMillis);
            HideStatus();
        }

        private void HideStatus()
        {
            StatusLabel.Content = string.Empty;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            FilePathTextBox.TextChanged += async (eSender, args) =>
            {
                RefreshCheckButtonState();
                if (!AutoComputeHashCheckbox.IsChecked.GetValueOrDefault(true))
                    return;

                var filePath = FilePathTextBox.Text;
                if (File.Exists(filePath))
                    ComputedHashTextBox.Text = await ComputeFileHashStringAsync(filePath, SelectedAlgorithm);
            };
            ExpectedHashTextBox.TextChanged += (eSender, args) => RefreshCheckButtonState();
            AlgorithmCombobox.SelectionChanged += (eSender, args) => RefreshCheckButtonState();
            ComputedHashTextBox.TextChanged += (eSender, args) => CopyComputedHashButton.IsEnabled = !string.IsNullOrEmpty(ComputedHashTextBox.Text);

            FilePathTextBox.Text = App.StartupArgFile;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AlgorithmCombobox.ItemsSource = Enum.GetValues(typeof(ChecksumAlgorithm)).Cast<ChecksumAlgorithm>();
            Window.MinWidth = Window.ActualWidth;
            Window.MinHeight = Window.ActualHeight;
        }

        private void SelectFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                FilePathTextBox.Text = openFileDialog.FileName ?? string.Empty;
        }

        private void FilePathTextBox_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (data == null || data.Length < 1)
                return;

            FilePathTextBox.Text = data[0];
        }

        private void FilePathTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
                e.Handled = true;
            }
        }

        private void HashTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
                e.Handled = true;
            }
        }

        private async void ExpectedHashTextBox_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (data == null || data.Length < 1)
                return;

            ExpectedHashTextBox.Text = await ComputeFileHashStringAsync(data[0], SelectedAlgorithm);
        }

        private async void ComputedHashTextBox_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (data == null || data.Length < 1)
                return;

            ComputedHashTextBox.Text = await ComputeFileHashStringAsync(data[0], SelectedAlgorithm);
        }

        private async void CopyComputedHashButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ComputedHashTextBox.Text);
            CopyComputedHashButton.Content = "Copied";
            await Task.Delay(2000);
            CopyComputedHashButton.Content = "Copy";
        }

        private async void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            var filePath = FilePathTextBox.Text;
            var expectedHash = ExpectedHashTextBox.Text.Trim();
            var algorithm = SelectedAlgorithm;

            ShowStatus("Checking file hash...", Colors.Black);
            IsComputingInProgress = true;
            try
            {
                var (hashMatches, _) = await Task.Run(() => ChecksumChecker.CheckFile(filePath, expectedHash, algorithm));
                var (statusText, statusColor) = hashMatches ? ("File hash matches", Colors.Green) : ("File hash does not match", Colors.Red);
                ShowStatus(statusText, statusColor, 25_000);
            }
            catch (Exception ex)
            {
                var statusText = "An error occurred when checking file hash";
                statusText += !string.IsNullOrWhiteSpace(ex.Message) ? ": \n" + ex.Message : ".";
                ShowStatus(statusText, Colors.Red);
            }
            finally
            {
                IsComputingInProgress = false;
            }
        }
    }
}
