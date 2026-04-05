using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuenosAiresExp.Models;
using BuenosAiresExp.UI;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BuenosAiresExp.Services
{
    public static class PdfService
    {
        public static void GeneratePdf(Itinerary itinerary, string outputPath)
        {
            var items = itinerary.Items.OrderBy(i => i.Order).ToList();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(0);
                    page.MarginVertical(0);
                    page.DefaultTextStyle(s => s
                        .FontFamily("Segoe UI")
                        .FontColor(BuenosAiresTheme.TextColor.ToHex())
                        .FontSize(10));

                    page.Header().Element(h => BuildHeader(h, itinerary));
                    page.Content().Element(c => BuildContent(c, items));
                    page.Footer().Element(f => BuildFooter(f));
                });
            }).GeneratePdf(outputPath);
        }

        // Header

        private static void BuildHeader(IContainer header, Itinerary itinerary)
        {
            header
                .Background(BuenosAiresTheme.PrimaryColor.ToHex())
                .PaddingHorizontal(40)
                .PaddingVertical(28)
                .Column(col =>
                {
                    col.Item()
                        .Text("Buenos Aires Explorer")
                        .FontSize(10)
                        .FontColor(BuenosAiresTheme.PrimaryColorHighlight.ToHex());

                    col.Item().Height(6);

                    col.Item()
                        .Text(itinerary.Name)
                        .FontSize(22)
                        .Bold()
                        .FontColor(BuenosAiresTheme.SurfaceColor.ToHex());

                    col.Item().Height(4);

                    col.Item()
                        .Text($"Data: {itinerary.Date:dd/MM/yyyy}  •  {itinerary.Items.Count} paradas")
                        .FontSize(10)
                        .FontColor(BuenosAiresTheme.PrimaryColorMuted.ToHex());

                    if (!string.IsNullOrWhiteSpace(itinerary.Notes))
                    {
                        col.Item().Height(4);
                        col.Item()
                            .Text(itinerary.Notes)
                            .FontSize(9)
                            .Italic()
                            .FontColor(BuenosAiresTheme.PrimaryColorHighlight.ToHex());
                    }
                });
        }

        //Content

        private static void BuildContent(IContainer content, List<ItineraryItem> items)
        {
            var locationIcon = TryLoadIconBytes("icon-pin-48px-light.png");
            var distanceIcon = TryLoadIconBytes("icon-arrows-updown-48px-light.png");

            content
                .Background(BuenosAiresTheme.FillColor.ToHex())
                .PaddingHorizontal(40)
                .PaddingTop(24)
                .PaddingBottom(12)
                .Column(col =>
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var loc = items[i].Location;
                        var hasNext = i < items.Count - 1;

                        col.Item()
                            .Background(BuenosAiresTheme.SurfaceColor.ToHex())
                            .Border(1)
                            .BorderColor(BuenosAiresTheme.BorderColor.ToHex())
                            .CornerRadius(10)
                            .Padding(16)
                            .Row(row =>
                            {
                                // Badge numérico
                                row.ConstantItem(44).AlignTop()
                                    .Width(36).Height(36)
                                    .Background(BuenosAiresTheme.PrimaryColorLight.ToHex())
                                    .Border(1.5f)
                                    .BorderColor(BuenosAiresTheme.PrimaryColor.ToHex())
                                    .CornerRadius(18)
                                    .AlignCenter().AlignMiddle()
                                    .Text($"{i + 1}")
                                    .FontSize(13).Bold()
                                    .FontColor(BuenosAiresTheme.PrimaryColor.ToHex());

                                // Dados do local
                                row.RelativeItem().Column(info =>
                                {
                                    // Nome + badge de categoria
                                    info.Item().Row(nameRow =>
                                    {
                                        nameRow.RelativeItem()
                                            .Text(loc.Name)
                                            .FontSize(13).Bold()
                                            .FontColor(BuenosAiresTheme.TextColor.ToHex());

                                        var (bg, fg) = BuenosAiresTheme.GetCategoryColors(loc.Category);
                                        nameRow.AutoItem()
                                            .Background(bg.ToHex())
                                            .CornerRadius(4)
                                            .PaddingHorizontal(8)
                                            .PaddingVertical(2)
                                            .AlignMiddle()
                                            .Text(loc.Category)
                                            .FontSize(8).Bold()
                                            .FontColor(fg.ToHex());
                                    });

                                    info.Item().Height(4);

                                    info.Item().Row(addressRow =>
                                    {
                                        if (locationIcon != null)
                                        {
                                            addressRow.ConstantItem(12)
                                                .Height(12)
                                                .AlignMiddle()
                                                .Image(locationIcon, ImageScaling.FitArea);
                                        }
                                        else
                                        {
                                            addressRow.AutoItem()
                                                .Text("📍")
                                                .FontSize(9)
                                                .FontColor(BuenosAiresTheme.TextMutedColor.ToHex());
                                        }

                                        addressRow.RelativeItem()
                                            .PaddingLeft(4)
                                            .Text(loc.Address)
                                            .FontSize(9)
                                            .FontColor(BuenosAiresTheme.TextMutedColor.ToHex());
                                    });

                                    info.Item().Height(2);

                                    info.Item()
                                        .Text($"{loc.Latitude:F5}°,  {loc.Longitude:F5}°")
                                        .FontSize(8)
                                        .FontColor(BuenosAiresTheme.BorderColor.ToHex());

                                    if (!string.IsNullOrWhiteSpace(loc.Notes))
                                    {
                                        info.Item().Height(6);
                                        info.Item()
                                            .Background(BuenosAiresTheme.FillColor.ToHex())
                                            .CornerRadius(4)
                                            .Padding(6)
                                            .Text(loc.Notes)
                                            .FontSize(9).Italic()
                                            .FontColor(BuenosAiresTheme.TextMutedColor.ToHex());
                                    }
                                });
                            });

                        // Conector de distância entre paradas
                        if (hasNext)
                        {
                            var dist = DistanceService.FormatDistance(loc, items[i + 1].Location);
                            col.Item()
                                .PaddingLeft(20)
                                .PaddingVertical(4)
                                .Row(distRow =>
                                {
                                    if (distanceIcon != null)
                                    {
                                        distRow.ConstantItem(16)
                                            .Height(16)
                                            .AlignMiddle()
                                            .Image(distanceIcon, ImageScaling.FitArea);
                                    }
                                    else
                                    {
                                        distRow.AutoItem()
                                            .Text("↕")
                                            .FontSize(9)
                                            .Bold()
                                            .FontColor(BuenosAiresTheme.AccentColor.ToHex());
                                    }

                                    distRow.RelativeItem()
                                        .PaddingLeft(6)
                                        .Text(dist)
                                        .FontSize(9)
                                        .Bold()
                                        .FontColor(BuenosAiresTheme.AccentColor.ToHex());
                                });
                        }

                        col.Item().Height(8);
                    }
                });
        }

        private static byte[]? TryLoadIconBytes(string fileName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "baexp-icons", "png", fileName);
            if (!File.Exists(path))
                return null;

            try
            {
                return File.ReadAllBytes(path);
            }
            catch
            {
                return null;
            }
        }

        //Footer

        private static void BuildFooter(IContainer footer)
        {
            footer
                .Background(BuenosAiresTheme.SurfaceColor.ToHex())
                .BorderTop(1)
                .BorderColor(BuenosAiresTheme.BorderColor.ToHex())
                .PaddingHorizontal(40)
                .PaddingVertical(10)
                .Row(row =>
                {
                    row.RelativeItem()
                        .Text("Buenos Aires Explorer  •  Sua viagem com autonomia e controle")
                        .FontSize(8)
                        .FontColor(BuenosAiresTheme.TextMutedColor.ToHex());

                    row.AutoItem()
                        .Text($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(8)
                        .FontColor(BuenosAiresTheme.TextMutedColor.ToHex());
                });
        }
    }
}