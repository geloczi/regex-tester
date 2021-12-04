using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using RegexTester.ViewModels;

namespace RegexTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _vm;
        private readonly string _patternFilePath = Path.Combine(App.AppDataFolder, "pattern.txt");
        private readonly string _inputFilePath = Path.Combine(App.AppDataFolder, "input.txt");
        private string _originalString = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            Closing += MainWindow_Closing;
            PreviewKeyDown += MainWindow_PreviewKeyDown;



            cbRunOnChanged.IsChecked = App.Config.RunOnChanged;

            DataContext = _vm = new MainWindowViewModel();
            _vm.PropertyChanged += _vm_PropertyChanged;

            if (File.Exists(_patternFilePath))
            {
                if (Keyboard.IsKeyDown(Key.LeftShift))
                    File.Delete(_patternFilePath);
                else
                    _vm.Pattern = File.ReadAllText(_patternFilePath);
            }

            if (File.Exists(_inputFilePath))
            {
                if (Keyboard.IsKeyDown(Key.LeftShift))
                    File.Delete(_inputFilePath);
                else
                    rtb.Document = StringToFlowDocument(File.ReadAllText(_inputFilePath));
            }
        }

        private void _vm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (cbRunOnChanged.IsChecked == true)
            {
                Run();
            }
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.F5:
                        e.Handled = true;
                        Run();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                App.Config.RunOnChanged = cbRunOnChanged.IsChecked == true;
                File.WriteAllText(_inputFilePath, FlowDocumentToString(rtb.Document));
                File.WriteAllText(_patternFilePath, _vm.Pattern);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            Run();
        }

        private void Run()
        {
            try
            {
                _originalString = FlowDocumentToString(rtb.Document);
                var matches = _vm.GetMatches(_originalString, TimeSpan.FromMinutes(1));
                rtb.Document = TextMatchesToFlowDocument(_originalString, matches);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                Reset();
            }
        }

        private void Reset()
        {
            try
            {
                rtb.Document = StringToFlowDocument(_originalString);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private static FlowDocument TextMatchesToFlowDocument(string text, MatchCollection? matches)
        {
            if (matches?.Count > 0 != true)
                return StringToFlowDocument(text);

            var doc = new FlowDocument();
            var paragraph = new Paragraph();
            int lastIndex = 0;
            int lastLength = 0;

            foreach (Match match in matches)
            {
                if (match.Index < lastIndex)
                    throw new Exception("Matches must be ordered by index.");

                // Add all preceding text unformatted
                if (match.Index > 0)
                    paragraph.Inlines.Add(new Run(text.Substring(lastIndex + lastLength, match.Index - (lastIndex + lastLength))));

                // Highlight match
                if (match.Value != "\r")
                    paragraph.Inlines.Add(new Run(text.Substring(match.Index, match.Length)) { Background = Brushes.Cyan });

                lastIndex = match.Index;
                lastLength = match.Length;
            }

            // Add remaining text unformatted
            if (lastIndex + lastLength < text.Length)
                paragraph.Inlines.Add(new Run(text.Substring(lastIndex + lastLength, text.Length - lastIndex - lastLength)));

            doc.Blocks.Add(paragraph);
            return doc;
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(this, ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static FlowDocument StringToFlowDocument(string s)
        {
            return new FlowDocument(new Paragraph(new Run(s)));
        }

        private static string FlowDocumentToString(FlowDocument doc)
        {
            string text = new TextRange(doc.ContentStart, doc.ContentEnd).Text;
            if (text.EndsWith(Environment.NewLine))
                text = text.Substring(0, text.Length - Environment.NewLine.Length);
            return text;
        }
    }
}
