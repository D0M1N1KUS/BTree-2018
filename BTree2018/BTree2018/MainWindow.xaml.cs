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
        private readonly Brush WHITE_BRUSH = Brushes.White;

        private IBTree<int> BTree;

        private MultipleOperationExecuter<int> OperationsExecuter;
        
        public MainWindow()
        {
            InitializeComponent();
            NORMAL_COLOR_BRUSH = Value1TextBox.BorderBrush;
            enableButtons(false);
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

        private void clearLog(object sender, RoutedEventArgs e)
        {
            LogTextBlock.Text = string.Empty;
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
            textBox.BorderBrush = InputValidation.TryParse<int>(textBox.Text) ? NORMAL_COLOR_BRUSH : ERROR_COLOR_BRUSH;
        }

        private void clearValueTextBoxes(object sender, RoutedEventArgs e)
        {
            var valueTextBoxes = getValueTextBoxes();
            foreach (var valueTextBox in valueTextBoxes)
            {
                valueTextBox.Text = "0";
            }

            RecordValueTextBox.Text = "0";
        }

        private void addRecord(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!InputValidation.TryParseTextBoxCollection<int>(getValueTextBoxes(),
                    (TextBox textBox) => textBox.BorderBrush = ERROR_COLOR_BRUSH,
                    (TextBox textBox) => textBox.BorderBrush = NORMAL_COLOR_BRUSH))
                    return;
                var recordToAdd = TextInputConverter.ConvertToRecord<int>(getValueTextBoxes());
                BTree.Add(recordToAdd);
                writeStatisticsToInfoBar();               
                if(RefreshTreeViewCheckBox.IsChecked ?? false) refreshTreeView(sender, e);
                RecordOperationInfoTextBlock.Foreground = WHITE_BRUSH;
                RecordOperationInfoTextBlock.Text = "Successfully added " + recordToAdd;
                Statistics.GetStatistics();
            }
            catch (Exception ex)
            {
                RecordOperationInfoTextBlock.Foreground = ERROR_COLOR_BRUSH;
                RecordOperationInfoTextBlock.Text = "Failed to add record!";
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
                writeStatisticsToInfoBar();
                if(RefreshTreeViewCheckBox.IsChecked ?? false) refreshTreeView(sender, e);
                RecordOperationInfoTextBlock.Foreground = WHITE_BRUSH;
                RecordOperationInfoTextBlock.Text = "Record successfully removed.";
                Statistics.GetStatistics();
            }
            catch (Exception ex)
            {
                RecordOperationInfoTextBlock.Foreground = ERROR_COLOR_BRUSH;
                RecordOperationInfoTextBlock.Text = "Failed do remove record!";
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
            {
                BTree = diaglog.BTree;
                enableButtons();
            }
        }

        private void enableButtons(bool enable = true)
        {
            AddRecordButton.IsEnabled = enable;
            RemoveRecordButton.IsEnabled = enable;
            SearchValueButton.IsEnabled = enable;
            AlterRecordButton.IsEnabled = enable;
            ExecuteMultipleOperationsButton.IsEnabled = enable;
        }

        private void refreshTreeView(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BTree == null) return;
                var rootPage = BTree.GetRootPage();
                RootTreeViewItem.Items.Clear();
                buildTreeView(rootPage, RootTreeViewItem);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void buildTreeView(IPage<int> beginningPage, TreeViewItem current)
        {
            if (beginningPage.PageType == PageType.NULL || beginningPage.KeysInPage == 0)
            {
                var newTreeViewItem = new TreeViewItem();
                newTreeViewItem.Foreground = WHITE_BRUSH;
                newTreeViewItem.Header = "[empty]";
                current.Items.Add(newTreeViewItem);
            }
            for (var i = 0; i < beginningPage.KeysInPage + 1; i++)
            {
                if (!beginningPage.PointerAt(i).Equals(BTreePagePointer<int>.NullPointer))
                {
                    var newTreeViewItem = new TreeViewItem();
                    newTreeViewItem.Foreground = WHITE_BRUSH;
                    newTreeViewItem.Header = "PageIndex: " + beginningPage.PointerAt(i).Index;
                    buildTreeView(BTree.GetPage(beginningPage.PointerAt(i)), newTreeViewItem);
                    current.Items.Add(newTreeViewItem);
                }
                if (i < beginningPage.KeysInPage)
                {
                    var newTreeViewItem = new TreeViewItem();
                    newTreeViewItem.Foreground = WHITE_BRUSH;
                    newTreeViewItem.Header = "Key: " + beginningPage.KeyAt(i).Value;
                    var i1 = i;
                    newTreeViewItem.MouseLeftButtonUp += (sender, args) => { loadRecord(beginningPage.KeyAt(i1)); }; 
                    current.Items.Add(newTreeViewItem);
                }
            }
        }

        private void loadRecord(IKey<int> key)
        {
            try
            {
                var record = BTree.Get(key);
                var valueTextBoxes = getValueTextBoxes();
                for(var i = 0; i < record.ValueComponents.Length; i++)
                {
                    valueTextBoxes[i].Text = record.ValueComponents[i].ToString();
                }

                RecordValueTextBox.Text = record.Value.ToString();
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        private void executeCommands(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OperationsExecuter == null)
                    OperationsExecuter = new MultipleOperationExecuter<int>(BTree)
                    {
                        Executer = new CommandExecuter<int>() {bTree = BTree}
                    };
                OperationsExecuter.Execute(MultipleOperationsScriptTextBox.Text);
                writeStatisticsToInfoBar();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void commandTabFocused(object sender, RoutedEventArgs e)
        {
            MultipleOperationsInfo.Content = string.Empty;
        }

        private void writeStatisticsToInfoBar()
        {
            var statistics = Statistics.GetStatistics(clearStatistics: true);
            StatusLabel.Content = string.Concat("Pages read [", statistics.Item3,
                "]  Pages written [", statistics.Item4,
                "]  Bytes read [", statistics.Item1,
                "]  Bytes written [", statistics.Item2, "]");
        }
    }
}