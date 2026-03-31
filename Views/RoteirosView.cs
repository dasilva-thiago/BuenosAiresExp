using BuenosAiresExp;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
using System;
using System.Drawing;
using System.Windows.Forms;
using BuenosAiresExp.Models;

public class RoteirosView : UserControl
{
    private Panel _pnlHeader;
    private TableLayoutPanel _headerLayout;
    private Label _lblTitle;
    private Label _lblSubtitle;
    private RoundedButton _btnNovoRoteiro;

    private Panel _pnlToolbar;
    private RoundedTextBox _txtBuscar;
    private Label _lblStatus;

    private Panel _pnlContent;

    private readonly ItineraryService _itineraryService = new();
    private List<Itinerary> _allItineraries = new();
    private FlowLayoutPanel _flowRoteiros;

    public RoteirosView()
    {
        BuildLayout();
    }

    private void BuildLayout()
    {
        _pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            Padding = new Padding(32, 16, 32, 0),
            BackColor = BuenosAiresTheme.FillColor
        };

        _headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
        };
        _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
        _headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
        _headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));

        _lblTitle = new Label
        {
            Text = "Roteiros",
            Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 20f, FontStyle.Bold),
            ForeColor = BuenosAiresTheme.TextColor,
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
            Margin = new Padding(0, 0, 5, 0)
        };
        _headerLayout.Controls.Add(_lblTitle, 0, 0);

        _lblSubtitle = new Label
        {
            Text = "Monte e gerencie seus roteiros de viagem",
            Font = BuenosAiresTheme.BodyFont,
            ForeColor = BuenosAiresTheme.TextMutedColor,
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Top
        };
        _headerLayout.Controls.Add(_lblSubtitle, 0, 1);

        _btnNovoRoteiro = new RoundedButton
        {
            Text = "Novo Roteiro",
            Width = 130,
            Font = BuenosAiresTheme.ButtonFont,
            ForeColor = Color.White,
            FillColor = BuenosAiresTheme.PrimaryColor,
            HoverColor = BuenosAiresTheme.PrimaryColorDark,
            BackColor = BuenosAiresTheme.FillColor,
            Dock = DockStyle.Right,
            Margin = new Padding(8, 4, 0, 4)
        };
        _btnNovoRoteiro.Click += BtnNovoRoteiro_Click;
        _headerLayout.SetRowSpan(_btnNovoRoteiro, 2);
        _headerLayout.Controls.Add(_btnNovoRoteiro, 1, 0);

        _pnlHeader.Controls.Add(_headerLayout);

        _pnlToolbar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 52,
            BackColor = BuenosAiresTheme.FillColor,
            Padding = new Padding(32, 8, 32, 8)
        };

        _txtBuscar = new RoundedTextBox
        {
            Width = 320,
            Height = 36,
            Placeholder = "Buscar por roteiros...",
            Dock = DockStyle.Left
        };
        _pnlToolbar.Controls.Add(_txtBuscar);

        _lblStatus = new Label
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            Font = BuenosAiresTheme.MutedFont,
            ForeColor = BuenosAiresTheme.TextMutedColor,
            TextAlign = ContentAlignment.MiddleRight,
            Margin = new Padding(0, 0, 12, 0)
        };
        _pnlToolbar.Controls.Add(_lblStatus);


        _pnlContent = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = BuenosAiresTheme.FillColor,
            Padding = new Padding(32, 16, 32, 16),
            AutoScroll = true
        };

        // Empty State inicial:
        var emptyStateLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        emptyStateLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
        emptyStateLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        emptyStateLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));

        var emptyFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            Anchor = AnchorStyles.None,
            Location = new Point(0, 0),
            Margin = new Padding(0,0,0,200)
        };

        var mapIcon = new Label
        {
            Size = new Size(48, 48),
            ImageAlign = ContentAlignment.MiddleCenter,
            AutoSize = false,
            Margin = new Padding(0, 0, 0, 16)
        };
        // try catch da imagem empty state
        try
        {
            mapIcon.Image = Image.FromFile("map_icon.png");
        }
        catch
        {
            mapIcon.Text = "";
            mapIcon.Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 36, FontStyle.Regular);
            mapIcon.TextAlign = ContentAlignment.MiddleCenter;
        }
        emptyFlow.Controls.Add(mapIcon);

        // empty state
        var lblEmptyTitle = new Label
        {
            Text = "Nenhum roteiro criado ainda",
            Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 16F, FontStyle.Bold),
            ForeColor = BuenosAiresTheme.TextColor,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleCenter,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 0)
        };
        emptyFlow.Controls.Add(lblEmptyTitle);

        // Descrição
        var lblEmptyDesc = new Label
        {
            Text = "Crie seu primeiro roteiro selecionando locais e calculando distâncias automaticamente.",
            Font = BuenosAiresTheme.BodyFont,
            ForeColor = BuenosAiresTheme.TextMutedColor,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleCenter,
            Height = 120,
            Padding = new Padding(32, 0, 32, 26)
        };
        emptyFlow.Controls.Add(lblEmptyDesc);

        // Botão de ação
        var btnCriarPrimeiro = new RoundedButton
        {
            Text = "Criar Primeiro Roteiro",
            Font = BuenosAiresTheme.ButtonFont,
            ForeColor = Color.White,
            FillColor = BuenosAiresTheme.PrimaryColor,
            BackColor = BuenosAiresTheme.PrimaryColorLight,
            HoverColor = BuenosAiresTheme.PrimaryButtonHover,
            Width = 220,
            Anchor = AnchorStyles.None,
            Padding = new Padding(12, 6, 12, 46)
        };
        btnCriarPrimeiro.Click += BtnNovoRoteiro_Click;
        emptyFlow.Controls.Add(btnCriarPrimeiro);

        emptyStateLayout.Controls.Add(new Panel(), 0, 0);
        emptyStateLayout.Controls.Add(emptyFlow, 0, 1);
        emptyStateLayout.Controls.Add(new Panel(), 0, 2);
        emptyStateLayout.SetColumnSpan(emptyFlow, 1);

        emptyStateLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
        emptyFlow.Anchor = AnchorStyles.None;
        emptyStateLayout.SetCellPosition(emptyFlow, new TableLayoutPanelCellPosition(0, 1));

        _flowRoteiros = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 8, 0, 8)
        };
        _pnlContent.Controls.Add(_flowRoteiros);

        _pnlContent.Controls.Add(emptyStateLayout);

        Controls.Add(_pnlContent);   
        Controls.Add(_pnlToolbar);   
        Controls.Add(_pnlHeader);    

        this.Dock = DockStyle.Fill;
        this.BackColor = BuenosAiresTheme.FillColor;
    }

    private void BtnNovoRoteiro_Click(object sender, EventArgs e)
    {
        var locations = new LocationService().GetAll();
        using (var form = new ItineraryForm(locations))
        {
            form.ShowDialog();
            LoadRoteiros();
        }
    }

    public void LoadRoteiros()
    {
        _allItineraries = _itineraryService.GetAll();
        RenderRoteiros(_allItineraries);
        _lblStatus.Text = $"{_allItineraries.Count} roteiro(s)";
    }

    private void RenderRoteiros(List<Itinerary> itineraries)
    {
        _flowRoteiros.SuspendLayout();
        _flowRoteiros.Controls.Clear();

        if (itineraries.Count == 0)
        {
            _flowRoteiros.Visible = false;
            _flowRoteiros.ResumeLayout();
            return;
        }

        _flowRoteiros.Visible = true;

        foreach (var it in itineraries)
        {
            var card = new RoundedPanel
            {
                Width = _flowRoteiros.ClientSize.Width - 16,
                Height = 90,
                FillColor = BuenosAiresTheme.SurfaceColor,
                BorderColor = BuenosAiresTheme.BorderColor,
                Padding = new Padding(16, 12, 16, 12),
                Margin = new Padding(0, 0, 0, 8)
            };

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));

            var pnlInfo = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            var lblNome = new Label
            {
                Text = it.Name,
                Font = BuenosAiresTheme.TitleFont,
                ForeColor = BuenosAiresTheme.TextColor,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblData = new Label
            {
                Text = $" {it.Date:dd/MM/yyyy}  •  {it.Items.Count} parada(s)", // adicionar icone de calendário e parada
                Font = BuenosAiresTheme.MutedFont,
                ForeColor = BuenosAiresTheme.TextMutedColor,
                AutoSize = true,
                Location = new Point(0, 28)
            };

            double totalKm = 0;
            var locs = it.Items.OrderBy(i => i.Order).Select(i => i.Location).ToList();
            for (int i = 0; i < locs.Count - 1; i++)
                totalKm += DistanceService.CalculateDistance(locs[i], locs[i + 1]);
            var distText = it.Items.Count < 2 ? "—"
                : totalKm < 1 ? $"{(int)(totalKm * 1000)} m"
                : $"{totalKm:F1} km";

            var lblDist = new Label
            {
                Text = $"↕ {distText}",
                Font = BuenosAiresTheme.MutedFont,
                ForeColor = BuenosAiresTheme.AccentTextDark,
                AutoSize = true,
                Location = new Point(0, 50)
            };

            pnlInfo.Controls.AddRange(new Control[] { lblNome, lblData, lblDist });

            var btnPdf = new RoundedButton
            {
                Text = "Exportar PDF",
                Dock = DockStyle.Fill,
                Font = BuenosAiresTheme.MutedFont,
                FillColor = BuenosAiresTheme.AccentCardFill,
                ForeColor = BuenosAiresTheme.AccentTextDark,
                HoverColor = BuenosAiresTheme.AccentColorLight,
                BackColor = BuenosAiresTheme.SurfaceColor,
                Margin = new Padding(8, 8, 0, 8)
            };
            btnPdf.Click += (s, e) => ExportPdf(it);

            tbl.Controls.Add(pnlInfo, 0, 0);
            tbl.Controls.Add(btnPdf, 1, 0);
            card.Controls.Add(tbl);

            _flowRoteiros.Resize += (s, e) =>
                card.Width = _flowRoteiros.ClientSize.Width - 16;

            _flowRoteiros.Controls.Add(card);
        }

        _flowRoteiros.ResumeLayout();
    }

    private void ExportPdf(Itinerary itinerary)
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Exportar Roteiro como PDF",
            Filter = "PDF (*.pdf)|*.pdf",
            FileName = itinerary.Name.Replace(" ", "_"),
            DefaultExt = "pdf"
        };
        if (dialog.ShowDialog() != DialogResult.OK) return;

        try
        {
            PdfService.GeneratePdf(itinerary, dialog.FileName);
            var open = MessageBox.Show("PDF gerado! Deseja abri-lo?", "Sucesso",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (open == DialogResult.Yes)
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = dialog.FileName,
                    UseShellExecute = true
                });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao gerar PDF:\n{ex.Message}", "Erro",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


}