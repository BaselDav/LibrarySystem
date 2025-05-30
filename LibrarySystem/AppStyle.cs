using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibrarySystem
{
    public static class AppStyle
    {
        public static readonly Font DefaultFont = new Font("Segoe UI", 10);
        //public static readonly Font DefaultFont = new Font("Cairo", 11);
        public static readonly Color PrimaryColor = Color.FromArgb(33, 150, 243); 
        public static readonly Color ButtonColor = Color.FromArgb(25, 118, 210);
        public static readonly Color LightGray = Color.FromArgb(245, 245, 245);

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse
        );

        public static void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = new Font(DefaultFont.FontFamily, 9.5f, FontStyle.Regular);
            btn.Height = 34;
            btn.Width = 100;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.BorderSize = 0;

            ApplyButtonColors(btn);

            btn.EnabledChanged -= Btn_EnabledChanged;
            btn.EnabledChanged += Btn_EnabledChanged;

            btn.Region = System.Drawing.Region.FromHrgn(
                CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 10, 10));
        }
        private static void ApplyButtonColors(Button btn)
        {
            if (!btn.Enabled)
            {
                btn.BackColor = Color.FromArgb(230, 230, 230); 
                btn.ForeColor = Color.DarkGray;
                btn.FlatAppearance.MouseOverBackColor = btn.BackColor;
                btn.FlatAppearance.MouseDownBackColor = btn.BackColor;
            }
            else
            {
                btn.BackColor = Color.FromArgb(66, 133, 244); 
                btn.ForeColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 103, 198);
                btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 80, 160);
            }
        }
        private static void Btn_EnabledChanged(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                ApplyButtonColors(btn);
            }
        }



        public static void StyleComboBox(ComboBox cmb)
        {
            cmb.Font = DefaultFont;
            cmb.Height = 40;
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.RightToLeft = RightToLeft.No;
            //cmb.RightToLeft = RightToLeft.Yes;
            cmb.FlatStyle = FlatStyle.Flat;
        }

        public static void StyleTextBox(TextBox txt)
        {
            txt.Font = DefaultFont;
            txt.RightToLeft = RightToLeft.No;
            //txt.RightToLeft = RightToLeft.Yes;
            txt.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void StyleGrid(DataGridView dgv)
        {
            dgv.Font = DefaultFont;
            dgv.RowTemplate.Height = 35;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = Color.White;
            dgv.GridColor = Color.Gainsboro;
            dgv.DefaultCellStyle.SelectionBackColor = Color.DarkGray;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(DefaultFont.FontFamily, 11, FontStyle.Bold);
            dgv.RightToLeft = RightToLeft.No;
            //dgv.RightToLeft = RightToLeft.Yes;
        }

        public static void StyleLabel(Label lbl)
        {
            lbl.Font = DefaultFont;
            lbl.ForeColor = Color.DimGray;
        }
    }
}
