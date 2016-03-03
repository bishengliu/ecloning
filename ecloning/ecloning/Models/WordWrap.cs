using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace ecloning.Models
{
    public class WordWrap
    {
        public static string Wrap(string text, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
            { return text; }
            if (text != null)
            {
                // Parse each line of text
                for (pos = 0; pos < text.Length; pos = next)
                {
                    // Find end of line
                    int eol = text.IndexOf(Environment.NewLine, pos);
                    if (eol == -1)
                        next = eol = text.Length;
                    else
                        next = eol + Environment.NewLine.Length;

                    // Copy this line of text, breaking into smaller lines as needed
                    if (eol > pos)
                    {
                        do
                        {
                            int len = eol - pos;
                            if (len > width)
                                len = BreakLine(text, pos, width);
                            sb.Append(text, pos, len);
                            //sb.Append(Environment.NewLine);
                            sb.Append("<br />");

                            // Trim whitespace following break
                            pos += len;
                            while (pos < eol && Char.IsWhiteSpace(text[pos]))
                                pos++;
                        } while (eol > pos);
                    }
                    else sb.Append(Environment.NewLine); // Empty line
                }
            }
            return sb.ToString();
        }

        private static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                i--;

            // If no whitespace found, break at maximum length
            if (i < 0)
                return max;

            // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                i--;

            // Return length of text before whitespace
            return i + 1;
        }
    }
}