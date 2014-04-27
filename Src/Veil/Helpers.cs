using System.IO;

namespace Veil
{
    internal static class Helpers
    {
        public static void HtmlEncode(TextWriter writer, string value)
        {
            var chars = value.ToCharArray();
            var startIndex = 0;
            var length = 0;
            for (int i = 0, j = chars.Length; i < j; i++)
            {
                if (chars[i] == '&' || chars[i] == '<' || chars[i] == '>' || chars[i] == '"' || chars[i] == '\'')
                {
                    if (length > 0) writer.Write(chars, startIndex, length);
                    startIndex = i + 1;
                    length = 0;

                    switch (chars[i])
                    {
                        case '&': writer.Write("&amp;"); break;
                        case '<': writer.Write("&lt;"); break;
                        case '>': writer.Write("&gt;"); break;
                        case '"': writer.Write("&quot;"); break;
                        case '\'': writer.Write("&#39;"); break;
                        default: break;
                    }
                    continue;
                }

                length++;
            }
            if (length > 0) writer.Write(chars, startIndex, length);
        }
    }
}