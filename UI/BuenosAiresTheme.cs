using System;
using System.Collections.Generic;
using System.Text;

namespace BuenosAiresExp.UI
{
    public static class BuenosAiresTheme
    {
        public static readonly Color PrimaryColor = Color.FromArgb(27, 79, 138);        // azul principal 
        public static readonly Color PrimaryColorLight = Color.FromArgb(232, 240, 250); // azul claro
        public static readonly Color PrimaryColorDark = Color.FromArgb(20, 60, 100);   // azul escuro
        public static readonly Color PrimaryColorMuted = Color.FromArgb(100, 150, 200); // azul suave para elementos secundários
        public static readonly Color AccentColor = Color.FromArgb(200, 169, 110);       // dourado
        public static readonly Color AccentColorMuted = Color.FromArgb(220, 195, 140);     // dourado suave para hover e destaques  
        public static readonly Color DangerColor = Color.FromArgb(155, 35, 83);         // vermelho-rosado 
        public static readonly Color FillColor = Color.FromArgb(245, 242, 237);         // bege claro 
        public static readonly Color HeaderColor = Color.FromArgb(28, 28, 30);          // quase preto 
        public static readonly Color TextColor = Color.FromArgb(28, 28, 30);            // quase preto 
        public static readonly Color TextMutedColor = Color.FromArgb(120, 120, 120);    // cinza médio 
        public static readonly Color BorderColor = Color.FromArgb(200, 200, 195);       // cinza quente 
        public static readonly Color AccentColorLight = Color.FromArgb(220, 195, 140);  // hover do dourado
        public static readonly Color AccentColorDark = Color.FromArgb(175, 145, 85);    // dourado mais escuro
        public static readonly Color SuccessColor = Color.FromArgb(39, 120, 80);        // verde para confirmações
        public static readonly Color SuccessColorLight = Color.FromArgb(220, 242, 231); // fundo verde suave
        public static readonly Color DangerColorLight = Color.FromArgb(255, 235, 240);  // fundo vermelho suave
        public static readonly Color SurfaceColor = Color.FromArgb(255, 255, 255);      // branco puro para cards
        public static readonly Color SurfaceMutedColor = Color.FromArgb(250, 248, 244); // branco levemente quente

        public static readonly Font TitleFont = new Font("Segoe UI", 14f, FontStyle.Bold);
        public static readonly Font SubtitleFont = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font BodyFont = new Font("Segoe UI", 9f, FontStyle.Regular);
        public static readonly Font MutedFont = new Font("Segoe UI", 8f, FontStyle.Regular);
        public static readonly Font FontMono = new Font("Consolas", 9f, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 9f, FontStyle.Bold);
        public static readonly Font LabelFont = new Font("Segoe UI", 8f, FontStyle.Bold);     
        public static readonly Font NotesFont = new Font("Segoe UI", 9f, FontStyle.Italic);   
        public static readonly Font BadgeFont = new Font("Segoe UI", 7.5f, FontStyle.Bold);   
        public static readonly Font CardTitleFont = new Font("Segoe UI", 10f, FontStyle.Bold); 
        public static readonly Font CardBodyFont = new Font("Segoe UI", 8.5f, FontStyle.Regular); 


        public static readonly int SmallRadius = 4;
        public static readonly int MediumRadius = 6;
        public static readonly int LargeRadius = 10;
        public static readonly int PaddingForm = 20;
        public static readonly int SpacingField = 12;
        public static readonly int ButtonHeight = 34;
        public static readonly int InputHeight = 32;
        public static readonly int HeaderHeight = 75;
        public static readonly int CardPadding = 12;
        public static readonly int CardRadius = 8;
        public static readonly int CardHeight = 72;        // altura padrão de um card
        public static readonly int SeparatorHeight = 1;    // linha divisória


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

        public static void ApplyDataGridViewHover(DataGridView grid)
        {
            int hoveredRow = -1;

            grid.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                hoveredRow = e.RowIndex;
                grid.InvalidateRow(e.RowIndex);
            };

            grid.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                hoveredRow = -1;
                grid.InvalidateRow(e.RowIndex);
            };

            grid.RowPrePaint += (s, e) =>
            {
                if (grid.Rows[e.RowIndex].Selected) return;

                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                    e.RowIndex == hoveredRow
                        ? PrimaryColorLight
                        : e.RowIndex % 2 == 0
                            ? Color.White
                            : Color.FromArgb(248, 246, 242);
            };
        }

    }
}
