using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.Exporters
{
    /// <summary>
    /// Represents a contiguous run of characters sharing the same formatting.
    /// </summary>
    public class FormattedRun
    {
        public string Text { get; set; } = "";
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Underline { get; set; }
        public bool Strikeout { get; set; }
        public Color ForeColor { get; set; } = Color.Empty;
        public Color BackColor { get; set; } = Color.Empty;
    }

    /// <summary>
    /// Utility class that parses RTF content and converts it to various output formats.
    /// Uses a RichTextBox to parse the RTF and extract character-level formatting.
    /// </summary>
    public static class RtfFormatConverter
    {
        /// <summary>
        /// Extracts formatted runs from RTF content.
        /// </summary>
        public static FormattedRun[] GetFormattedRuns(string rtf)
        {
            using var rtfBox = new RichTextBox();
            rtfBox.Rtf = rtf;

            if (rtfBox.TextLength == 0)
                return [];

            var runs = new System.Collections.Generic.List<FormattedRun>();
            FormattedRun? current = null;

            for (int i = 0; i < rtfBox.TextLength; i++)
            {
                rtfBox.Select(i, 1);
                var font = rtfBox.SelectionFont ?? rtfBox.Font;
                var foreColor = rtfBox.SelectionColor;
                var backColor = rtfBox.SelectionBackColor;
                char ch = rtfBox.Text[i];

                // Normalize vertical tab (0x0B, RTF \line) to newline and skip other invalid control chars
                if (ch == '\v') ch = '\n';
                if (char.IsControl(ch) && ch != '\n' && ch != '\r' && ch != '\t')
                    continue;

                bool sameRun = current != null
                    && current.Bold == font.Bold
                    && current.Italic == font.Italic
                    && current.Underline == font.Underline
                    && current.Strikeout == font.Strikeout
                    && current.ForeColor == foreColor
                    && current.BackColor == backColor;

                if (sameRun)
                {
                    current!.Text += ch;
                }
                else
                {
                    current = new FormattedRun
                    {
                        Text = ch.ToString(),
                        Bold = font.Bold,
                        Italic = font.Italic,
                        Underline = font.Underline,
                        Strikeout = font.Strikeout,
                        ForeColor = foreColor,
                        BackColor = backColor
                    };
                    runs.Add(current);
                }
            }

            return [.. runs];
        }

        /// <summary>
        /// Converts RTF to inline HTML spans preserving bold, italic, underline, strikethrough, and colors.
        /// </summary>
        public static string ToHtml(string rtf)
        {
            var runs = GetFormattedRuns(rtf);
            if (runs.Length == 0) return "";

            var sb = new StringBuilder();
            foreach (var run in runs)
            {
                string encoded = HtmlEncode(run.Text).Replace("\n", "<br>");

                // Build inline style for colors
                var style = new StringBuilder();
                if (!run.ForeColor.IsEmpty && run.ForeColor != Color.Black && run.ForeColor != SystemColors.WindowText)
                    style.Append($"color:#{run.ForeColor.R:X2}{run.ForeColor.G:X2}{run.ForeColor.B:X2};");
                if (!run.BackColor.IsEmpty && run.BackColor != Color.White && run.BackColor != SystemColors.Window)
                    style.Append($"background-color:#{run.BackColor.R:X2}{run.BackColor.G:X2}{run.BackColor.B:X2};");

                string text = encoded;
                if (run.Bold) text = $"<strong>{text}</strong>";
                if (run.Italic) text = $"<em>{text}</em>";
                if (run.Underline) text = $"<u>{text}</u>";
                if (run.Strikeout) text = $"<s>{text}</s>";

                if (style.Length > 0)
                    text = $"<span style=\"{style}\">{text}</span>";

                sb.Append(text);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts RTF to Markdown preserving bold, italic, strikethrough.
        /// Underline and colors are not supported in standard Markdown.
        /// </summary>
        public static string ToMarkdown(string rtf)
        {
            var runs = GetFormattedRuns(rtf);
            if (runs.Length == 0) return "";

            var sb = new StringBuilder();
            foreach (var run in runs)
            {
                // Split on newlines so formatting markers don't span across lines;
                // most markdown parsers require inline markers on the same line
                string[] lines = run.Text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i > 0) sb.Append('\n');

                    string text = lines[i];
                    if (text.Length == 0) continue;

                    if (run.Strikeout) text = $"~~{text}~~";
                    if (run.Bold && run.Italic) text = $"***{text}***";
                    else if (run.Bold) text = $"**{text}**";
                    else if (run.Italic) text = $"*{text}*";

                    sb.Append(text);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Sanitizes text for export by replacing vertical tabs (0x0B) with newlines
        /// and stripping other invalid control characters.
        /// </summary>
        public static string SanitizeForExport(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var sb = new StringBuilder(text.Length);
            foreach (char ch in text)
            {
                if (ch == '\v')
                    sb.Append('\n');
                else if (char.IsControl(ch) && ch != '\n' && ch != '\r' && ch != '\t')
                    continue;
                else
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns true if the RTF contains any formatting beyond plain text.
        /// </summary>
        public static bool HasFormatting(string? rtf)
        {
            if (string.IsNullOrEmpty(rtf)) return false;

            var runs = GetFormattedRuns(rtf);
            foreach (var run in runs)
            {
                if (run.Bold || run.Italic || run.Underline || run.Strikeout)
                    return true;
                if (!run.ForeColor.IsEmpty && run.ForeColor != Color.Black && run.ForeColor != SystemColors.WindowText)
                    return true;
                if (!run.BackColor.IsEmpty && run.BackColor != Color.White && run.BackColor != SystemColors.Window)
                    return true;
            }
            return false;
        }

        private static string HtmlEncode(string value) =>
            value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("\v", "\n");
    }
}
