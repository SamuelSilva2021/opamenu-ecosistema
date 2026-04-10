using OpaMenu.Desktop.Models.DTOs.Printing;
using OpaMenu.Desktop.Models.Enums;
using OpaMenu.Desktop.Models.Printing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpaMenu.Desktop.Services.Implementation.Printing;

internal sealed class EscPosRenderer
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public byte[] RenderTest(PrinterMapping mapping)
    {
        var bytes = new List<byte>(512);
        bytes.AddRange(Init());
        bytes.AddRange(SelectCodePage());

        bytes.AddRange(TextCentered("OPAMENU", mapping, bold: true, doubleHeight: true));
        bytes.AddRange(TextCentered("Teste de Impressão", mapping));
        bytes.AddRange(TextCentered(DateTime.Now.ToString("dd/MM/yyyy HH:mm", PtBr), mapping));
        bytes.AddRange(Feed(2));
        bytes.AddRange(Cut());
        return bytes.ToArray();
    }

    public byte[] RenderTabBill(TabBillPrintPayload payload, PrinterMapping mapping)
    {
        var width = GetWidth(mapping.PaperSize);
        var bytes = new List<byte>(4096);
        bytes.AddRange(Init());
        bytes.AddRange(SelectCodePage());

        bytes.AddRange(TextCentered("CONTA", mapping, bold: true, doubleHeight: true));
        bytes.AddRange(TextLine($"Mesa: {payload.TableName}", mapping));
        bytes.AddRange(TextLine($"Comanda: {payload.TabName}", mapping));
        bytes.AddRange(TextLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm", PtBr), mapping));
        bytes.AddRange(Hr(width, mapping));

        foreach (var order in payload.Orders.OrderBy(o => o.CreatedAt))
        {
            bytes.AddRange(TextLine($"Pedido #{order.OrderNumber}", mapping, bold: true));
            foreach (var item in order.Items)
            {
                var left = $"{item.Quantity}x {item.ProductName}";
                var right = item.Subtotal.ToString("C", PtBr);
                bytes.AddRange(TwoColumnWrapped(left, right, width, mapping));

                foreach (var ad in item.Aditionals)
                {
                    var adLeft = $"  + {ad.Quantity}x {ad.AditionalName}";
                    var adRight = ad.Subtotal.ToString("C", PtBr);
                    bytes.AddRange(TwoColumnWrapped(adLeft, adRight, width, mapping));
                }

                if (!string.IsNullOrWhiteSpace(item.Notes))
                {
                    foreach (var line in Wrap($"  Obs: {item.Notes.Trim()}", width))
                        bytes.AddRange(TextLine(line, mapping));
                }
            }

            bytes.AddRange(Hr(width, mapping));
        }

        bytes.AddRange(TwoColumn("TOTAL", payload.Total.ToString("C", PtBr), width, mapping, bold: true));
        bytes.AddRange(Feed(2));
        bytes.AddRange(Cut());
        return bytes.ToArray();
    }

    private static int GetWidth(EPrinterPaperSize paperSize)
    {
        return paperSize switch
        {
            EPrinterPaperSize.Mm58 => 32,
            EPrinterPaperSize.Mm80 => 42,
            _ => 42
        };
    }

    private static IEnumerable<byte> Init() => new byte[] { 0x1B, 0x40 };

    private static IEnumerable<byte> SelectCodePage() => new byte[] { 0x1B, 0x74, 0x10 };

    private static IEnumerable<byte> Feed(int lines)
    {
        for (var i = 0; i < lines; i++)
            yield return 0x0A;
    }

    private static IEnumerable<byte> Cut() => new byte[] { 0x1D, 0x56, 0x42, 0x00 };

    private static IEnumerable<byte> TextCentered(string text, PrinterMapping mapping, bool bold = false, bool doubleHeight = false)
    {
        var width = GetWidth(mapping.PaperSize);
        foreach (var line in Wrap(text, width))
        {
            yield return 0x1B; yield return 0x61; yield return 0x01;
            foreach (var b in SelectBold(bold)) yield return b;
            foreach (var b in SelectDoubleHeight(doubleHeight)) yield return b;
            foreach (var b in Encode(line)) yield return b;
            yield return 0x0A;
            foreach (var b in SelectBold(false)) yield return b;
            foreach (var b in SelectDoubleHeight(false)) yield return b;
            yield return 0x1B; yield return 0x61; yield return 0x00;
        }
    }

    private static IEnumerable<byte> TextLine(string text, PrinterMapping mapping, bool bold = false)
    {
        foreach (var b in SelectBold(bold)) yield return b;
        foreach (var line in Wrap(text, GetWidth(mapping.PaperSize)))
        {
            foreach (var b in Encode(line)) yield return b;
            yield return 0x0A;
        }
        foreach (var b in SelectBold(false)) yield return b;
    }

    private static IEnumerable<byte> Hr(int width, PrinterMapping mapping)
    {
        return TextLine(new string('-', width), mapping);
    }

    private static IEnumerable<byte> TwoColumn(string left, string right, int width, PrinterMapping mapping, bool bold = false)
    {
        foreach (var b in SelectBold(bold)) yield return b;
        var line = ComposeTwoColumnLine(left, right, width);
        foreach (var b in Encode(line)) yield return b;
        yield return 0x0A;
        foreach (var b in SelectBold(false)) yield return b;
    }

    private static IEnumerable<byte> TwoColumnWrapped(string left, string right, int width, PrinterMapping mapping)
    {
        var rightTrimmed = right.Trim();
        var rightLen = rightTrimmed.Length;
        var leftMax = Math.Max(1, width - rightLen - 1);

        var wrappedLeft = Wrap(left, leftMax).ToList();
        if (wrappedLeft.Count == 0)
        {
            foreach (var b in TwoColumn(string.Empty, rightTrimmed, width, mapping)) yield return b;
            yield break;
        }

        foreach (var b in Encode(ComposeTwoColumnLine(wrappedLeft[0], rightTrimmed, width))) yield return b;
        yield return 0x0A;

        for (var i = 1; i < wrappedLeft.Count; i++)
        {
            foreach (var b in Encode(wrappedLeft[i])) yield return b;
            yield return 0x0A;
        }
    }

    private static string ComposeTwoColumnLine(string left, string right, int width)
    {
        left = left ?? string.Empty;
        right = right ?? string.Empty;

        if (right.Length >= width)
            return right[..Math.Min(width, right.Length)];

        var leftMax = Math.Max(0, width - right.Length - 1);
        var leftTrim = left.Length > leftMax ? left[..leftMax] : left;
        var spaces = new string(' ', Math.Max(1, width - leftTrim.Length - right.Length));
        return leftTrim + spaces + right;
    }

    private static IEnumerable<string> Wrap(string text, int width)
    {
        text ??= string.Empty;
        if (width <= 1)
            return new[] { text };

        var lines = new List<string>();
        var remaining = text.TrimEnd();

        while (!string.IsNullOrEmpty(remaining))
        {
            if (remaining.Length <= width)
            {
                lines.Add(remaining);
                break;
            }

            var slice = remaining[..width];
            var lastSpace = slice.LastIndexOf(' ');
            if (lastSpace <= 0)
            {
                lines.Add(slice);
                remaining = remaining[width..].TrimStart();
                continue;
            }

            lines.Add(slice[..lastSpace].TrimEnd());
            remaining = remaining[lastSpace..].TrimStart();
        }

        if (lines.Count == 0)
            lines.Add(string.Empty);

        return lines;
    }

    private static IEnumerable<byte> SelectBold(bool bold)
    {
        yield return 0x1B;
        yield return 0x45;
        yield return bold ? (byte)0x01 : (byte)0x00;
    }

    private static IEnumerable<byte> SelectDoubleHeight(bool enabled)
    {
        yield return 0x1D;
        yield return 0x21;
        yield return enabled ? (byte)0x01 : (byte)0x00;
    }

    private static byte[] Encode(string text)
    {
        var encoding = Encoding.GetEncoding(1252);
        return encoding.GetBytes(text);
    }
}

