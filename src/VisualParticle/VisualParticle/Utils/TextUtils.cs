using Microsoft.Xna.Framework.Media;
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

            String decomposed = input.Normalize(NormalizationForm.FormD);
            IEnumerable<Char> filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            return new String(filtered.ToArray());
        }

        public static String GetSongName(Song s)
        {
            if (s == null)
                return "NULL";
            else
                return GetSongName(s.Artist.Name, s.Name);
        }

        public static String GetSongName(String artistName, String songName)
        {
            return String.Format("{0}{1}{2}", artistName, String.IsNullOrEmpty(artistName) ? String.Empty : " - ", songName);
        }
    }
}
