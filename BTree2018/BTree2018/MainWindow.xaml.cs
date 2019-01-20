using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BTree2018.BTreeStructure;
using BTree2018.Interfaces;
using BTree2018.Interfaces.BTreeStructure;
using BTree2018.Logging;
using BTree2018.UtilityClasses;

namespace BTree2018
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int LOG_BUFFER_SIZE = 3000;
        private readonly Brush ERROR_COLOR_BRUSH = Brushes.Red;
        private readonly Brush NORMAL_COLOR_BRUSH;

        private IBTree<int> BTree;

        private FileChooser FileChooser;
        
        public MainWindow()
        {
            InitializeComponent();
            NORMAL_COLOR_BRUSH = Value1TextBox.BorderBrush;
        }

        private void DisplayLog(object sender, RoutedEventArgs e)
        {
            Logger.Log("Switched to log tab");
            LogTextBlock.Text += Logger.GetLog();
            var logLength = LogTextBlock.Text.Length;
            if (logLength > LOG_BUFFER_SIZE)
            {
                LogTextBlock.Text = LogTextBlock.Text.Remove(0, logLength - LOG_BUFFER_SIZE);
            }
        }

        private TextBox[] getValueTextBoxes()
        {
            return new[]
            {
                Value1TextBox, Value2TextBox, Value3TextBox, Value4TextBox, Value5TextBox,
                Value6TextBox, Value7TextBox, Value8TextBox, Value9TextBox, Value10TextBox,
                Value11TextBox, Value12TextBox, Value13TextBox, Value14TextBox, Value15TextBox
            };
        }

        private void checkValueOfValueTextBox(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(textBox == null) throw new Exception("checkValueOfValueTextBox is not suitable to be used with " + sender);
            textBox.BorderBrush = InputValidation.TryParse<int>(textBox.Text) ? ERROR_COLOR_BRUSH : NORMAL_COLOR_BRUSH;
        }

        //TODO: Make a new and open dialogue for btree creatoin
        private void selectFile(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if(button == null) throw new Exception("selectFile is not suitable to be used with " + sender);
            string filePath = FileChooser.ChooseFileDialog();
        }

        private void addRecord(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!InputValidation.TryParseTextBoxCollection<int>(getValueTextBoxes(),
                    (TextBox textBox) => textBox.BorderBrush = ERROR_COLOR_BRUSH,
                    (TextBox textBox) => textBox.BorderBrush = NORMAL_COLOR_BRUSH))
                    return;
                BTree.Add(TextInputConverter.ConvertToRecord<int>(getValueTextBoxes()));
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to add record!");
                Logger.Log(ex);
            }
        }

        private void removeRecord(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!InputValidation.TryParse<int>(RecordValueTextBox.Text)) return;
                BTree.Remove(TextInputConverter.ConvertToKey<int>(RecordValueTextBox));
            }
            catch (Exception ex)
            {
                Logger.Log("Failed do remove record!");
            }
        }

        private void showNewBTreeDialog(object sender, RoutedEventArgs e)
        {
            var diaglog = new NewOpenDialog();
            var dialogResult = diaglog.ShowDialog();
            if (diaglog.BTree == null)
                Logger.Log("BTree creation aborted!");
            else
                BTree = diaglog.BTree;
        }

        private void refreshTreeView(object sender, RoutedEventArgs e)
        {
            if(BTree == null) return;
            var rootPage = BTree.GetRootPage();
        }

        private void buildTreeView(IPage<int> beginningPage, TreeViewItem current)
        {
            
//            newTreeViewItem.Header = beginningPage.ToString("kp");
//            current.Items.Add(newTreeViewItem);
            for (var i = 0; i < beginningPage.KeysInPage + 1; i++)
            {
                if (!beginningPage.PointerAt(i).Equals(BTreePagePointer<int>.NullPointer))
                {
                    buildTreeView(BTree.GetPage(beginningPage.PointerAt(i)), newTreeViewItem);
                }
                if (i < beginningPage.KeysInPage)
                {
                    var newTreeViewItem = new TreeViewItem();
                    newTreeViewItem.Header = beginningPage.KeyAt(i).Value.ToString();
                    current.Items.Add(newTreeViewItem);
                }
            }
        }
    }
}