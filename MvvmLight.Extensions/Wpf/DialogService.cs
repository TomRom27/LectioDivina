using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using Cursor = System.Windows.Forms.Cursor;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;

namespace MvvmLight.Extensions.Wpf
{
    public class DialogService : IDialogService
    {
        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            if (afterHideCallback != null)
                afterHideCallback();

            return Task.Factory.StartNew(() => { });
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText,
            Action<bool> afterHideCallback)
        {
            bool isConfirmed = (MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes);
            if (afterHideCallback != null)
                afterHideCallback(isConfirmed);

            return new Task<bool>(() => { return isConfirmed; });
        }

        // single button message
        public Task ShowMessage(string message, string title)
        {
            
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);

            return Task.Factory.StartNew(() => { });
        }

        public string SelectFile(string description, string allowedExtenstions)
        {

            var dialog = new System.Windows.Forms.OpenFileDialog();

            dialog.CheckFileExists = true;
            dialog.Title = description;
            if (!String.IsNullOrEmpty(allowedExtenstions))
            {
                // "Text files (*.txt)|*.txt|All files (*.*)|*.*"
                dialog.Filter = "(" + allowedExtenstions + ")|" + allowedExtenstions + "|(*.*)|*.*";
            }

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
                return dialog.FileName;
            else
                return "";
        }


        public string SelectFolder(string description, string selectedFolder)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (!String.IsNullOrEmpty(selectedFolder))
                dialog.SelectedPath = selectedFolder;

            dialog.Description = description;

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
                return dialog.SelectedPath;
            else
                return "";
        }

        private System.Windows.Input.Cursor normalCursor;

        public void SetBusy()
        {

            normalCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
        }

        public void SetNormal()
        {
            Mouse.OverrideCursor = normalCursor;
        }

        /*
private Window FindOwnerWindow(object viewModel)
    {
        var view = views.SingleOrDefault(v => ReferenceEquals(v.DataContext, viewModel));

        if (view == null)
        {
            throw new ArgumentException("Viewmodel is not referenced by any registered View.");
        }

        DependencyObject owner = view;

        // Iterate through parents until a window is found, 
        // if the view is not a window itself
        while (!(owner is Window))
        {
            owner = VisualTreeHelper.GetParent(owner);
            if (owner == null) 
                throw new Exception("No window found owning the view.");
        }

        // Make sure owner window was found
        if (owner == null)
        {
            throw new InvalidOperationException("View is not contained within a Window.");
        }

        return (Window) owner;
    }         
         * */

    }
}
