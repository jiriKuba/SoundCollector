using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoundCollector.Utils
{
    class TextUtils
    {
        public static String RemoveDiacritics(String input)
        {
            if (String.IsNullOrEmpty(input))
                return input;

            //String normalized = input.Normalize(NormalizationForm.FormKD);
            //Encoding removal = Encoding.GetEncoding(Encoding.ASCII.CodePage,
            //                                        new EncoderReplacementFallback(""),
            //                                        new DecoderReplacementFallback(""));
            //Byte[] bytes = removal.GetBytes(normalized);
            //return Encoding.ASCII.GetString(bytes);

            String decomposed = input.Normalize(NormalizationForm.FormD);
            IEnumerable<Char> filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            return new String(filtered.ToArray());
        }
    }
}
