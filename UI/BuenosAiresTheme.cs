using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BuenosAiresExp.UI
{
    public static class BuenosAiresTheme
    {
        public static readonly Color PrimaryColor = Color.FromArgb(27, 79, 138);        // azul principal 
        public static readonly Color PrimaryColorLight = Color.FromArgb(232, 240, 250); // azul claro
        public static readonly Color PrimaryColorHighlight = Color.FromArgb(190, 220, 255);       // sombra azulada para profundidade
        public static readonly Color PrimaryColorDark = Color.FromArgb(20, 60, 100);   // azul escuro
        public static readonly Color PrimaryColorMuted = Color.FromArgb(100, 150, 200); // azul suave para elementos secundários
        public static readonly Color PrimaryButtonHover = Color.FromArgb(165, 205, 245); // azul claro um pouco mais escuro para hover
        public static readonly Color AccentColor = Color.FromArgb(200, 169, 110);       // dourado
        public static readonly Color AccentColorLight = Color.FromArgb(220, 195, 140);  // hover do dourado
        public static readonly Color AccentColorMuted = Color.FromArgb(220, 195, 140);     // dourado suave para hover e destaques  
        public static readonly Color DangerColor = Color.FromArgb(155, 35, 83);         // vermelho-rosado 
        public static readonly Color PdfRedColor = Color.FromArgb(237, 28, 36);              // O vermelho 'acrobata'
        public static readonly Color PdfRedHover = Color.FromArgb(196, 17, 27);       // vermelho 'acrobata' mais escuro para hover
        public static readonly Color FillColor = Color.FromArgb(245, 242, 237);         // bege claro 
        public static readonly Color OffWhiteColor = Color.FromArgb(252, 250, 246);     // branco/off-white para superfícies claras
        public static readonly Color HeaderColor = Color.FromArgb(28, 28, 30);          // quase preto 
        public static readonly Color TextColor = Color.FromArgb(28, 28, 30);            // quase preto 
        public static readonly Color TextMutedColor = Color.FromArgb(120, 120, 120);    // cinza médio 
        public static readonly Color BorderColor = Color.FromArgb(200, 200, 195);       // cinza quente 
        public static readonly Color AccentColorDark = Color.FromArgb(175, 145, 85);    // dourado mais escuro
        public static readonly Color SuccessColor = Color.FromArgb(39, 120, 80);        // verde para confirmações
        public static readonly Color SuccessColorLight = Color.FromArgb(220, 242, 231); // fundo verde suave
        public static readonly Color DangerColorLight = Color.FromArgb(255, 235, 240);  // fundo vermelho suave
        public static readonly Color SurfaceColor = Color.FromArgb(255, 255, 255);      // branco puro para cards
        public static readonly Color SurfaceMutedColor = Color.FromArgb(250, 248, 244); // branco levemente quente

        public static readonly Color AccentColorHighlight = Color.FromArgb(230, 190, 100); // dourado médio para borda
        public static readonly Color AccentCardFill = Color.FromArgb(255, 248, 225);        // dourado bem claro, quase creme
        public static readonly Color AccentTextDark = Color.FromArgb(140, 90, 10);          // dourado escuro legível
        public static readonly Color AccentTextMid = Color.FromArgb(160, 110, 30);          // dourado médio para descrição
        public static readonly Color AccentButtonFill = Color.FromArgb(200, 155, 50);       // dourado vibrante para botão
        public static readonly Color AccentButtonHover = Color.FromArgb(175, 130, 30);

        public static readonly Color CategoryFoodBg = Color.FromArgb(255, 237, 213);
        public static readonly Color CategoryFoodFg = Color.FromArgb(154, 52, 18);
        public static readonly Color CategoryCoffeeBg = Color.FromArgb(254, 243, 199);
        public static readonly Color CategoryCoffeeFg = Color.FromArgb(146, 64, 14);
        public static readonly Color CategoryNatureBg = Color.FromArgb(209, 250, 229);
        public static readonly Color CategoryNatureFg = Color.FromArgb(6, 95, 70);
        public static readonly Color CategoryCultureBg = Color.FromArgb(219, 234, 254);
        public static readonly Color CategoryCultureFg = Color.FromArgb(30, 64, 175);
        public static readonly Color CategoryUrbanBg = Color.FromArgb(237, 233, 254);
        public static readonly Color CategoryUrbanFg = Color.FromArgb(109, 40, 217);
        public static readonly Color CategoryNightlifeBg = Color.FromArgb(252, 231, 243);
        public static readonly Color CategoryNightlifeFg = Color.FromArgb(157, 23, 77);
        public static readonly Color CategoryDefaultBg = Color.FromArgb(243, 244, 246);
        public static readonly Color CategoryDefaultFg = Color.FromArgb(75, 85, 99);


        public static readonly Font TitleFont = new Font("Segoe UI", 14f, FontStyle.Bold);
        public static readonly Font SubtitleFont = new Font("Segoe UI", 11f, FontStyle.Regular);
        public static readonly Font BodyFont = new Font("Segoe UI", 10.5f, FontStyle.Regular);
        public static readonly Font MutedFont = new Font("Segoe UI", 8f, FontStyle.Regular);
        public static readonly Font PlaceholderFont = new Font("Segoe UI", 10.5f, FontStyle.Italic);
        public static readonly Font FontMono = new Font("Consolas", 9f, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font LabelFont = new Font("Segoe UI", 9f, FontStyle.Bold);     
        public static readonly Font NotesFont = new Font("Segoe UI", 9f, FontStyle.Italic);   
        public static readonly Font BadgeFont = new Font("Segoe UI", 7.5f, FontStyle.Bold);   
        public static readonly Font CardTitleFont = new Font("Segoe UI", 10f, FontStyle.Bold); 
        public static readonly Font CardBodyFont = new Font("Segoe UI", 8.5f, FontStyle.Regular); 


        public static readonly int SmallRadius = 4;
        public static readonly int MediumRadius = 6;
        public static readonly int LargeRadius = 10;
        public static readonly int PaddingForm = 20;
        public static readonly int SpacingField = 20;
        public static readonly int ButtonHeight = 38;
        public static readonly int InputHeight = 36;
        public static readonly int HeaderHeight = 75;
        public static readonly int CardPadding = 12;
        public static readonly int CardRadius = 8;
        public static readonly int CardHeight = 72;       
        public static readonly int SeparatorHeight = 1;    

        private static Icon? _windowIcon;

        public static Icon? GetWindowIcon()
        {
            if (_windowIcon != null)
                return _windowIcon;

            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;

                var icoPath = Path.Combine(baseDir, "Assets", "baexp-logo-system", "icons", "ico", "icon-v3-expressive-32px-dark.ico");
                if (File.Exists(icoPath))
                {
                    _windowIcon = new Icon(icoPath);
                    return _windowIcon;
                }

                var pngPath = Path.Combine(baseDir, "Assets", "baexp-logo-system", "icons", "png", "icon-v3-expressive-32px-dark.png");
                if (File.Exists(pngPath))
                {
                    using var bmp = new Bitmap(pngPath);
                    _windowIcon = Icon.FromHandle(bmp.GetHicon());
                    return _windowIcon;
                }
            }
            catch
            {
            }

            return null;
        }


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
        }

        public static void ApplyTextBox (TextBox textbox)
        {
            textbox.BackColor = OffWhiteColor;
            textbox.ForeColor = TextColor;
            textbox.Font = BodyFont;
            textbox.BorderStyle = BorderStyle.FixedSingle;
            textbox.Height = InputHeight;
        }

        public static void ApplyComboBox (ComboBox comboBox) 
        { 
            comboBox.BackColor = OffWhiteColor;
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
            grid.RowTemplate.Height = 36;

            grid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = PrimaryColorLight;
            grid.ColumnHeadersDefaultCellStyle. Font = ButtonFont;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding (6, 0 , 0, 0);
            grid.ColumnHeadersHeight = 38;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.EnableHeadersVisualStyles = false;


            grid.DefaultCellStyle.BackColor = OffWhiteColor;
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

        // para converter um Color em string hexadecimal, útil para exportar cores em formatos como CSS ou JSON
        // uso no PDFservice apenas para converter as cores do tema em hex para o QuestPDF, que aceita cores em formato hexadecimal.
        public static string ToHex(this Color color)
            => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        private static readonly Dictionary<string, (Color Bg, Color Fg)> _CategoryPalette =
          new(StringComparer.OrdinalIgnoreCase)
          {
              ["restaurante"] = (CategoryFoodBg, CategoryFoodFg),
              ["parrilla"] = (CategoryFoodBg, CategoryFoodFg),
              ["pizzaria"] = (CategoryFoodBg, CategoryFoodFg),
              ["café"] = (CategoryCoffeeBg, CategoryCoffeeFg),
              ["cafeteria"] = (CategoryCoffeeBg, CategoryCoffeeFg),
              ["sorveteria"] = (CategoryCoffeeBg, CategoryCoffeeFg),
              ["parque"] = (CategoryNatureBg, CategoryNatureFg),
              ["reserva natural"] = (CategoryNatureBg, CategoryNatureFg),
              ["mirante"] = (CategoryNatureBg, CategoryNatureFg),
              ["museu"] = (CategoryCultureBg, CategoryCultureFg),
              ["teatro"] = (CategoryCultureBg, CategoryCultureFg),
              ["biblioteca"] = (CategoryCultureBg, CategoryCultureFg),
              ["centro cultural"] = (CategoryCultureBg, CategoryCultureFg),
              ["livraria"] = (CategoryCultureBg, CategoryCultureFg),
              ["monumento"] = (CategoryCultureBg, CategoryCultureFg),
              ["ponto turístico"] = (CategoryCultureBg, CategoryCultureFg),
              ["igreja"] = (CategoryCultureBg, CategoryCultureFg),
              ["bairro"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["rua"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["transporte"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["estádio"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["feira"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["mercado"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["shopping"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["supermercado"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["financeiro"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["saúde"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["hospedagem"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["cemitério"] = (CategoryUrbanBg, CategoryUrbanFg),
              ["bar"] = (CategoryNightlifeBg, CategoryNightlifeFg),
              ["vida noturna"] = (CategoryNightlifeBg, CategoryNightlifeFg),
              ["milonga"] = (CategoryNightlifeBg, CategoryNightlifeFg),
              ["outro"] = (CategoryDefaultBg, CategoryDefaultFg)
          };

        public static (Color Bg, Color Fg) GetCategoryColors(string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return (CategoryDefaultBg, CategoryDefaultFg);

            return _CategoryPalette.TryGetValue(category!.Trim(), out var palette) ? palette : (CategoryDefaultBg, CategoryDefaultFg);
        }

        public static Color GetCategoryColor(string? category) => GetCategoryColors(category).Bg;
        public static Color GetCategoryTextColor(string? category) => GetCategoryColors(category).Fg;
    }
}
