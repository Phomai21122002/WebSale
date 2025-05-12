using System.Globalization;
using System.Text;

namespace WebSale.Extensions
{
    public static class RemoveDiacritics
    {
        public static string RemoveDiacriticsChar(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
