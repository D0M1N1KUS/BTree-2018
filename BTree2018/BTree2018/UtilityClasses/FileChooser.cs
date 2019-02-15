using System.Windows.Forms;

namespace BTree2018.UtilityClasses
{
    public class FileChooser
    {
        public string ChooseFileDialog(string dialogTitle = "Choose file")
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = dialogTitle;
                dialog.CheckFileExists = false;
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                    return dialog.FileName;
            }
            return string.Empty;
        }
    }
}