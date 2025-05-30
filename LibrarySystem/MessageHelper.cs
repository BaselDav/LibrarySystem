using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibrarySystem.Localization;

namespace LibrarySystem
{
    public static class MessageHelper
    {
        public static DialogResult ShowConfirm(string message, string title = Labels.ConfirmTitle)
        {
            return MessageBox.Show(
                message,
                title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2,
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading
            );
        }

        public static void ShowInfo(string message, string title = Labels.InfoTitle)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading
            );
        }
    }

}
