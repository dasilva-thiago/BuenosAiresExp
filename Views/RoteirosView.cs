using BuenosAiresExp;
using BuenosAiresExp.Services;
using BuenosAiresExp.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

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
            form.ShowDialog();
    }
}