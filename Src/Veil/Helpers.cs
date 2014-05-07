using System.IO;

namespace Veil
{
    internal static class Helpers
    {
        public static void HtmlEncode(TextWriter writer, string value)
        {
            if (value == null || value.Length == 0) return;

            char[] chars = null;
            var startIndex = 0;
            var length = 0;
            for (int i = 0, j = value.Length; i < j; i++)
            {
                char c = value[i];
                if (c == '&' || c == '<' || c == '>' || c == '"' || c == '\'')
                {
                    if (chars == null) chars = value.ToCharArray();
                    if (length > 0) writer.Write(chars, startIndex, length);
                    startIndex = i + 1;
                    length = 0;

                    switch (c)
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
            if (startIndex == 0) writer.Write(value);
            else if (length > 0) writer.Write(chars, startIndex, length);
        }

        public static bool Boolify(object o)
        {
            if (o is bool) return (bool)o;
            return o != null;
        }

        public static object RuntimeBind(object model, string itemName)
        {
            var type = model.GetType();

            var property = type.GetProperty(itemName);
            if (property != null) return property.GetValue(model, null);

            var field = type.GetField(itemName);
            if (field != null) return field.GetValue(model);

            var dictionaryType = type.GetDictionaryTypeWithKey<string>();
            if (dictionaryType != null) return dictionaryType.GetMethod("get_Item").Invoke(model, new[] { itemName });

            return null;
        }
    }
}