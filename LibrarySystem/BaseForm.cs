using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibrarySystem
{
    public class BaseForm : Form
    {
        public BaseForm()
        {
            this.Font = AppStyle.DefaultFont;
            this.RightToLeft = RightToLeft.No; 
            this.RightToLeftLayout = false;
            //this.RightToLeft = RightToLeft.Yes;
            //this.RightToLeftLayout = true;
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ApplyStyleToAllControls(this.Controls);
        }

        public void ApplyStyleToAllControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control)
                {
                    case Button btn:
                        AppStyle.StyleButton(btn);
                        break;

                    case ComboBox cmb:
                        AppStyle.StyleComboBox(cmb);
                        break;

                    case TextBox txt:
                        AppStyle.StyleTextBox(txt);
                        break;

                    case DataGridView dgv:
                        AppStyle.StyleGrid(dgv);
                        break;

                    case Label lbl:
                        AppStyle.StyleLabel(lbl);
                        break;
                }

                if (control.HasChildren)
                    ApplyStyleToAllControls(control.Controls);
            }
        }

        public void FormatDataGridViewHeadersAndAlignment(DataGridView dgv)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dgv.ColumnHeadersDefaultCellStyle.BackColor = AppStyle.PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(AppStyle.DefaultFont.FontFamily, 11, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
        }
    }
}
