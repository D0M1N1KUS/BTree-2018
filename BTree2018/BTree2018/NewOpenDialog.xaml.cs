using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Logging;
using BTree2018.UtilityClasses;
using Path = System.IO.Path;

namespace BTree2018
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NewOpenDialog : Window
    {
        private readonly Brush ERROR_COLOR_BRUSH = Brushes.Red;
        private readonly Brush NORMAL_COLOR_BRUSH;

        private bool PageFileSet = false;
        private bool RecordFileSet = false;
        private bool PageMapFileSet = false;
        private bool RecordMapFileSet = false;
        
        private FileChooser FileChooser = new FileChooser();

        public IBTree<int> BTree { get; private set; } = null;


        public NewOpenDialog()
        {
            InitializeComponent();
            NORMAL_COLOR_BRUSH = DTextBox.BorderBrush;
        }

        private void openFileDialog(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if(button == null) throw new Exception("openFileDialog is not suitable to be used with " + sender);
                
                var filePath = FileChooser.ChooseFileDialog();
                if (filePath == string.Empty)
                {
                    LogTextBlock.Text = "No file has been choosen!";
                    return;
                }

                if (button.Name == "SelectPagefileButton")
                {
                    PageFileSelectionTextBox.Text = filePath;
                    PageFileSet = true;
                }
                else if (button.Name == "SelectRecordfileButton")
                {
                    RecordFilePathTextBox.Text = filePath;
                    RecordFileSet = true;
                }
                else if (button.Name == "SelectPageMapFileButton")
                {
                    PageMapFilePathTextBox.Text = filePath;
                    PageMapFileSet = true;
                }
                else if (button.Name == "SelectRecordMapFileButton")
                {
                    RecordMapFilePathTextBox.Text = filePath;
                    RecordMapFileSet = true;
                }
                else
                    throw new Exception("Unknown button " + button.Name);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                LogTextBlock.Text = ex.Message;
            }
            
            enableButons();
        }

        private void enableButons()
        {
            OpenButton.IsEnabled = PageFileSet && PageMapFileSet && RecordFileSet && RecordMapFileSet &&
                                   
                                   PageFileSelectionTextBox.Text.IndexOfAny(Path.GetInvalidPathChars()) >= 0 &&
                                   RecordFilePathTextBox.Text.IndexOfAny(Path.GetInvalidPathChars()) >= 0 &&
                                   PageMapFilePathTextBox.Text.IndexOfAny(Path.GetInvalidPathChars()) >= 0 &&
                                   RecordMapFilePathTextBox.Text.IndexOfAny(Path.GetInvalidPathChars()) >= 0;
            NewButton.IsEnabled = OpenButton.IsEnabled && InputValidation.TryParse<long>(DTextBox.Text);
        }

        private void checkIfNumber(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(textBox == null) throw new Exception("checkIfNumber cant be used with" + sender);
            textBox.BorderBrush = InputValidation.TryParse<long>(textBox.Text)
                ?  NORMAL_COLOR_BRUSH
                : ERROR_COLOR_BRUSH;
        }

        private void createNewBTree(object sender, RoutedEventArgs e)
        {
            try
            {
                BTree = BTreeBuilder<int>.New(sizeof(int),
                    int.Parse(DTextBox.Text),
                    PageFileSelectionTextBox.Text, RecordFilePathTextBox.Text,
                    PageMapFilePathTextBox.Text, RecordMapFilePathTextBox.Text);
                Close();

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                LogTextBlock.Text = ex.Message;
            }
        }

        private void openBTree(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!checkFilePaths()) return;
                BTree = BTreeBuilder<int>.Open(sizeof(int),
                    PageFileSelectionTextBox.Text, RecordFilePathTextBox.Text,
                    PageMapFilePathTextBox.Text, RecordMapFilePathTextBox.Text);
                Close();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                LogTextBlock.Text = ex.Message;
            }
        }

        private bool checkFilePaths()
        {
            var messageBuilder = new StringBuilder();
            if (!File.Exists(PageFileSelectionTextBox.Text))
                messageBuilder.Append("The file \"" + PageFileSelectionTextBox.Text + "\" could not be found!\n");
            if(!File.Exists(PageMapFilePathTextBox.Text))
                messageBuilder.Append("The file \"" + PageMapFilePathTextBox.Text + "\" could not be found!\n");
            if(!File.Exists(RecordFilePathTextBox.Text))
                messageBuilder.Append("The file \"" + RecordFilePathTextBox.Text + "\" could not be found!\n");
            if(!File.Exists(RecordMapFilePathTextBox.Text))
                messageBuilder.Append("The file \"" + RecordMapFilePathTextBox.Text + "\" could not be found!\n");
            if (messageBuilder.Length <= 0) return true;
            var message = messageBuilder.ToString();
            LogTextBlock.Text = message;
            Logger.Log(message);
            return false;

        }

        private void cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
