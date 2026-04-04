using BuenosAiresExp;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BuenosAiresExp.Models;
namespace BuenosAiresExp.Views
{
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
    private TableLayoutPanel _emptyStateLayout;

    private readonly ItineraryService _itineraryService = new();
    private List<Itinerary> _allItineraries = new();
    private FlowLayoutPanel _flowRoteiros;

    private const int CardsPerRow = 3;

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
            Text = "Monte e gerencie sua viagem",
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
            Dock = DockStyle.Left,
            BackColor = Color.White
        };
        _txtBuscar.TextChanged += OnSearchTextChanged;
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
        _emptyStateLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        _emptyStateLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
        _emptyStateLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _emptyStateLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));

        var emptyFlow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            Anchor = AnchorStyles.None,
            Location = new Point(0, 0),
            Margin = new Padding(0, 0, 0, 200)
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

        _emptyStateLayout.Controls.Add(new Panel(), 0, 0);
        _emptyStateLayout.Controls.Add(emptyFlow, 0, 1);
        _emptyStateLayout.Controls.Add(new Panel(), 0, 2);
        _emptyStateLayout.SetColumnSpan(emptyFlow, 1);

        _emptyStateLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
        emptyFlow.Anchor = AnchorStyles.None;
        _emptyStateLayout.SetCellPosition(emptyFlow, new TableLayoutPanelCellPosition(0, 1));

        _flowRoteiros = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoScroll = true,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 8, 0, 8)
        };

        _flowRoteiros.Resize += OnFlowRoteirosResize;

        _pnlContent.Controls.Add(_flowRoteiros);
        _pnlContent.Controls.Add(_emptyStateLayout);

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

    private void OnSearchTextChanged(object? sender, EventArgs e)
    {
        var q = _txtBuscar.Value;
        var filtered = string.IsNullOrWhiteSpace(q)
            ? _allItineraries
            : _allItineraries
                .Where(it => it.Name.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();

        RenderRoteiros(filtered);
    }

    private void OnFlowRoteirosResize(object? sender, EventArgs e)
    {
        if (_allItineraries.Count > 0)
        {
            RenderRoteiros(_allItineraries);
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

        bool hasItems = itineraries.Count > 0;
        _emptyStateLayout.Visible = !hasItems;
        _flowRoteiros.Visible = hasItems;

        if (!hasItems)
        {
            _flowRoteiros.ResumeLayout();
            return;
        }

        int cardWidth = CalculateCardWidth();
        int spacing = 16;
        int index = 0;

        foreach (var it in itineraries)
        {
            var card = CreateItineraryCard(it, cardWidth);
            bool isEndOfRow = ((index + 1) % CardsPerRow) == 0;
            card.Margin = new Padding(0, 0, isEndOfRow ? 0 : spacing, 16);
            _flowRoteiros.Controls.Add(card);
            index++;
        }

        _flowRoteiros.ResumeLayout();
    }

    private int CalculateCardWidth()
    {
        int spacing = 16;
        int available = _flowRoteiros.ClientSize.Width - _flowRoteiros.Padding.Horizontal;
        if (_flowRoteiros.VerticalScroll.Visible)
            available -= SystemInformation.VerticalScrollBarWidth;
        int width = (available - spacing * (CardsPerRow - 1)) / CardsPerRow;
        return Math.Max(width, 280);
    }

    private Panel CreateItineraryCard(Itinerary it, int cardWidth)
    {
        double totalKm = 0;
        var locs = it.Items.OrderBy(i => i.Order).Select(i => i.Location).ToList();
        for (int i = 0; i < locs.Count - 1; i++)
            totalKm += DistanceService.CalculateDistance(locs[i], locs[i + 1]);
        var distText = it.Items.Count < 2 ? "—"
            : totalKm < 1 ? $"{(int)(totalKm * 1000)} m"
            : $"{totalKm:F1} km";

        var card = new RoundedPanel
        {
            Width = cardWidth,
            Height = 145,
            FillColor = BuenosAiresTheme.SurfaceColor,
            BorderColor = BuenosAiresTheme.BorderColor,
            Padding = new Padding(18, 14, 18, 14)
        };

        // barra de ações — botões editar, excluir, visualizar
        var headerRow = new Panel
        {
            Dock = DockStyle.Top,
            Height = 36,
            BackColor = Color.Transparent
        };

        var lblNome = new Label
        {
            Text = it.Name,
            Font = new Font(BuenosAiresTheme.TitleFont.FontFamily, 12f, FontStyle.Bold),
            ForeColor = BuenosAiresTheme.TextColor,
            Dock = DockStyle.Fill,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(0, 2, 0, 0)
        };

        var pnlActions = new Panel
        {
            Dock = DockStyle.Right,
            Width = 126,
            BackColor = Color.Transparent
        };

        var btnEdit = MakeActionButton("Edit", BuenosAiresTheme.TextColor, "edit_icon.png");
        var btnDel = MakeActionButton("Del", BuenosAiresTheme.DangerColor, "delete_icon.png");
        var btnView = MakeActionButton("View", BuenosAiresTheme.AccentColor, "view_icon.png");

        btnEdit.Dock = DockStyle.Right;
        btnDel.Dock = DockStyle.Right;
        btnView.Dock = DockStyle.Right;

        btnEdit.Click += (s, e) => EditItinerary(it);
        btnDel.Click += (s, e) => DeleteItinerary(it);
        btnView.Click += (s, e) => ViewItinerary(it);

        pnlActions.Controls.Add(btnView);
        pnlActions.Controls.Add(btnDel);
        pnlActions.Controls.Add(btnEdit);

        headerRow.Controls.Add(lblNome);
        headerRow.Controls.Add(pnlActions);

        // badge de categoria colorido
        var flowContent = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0)
        };

        var lblData = new Label
        {
            Text = $"      {it.Date:dd/MM/yyyy}   •   {it.Items.Count} parada(s)",
            Font = BuenosAiresTheme.MutedFont,
            ForeColor = BuenosAiresTheme.TextColor,
            AutoSize = true,
            Margin = new Padding(0, 6, 0, 8)
        };

        try
        {
            var calendarIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "calendar_icon.png");
            if (File.Exists(calendarIconPath))
            {
                using var calendarStream = File.OpenRead(calendarIconPath);
                using var originalCalendarIcon = Image.FromStream(calendarStream);
                lblData.Image = new Bitmap(originalCalendarIcon, new Size(14, 14));
                lblData.ImageAlign = ContentAlignment.MiddleLeft;
                lblData.TextAlign = ContentAlignment.MiddleLeft;
            }
        }
        catch
        {
        }

        var lblDist = new Label
        {
            Text = $"    {distText}  ",
            Font = new Font(BuenosAiresTheme.BadgeFont.FontFamily, 8f, FontStyle.Bold),
            ForeColor = BuenosAiresTheme.AccentTextDark,
            BackColor = BuenosAiresTheme.AccentCardFill,
            AutoSize = true,
            Padding = new Padding(6, 2, 6, 2),
            Margin = new Padding(0, 0, 0, 8)
        };

        try
        {
            var distanceIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "distance_icon.png");
            if (File.Exists(distanceIconPath))
            {
                using var distanceStream = File.OpenRead(distanceIconPath);
                using var originalDistanceIcon = Image.FromStream(distanceStream);
                lblDist.Image = new Bitmap(originalDistanceIcon, new Size(12, 12));
                lblDist.ImageAlign = ContentAlignment.MiddleLeft;
                lblDist.TextAlign = ContentAlignment.MiddleLeft;
            }
        }
        catch
        {
        }

        var btnPdf = new RoundedButton
        {
            Text = "Exportar PDF",
            AutoSize = false,
            Width = 110,
            Height = 26,
            Font = BuenosAiresTheme.MutedFont,
            FillColor = BuenosAiresTheme.PdfRedColor,
            ForeColor = Color.White,
            HoverColor = BuenosAiresTheme.PdfRedHover,
            BackColor = BuenosAiresTheme.SurfaceColor,
            Margin = new Padding(0)
        };
        btnPdf.Click += (s, e) => ExportPdf(it);

        flowContent.Controls.AddRange(new Control[] { lblData, lblDist, btnPdf });

        card.Controls.Add(flowContent);
        card.Controls.Add(headerRow);

        return card;
    }

    private RoundedButton MakeActionButton(string text, Color color, string? iconFileName = null)
    {
        var button = new RoundedButton
        {
            Text = text,
            Width = 38,
            Height = 36,
            Font = new Font(BuenosAiresTheme.BodyFont.FontFamily, 10.5f, FontStyle.Regular),
            FillColor = Color.Transparent,
            ForeColor = color,
            HoverColor = BuenosAiresTheme.PrimaryColorLight,
            BackColor = BuenosAiresTheme.SurfaceColor,
            TextAlign = ContentAlignment.MiddleCenter,
            ImageAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(0)
        };

        if (!string.IsNullOrWhiteSpace(iconFileName))
        {
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", iconFileName);
            if (File.Exists(iconPath))
            {
                using var stream = File.OpenRead(iconPath);
                using var orig = Image.FromStream(stream);
                button.Image = new Bitmap(orig, new Size(16, 16));
                button.Text = string.Empty;
            }
        }

        return button;
    }

    private void EditItinerary(Itinerary itinerary)
    {
        var locations = new LocationService().GetAll();
        using var form = new ItineraryForm(locations, itinerary);
        form.ShowDialog();
        LoadRoteiros();
    }

    private void ViewItinerary(Itinerary itinerary)
    {
        var orderedLocations = itinerary.Items
            .OrderBy(i => i.Order)
            .Select(i => i.Location)
            .Where(l => l != null)
            .Cast<Location>()
            .ToList();

        if (orderedLocations.Count == 0)
        {
            MessageBox.Show("Este roteiro não possui locais para visualizar no mapa.",
                "Roteiro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var form = new ItineraryMapForm(orderedLocations, itinerary.Name);
        form.ShowDialog();
    }

    private void DeleteItinerary(Itinerary itinerary)
    {
        var confirm = MessageBox.Show(
            $"Deseja excluir o roteiro \"{itinerary.Name}\"?",
            "Confirmar exclusão",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm == DialogResult.Yes)
        {
            _itineraryService.Delete(itinerary.Id);
            LoadRoteiros();
        }
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
}
