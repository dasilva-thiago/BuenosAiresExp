using System;
using System.Collections.Generic;
using System.Text;

namespace BuenosAiresExp.UI
{
    public static class BuenosAiresTheme
    {
        public static readonly Color PrimaryColor = Color.FromArgb(27, 79, 138);
        public static readonly Color PrimaryColorLight = Color.FromArgb(232, 240, 250);
        public static readonly Color AccentColor = Color.FromArgb(200, 169, 110);
        public static readonly Color DangerColor = Color.FromArgb(155, 35, 83);
        public static readonly Color FillColor = Color.FromArgb(245, 242, 237);
        public static readonly Color HeaderColor = Color.FromArgb(28, 28, 30);
        public static readonly Color TextColor = Color.FromArgb(28, 28, 30);
        public static readonly Color TextMutedColor = Color.FromArgb(120, 120, 120);
        public static readonly Color BorderColor = Color.FromArgb(200, 200, 195);

        public static readonly Font TitleFont = new Font("Segoe UI", 14f, FontStyle.Bold);
        public static readonly Font SubtitleFont = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font BodyFont = new Font("Segoe UI", 9f, FontStyle.Regular);
        public static readonly Font MutedFont = new Font("Segoe UI", 8f, FontStyle.Regular);
        public static readonly Font FontMono = new Font("Consolas", 9f, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 9f, FontStyle.Bold);

        public static readonly int SmallRadius = 4;
        public static readonly int MediumRadius = 6;
        public static readonly int LargeRadius = 10;
        public static readonly int PaddingForm = 20;
        public static readonly int SpacingField = 12;
        public static readonly int ButtonHeight = 34;
        public static readonly int InputHeight = 32;
        public static readonly int HeaderHeight = 75;


        public static void ApplyForm(Form form)
        {
            form.BackColor = FillColor;
            form.Font = BodyFont;
        }
        
        public static void ApplyLabel (Label label, bool isMuted = false)
        {
            label.ForeColor = isMuted ? TextMutedColor : PrimaryColor;
            label.Font = isMuted ? MutedFont: new Font("Segoe UI", 8f, FontStyle.Bold);
            label.AutoSize = true;

            // O AutoSize = true faz com que o tamanho do label se ajuste automaticamente ao conteúdo, evitando que o texto fique cortado ou que haja muito espaço em branco.
            // isMuted aqui é definido como false. Chamando ApplyLabel(lblExemplo, true), o texto ficará com a cor definida em TextMutedColor e usará MutedFont
        }

        public static void ApplyTextBox (TextBox textbox)
        {
            textbox.BackColor = Color.White;
            textbox.ForeColor = TextColor;
            textbox.Font = BodyFont;
            textbox.BorderStyle = BorderStyle.FixedSingle;
            textbox.Height = InputHeight;
        }

        public static void ApplyComboBox (ComboBox comboBox) 
        { 
            comboBox.BackColor = Color.White;
            comboBox.ForeColor = TextColor;
            comboBox.Font = BodyFont;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Height = InputHeight;
        }

        public static void ApplyDataGridView (DataGridView grid)
        {
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            grid.BackgroundColor = FillColor;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = BorderColor;
            grid.Font = BodyFont;   
            grid.RowTemplate.Height = 32;

            grid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = PrimaryColorLight;
            grid.ColumnHeadersDefaultCellStyle. Font = ButtonFont;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding (6, 0 , 0, 0);
            grid.ColumnHeadersHeight = 34;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.EnableHeadersVisualStyles = false;


            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = TextColor;
            grid.DefaultCellStyle.SelectionBackColor = PrimaryColorLight;
            grid.DefaultCellStyle.SelectionForeColor = PrimaryColor;
            grid.DefaultCellStyle.Padding = new Padding(6, 0, 0, 0);

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 246, 242);

        }

    }
}
