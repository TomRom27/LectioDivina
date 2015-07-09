using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmLight.Extensions
{
    public interface IDialogService
    {
        Task ShowError(string message, string title, string buttonText, Action afterHideCallback);

        Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText,
            Action<bool> afterHideCallback);

        Task ShowMessage(string message, string title);

        string SelectFile(string description, string allowedExtenstions);

        string SelectFolder(string description, string selectedFolder);

        void SetBusy();
        void SetNormal();
    }
}
