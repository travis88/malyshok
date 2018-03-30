using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Import.Core.Services
{
    /// <summary>
    /// Класс для транслитерации 
    /// </summary>
    public class Transliteration
    {
        /// <summary>
        /// Функция транслитерации русского текста
        /// </summary>
        /// <param name="source">исходная строка</param>
        /// <returns></returns>
        public static string Translit(string source)
        {
            Dictionary<string, string> words = new Dictionary<string, string>
            {
                { "а", "a" },
                { "б", "b" },
                { "в", "v" },
                { "г", "g" },
                { "д", "d" },
                { "е", "e" },
                { "ё", "yo" },
                { "ж", "zh" },
                { "з", "z" },
                { "и", "i" },
                { "й", "j" },
                { "к", "k" },
                { "л", "l" },
                { "м", "m" },
                { "н", "n" },
                { "о", "o" },
                { "п", "p" },
                { "р", "r" },
                { "с", "s" },
                { "т", "t" },
                { "у", "u" },
                { "ф", "f" },
                { "х", "h" },
                { "ц", "c" },
                { "ч", "ch" },
                { "ш", "sh" },
                { "щ", "sch" },
                { "ъ", "j" },
                { "ы", "i" },
                { "ь", "j" },
                { "э", "e" },
                { "ю", "yu" },
                { "я", "ya" },
                { "А", "A" },
                { "Б", "B" },
                { "В", "V" },
                { "Г", "G" },
                { "Д", "D" },
                { "Е", "E" },
                { "Ё", "Yo" },
                { "Ж", "Zh" },
                { "З", "Z" },
                { "И", "I" },
                { "Й", "J" },
                { "К", "K" },
                { "Л", "L" },
                { "М", "M" },
                { "Н", "N" },
                { "О", "O" },
                { "П", "P" },
                { "Р", "R" },
                { "С", "S" },
                { "Т", "T" },
                { "У", "U" },
                { "Ф", "F" },
                { "Х", "H" },
                { "Ц", "C" },
                { "Ч", "Ch" },
                { "Ш", "Sh" },
                { "Щ", "Sch" },
                { "Ъ", "J" },
                { "Ы", "I" },
                { "Ь", "J" },
                { "Э", "E" },
                { "Ю", "Yu" },
                { "Я", "Ya" },
                { " ", "-" },
                { "–", "-" },
                { ":", "-" },
                { "<", "(" },
                { ">", ")" },
                { "[", "(" },
                { "]", ")" },
                { "{", "(" },
                { "}", ")" },
                { "«", "" },
                { "»", "" },
                { "\"", "" },
                { "#", "" },
                { "%", "" },
                { "&", "" },
                { "@", "" },
                { "$", "" },
                { "'", "" },
                { "*", "" },
                { ",", "" },
                { ";", "" },
                { "=", "" },
                { "+", "" },
                { "!", "" },
                { "?", "" },
                { "^", "" },
                { "`", "" },
                { "|", "" },
                { ".", "" },
                { "/", "-" },
                { "Ă", "A" },
                { "ă", "a" },
                { "ÿ", "y" },
                { "ĕ", "e" }
            };

            Regex re = new Regex("[-]{2,}");
            Regex Re = new Regex("[_]{2,}");
            Regex StartRe = new Regex("^[-|_]{1,}");
            Regex EndRe = new Regex("[-|_]${1,}");

            if (source.Length > 50)
            {
                source = source.Substring(0, 50);
            }

            foreach (KeyValuePair<string, string> pair in words)
            {
                source = source.Replace(pair.Key, pair.Value);
            }
            source = re.Replace(source, "-");
            source = Re.Replace(source, "_");
            source = StartRe.Replace(source, "");
            source = EndRe.Replace(source, "");
            source = source.ToLower();
            return source;
        }
    }
}
