using System;
using System.Text;
using System.Text.RegularExpressions;


    /// <summary>
    /// Класс для редактированиея текста
    /// </summary>
public class ShortText
{
    /// <summary>
    /// массив тегами исключениями
    /// </summary>
    string[] exceptionTegArray = null;

    /// <summary>
    /// используется только один рисунок
    /// </summary>
    bool oneImg = true;

    bool leaveImg = false;

    /// <summary>
    /// Свойство разрешает использовать изображения. Разрешается выводить одно изображение. По умолчанию false
    /// </summary>
    public bool LeaveImg
    {
        get { return leaveImg; }
        set { leaveImg = value; }
    }

    string exceptionTeg = "a";

    /// <summary>
    /// Свойство принимает или возвращает теги которые останутся после очистки. Применяется только для HtmlText. 
    /// Теги пишутся через запятую.
    /// По умолчанию a,img
    /// </summary>
    public string ExceptionTeg
    {
        get { return exceptionTeg; }
        set { exceptionTeg = value; }
    }

    string text = String.Empty;
    /// <summary>
    /// Стандартный конструктор
    /// </summary>
    /// <param name="text">Строка для редактирования</param>
    public ShortText(string text)
    {
        this.text = text;
    }

    /// <summary>
    /// Перегружанный конструктор
    /// </summary>
    /// <param name="text">Строка для редактирования</param>
    /// <param name="limit">Кол-во выводимых символов</param>
    public ShortText(string text, int limit)
        : this(text)
    {
        this.limit = limit;
    }

    /// <summary>
    /// Перегружанный конструктор
    /// </summary>
    /// <param name="text">Строка для редактирования</param>
    /// <param name="limit">Кол-во выводимых символов</param>
    /// <param name="searchWord">Слово для поиска</param>
    public ShortText(string text, int limit, string searchWord)
        : this(text, limit)
    {
        this.searchWord = searchWord;
    }

    /// <summary>
    /// Перегружанный конструктор
    /// </summary>
    /// <param name="text">Строка для редактирования</param>
    /// <param name="searchWord">Слово для поиска</param>
    public ShortText(string text, string searchWord)
        : this(text)
    {
        this.searchWord = searchWord;
    }

    string htmlText = String.Empty;

    /// <summary>
    /// Возвращает текст с Html кодом
    /// </summary>
    [Obsolete("Недоработанное свойство")]
    public string HtmlText
    {
        get
        {
            htmlText = HTMLString(text);
            if (!String.IsNullOrEmpty(searchWord) || limit != -1)
                htmlText = HTMLStringShort(htmlText, limit, searchWord);
            return htmlText;
        }
    }

    string txtText = String.Empty;

    /// <summary>
    /// Возвращает текст без Html кода
    /// </summary>
    public string TxtText
    {
        get
        {
            txtText = SimpleString(text);
            if (!String.IsNullOrEmpty(searchWord) || limit != -1)
                txtText = SimpleStringShort(txtText, limit, searchWord);
            return txtText;
        }
    }

    string searchWord = String.Empty;
    /// <summary>
    /// Свойство принимает и возвращает слово для поиска
    /// </summary>
    public string SearchWord
    {
        get { return searchWord; }
        set { searchWord = value; }
    }

    int limit = -1;
    /// <summary>
    /// Свойство принимает или возвращает кол-во символов при выведении информации  
    /// </summary>
    public int Limit
    {
        get { return limit; }
        set { limit = value; }
    }

    /// <summary>
    /// Очищает текст от тегов
    /// </summary>
    /// <returns>Отредактированную строку</returns>
    string SimpleString(string text)
    {
        if (String.IsNullOrEmpty(text))
            return String.Empty;

        StringBuilder build = new StringBuilder(text);
        Regex r = new Regex(@"(</?\w+\s*[^<]*\s?/?>)|(<\?\w+:\s*[^<]*/>)|(\&nbsp;)", RegexOptions.IgnoreCase);
        MatchCollection mc = r.Matches(text);

        foreach (Match m in mc)
        {
            build.Replace(m.Value, String.Empty);
        }

        return build.ToString();
    }

    string searchWordConteiner = "strong";

    /// <summary>
    /// Свойство принимает или возвращает название тега в который надо заключить найденное слово. По умолчанию strong
    /// </summary>
    public string SearchWordConteiner
    {
        get { return searchWordConteiner; }
        set { searchWordConteiner = value; }
    }

    /// <summary>
    /// Метод сокращает и выделяет текст
    /// </summary>
    /// <param name="text">Строка редактирования</param>
    /// <param name="limit">Кол-во выводимых символов</param>
    /// <param name="searchWord">Слово для поиска</param>
    /// <returns>Отредактированный текст</returns>
    string SimpleStringShort(string text, int limit, string searchWord)
    {
        if (String.IsNullOrEmpty(text))
            return String.Empty;

        Regex r = null;
        MatchCollection mc = null;

        #region лимит
        if (limit != -1 && text.Length > limit)
        {
            if (!String.IsNullOrEmpty(searchWord))
            {
                r = new Regex(@"[\S\s]{30}" + searchWord + @"[\S\s]*", RegexOptions.IgnoreCase);
                if (r.IsMatch(text))
                {
                    Match m = r.Match(text);
                    if (m.Length > limit)
                        text = text.Substring(m.Index, limit);
                    else
                    {
                        int add = limit - m.Length + 30;
                        r = new Regex(@"[\S\s]{" + add + @"}" + searchWord + @"[\S\s]*", RegexOptions.IgnoreCase);
                        m = r.Match(text);
                        text = text.Substring(m.Index, limit);
                    }

                    text = "..." + text + "...";
                }
                else
                {
                    text = text.Substring(0, limit);
                    text = text + "...";
                }
            }
            else
            {
                text = text.Substring(0, limit);
                text = text + "...";
            }
        }
        #endregion

        StringBuilder build = new StringBuilder(text);
        #region поиск
        if (!String.IsNullOrEmpty(searchWord))
        {
            r = new Regex(@"[а-я0-9a-z]*" + searchWord + @"[а-я0-9a-z]*", RegexOptions.IgnoreCase);
            mc = r.Matches(text);

            int replaceStart = 0;
            int replaceLength = 0;
            int koef = 0;

            foreach (Match m in mc)
            {
                replaceLength = m.Length;
                replaceStart = m.Index + koef;
                build.Replace(m.Value, "<" + searchWordConteiner + ">" + m.Value + "</" + searchWordConteiner + ">", replaceStart, replaceLength);
                koef = build.Length - text.Length;
            }
        }
        #endregion

        return build.ToString();
    }

    /// <summary>
    /// Метод выборочной очистки html кода
    /// </summary>
    /// <param name="htmlText">Строка редактирования</param>
    /// <returns>Отредактированный текст</returns>
    string HTMLString(string htmlText)
    {
        if (String.IsNullOrEmpty(text))
            return String.Empty;

        exceptionTeg += leaveImg ? ",img" : String.Empty;
        exceptionTegArray = exceptionTeg.Split(',');
        StringBuilder build = new StringBuilder(text);
        Regex r = new Regex(@"(</?\w+\s*[^<]*\s?/?>)|(<\?\w+:\s*[^<]*/>)", RegexOptions.IgnoreCase);
        MatchCollection mc = r.Matches(text);

        foreach (Match m in mc)
        {
            if (TegName(m.Value))
                continue;

            build.Replace(m.Value, String.Empty);
        }

        return secondHTMLText(build.ToString());
    }

    /// <summary>
    /// Метод возвращает имя тега
    /// </summary>
    /// <param name="str">Строка редактирования</param>
    /// <returns>true-тег присутствует в исключении</returns>
    bool TegName(string str)
    {
        if (String.IsNullOrEmpty(exceptionTeg))
            return false;

        int i = 0;
        string nameTag = String.Empty;
        if (str.StartsWith("</"))
        {
            i = str.IndexOf(' ');
            if (i == -1)
            {
                i = str.IndexOf('>');
                if (i == -1)
                    return false;
            }

            nameTag = str.Substring(2, i - 2);
        }
        else if (str.StartsWith("<"))
        {
            i = str.IndexOf(' ');
            if (i == -1)
            {
                i = str.IndexOf('>');
                if (i == -1)
                    return false;
            }

            nameTag = str.Substring(1, i - 1);
        }
        else
            return false;

        for (i = 0; i < exceptionTegArray.Length; i++)
        {
            if (exceptionTegArray[i] == nameTag)
            {
                if (exceptionTegArray[i] == "img")
                {
                    if (!oneImg)
                    {
                        return false;
                    }
                    oneImg = false;
                }
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Вторичная проверка текста. Используется для HTML
    /// </summary>
    /// <param name="htmlText">Строка редактирования</param>
    /// <returns>Отредактированный текст</returns>
    string secondHTMLText(string htmlText)
    {
        if (leaveImg)
        {
            StringBuilder build = new StringBuilder(htmlText);
            Regex r = new Regex(@"</?img\s*[^<]*\s?/?>", RegexOptions.IgnoreCase);
            if (r.IsMatch(htmlText))
            {
                Match m = r.Match(htmlText);
                int _x = m.Value.IndexOf("src=\"");
                if (_x != -1)
                {
                    int _y = m.Value.IndexOf("\"", _x + 5);
                    build.Replace(m.Value, String.Empty);
                    build.Append("<img class=\"ShortTextImg\" " + m.Value.Substring(_x, _y - _x) + "\">");
                }
                else
                    build.Replace(m.Value, String.Empty);
            }
            return build.ToString();
        }

        return htmlText;
    }

    /// <summary>
    /// Метод сокращает и выделяет текст с HTML кодом
    /// </summary>
    /// <param name="text">Строка редактирования</param>
    /// <param name="limit">Кол-во выводимых символов</param>
    /// <param name="searchWord">Слово для поиска</param>
    /// <returns>Отредактированный текст</returns>
    string HTMLStringShort(string text, int limit, string searchWord)
    {
        if (String.IsNullOrEmpty(text))
            return String.Empty;

        Regex r = null;
        MatchCollection mc = null;

        // чистый текст без тегов
        string clearTxt = SimpleString(text);

        #region лимит
        if (limit != -1 && clearTxt.Length > limit)
        {
            if (!String.IsNullOrEmpty(searchWord))
            {
                r = new Regex(@"[^<>]{0,60}" + searchWord + @"[\S\s]*", RegexOptions.IgnoreCase);
                if (r.IsMatch(text))
                {
                    Match m = r.Match(text);
                    if (m.Length > limit)
                    {
                        text = Analiz(text, clearTxt, 150);
                    }
                    else
                    {
                        int add = limit - m.Length + 30;
                        r = new Regex(@"[\S\s]{" + add + @"}" + searchWord + @"[\S\s]*", RegexOptions.IgnoreCase);
                        m = r.Match(text);
                        text = text.Substring(m.Index, limit);
                    }

                    text = "..." + text + "...";
                }
                else
                {
                    text = text.Substring(0, limit);
                    text = text + "...";
                }
            }
            else
            {
                text = text.Substring(0, limit);
                text = text + "...";
            }
        }
        #endregion

        StringBuilder build = new StringBuilder(text);
        #region поиск
        if (!String.IsNullOrEmpty(searchWord))
        {
            r = new Regex(@"[а-я0-9a-z]*" + searchWord + @"[а-я0-9a-z]*", RegexOptions.IgnoreCase);
            mc = r.Matches(text);

            int replaceStart = 0;
            int replaceLength = 0;
            int koef = 0;

            foreach (Match m in mc)
            {
                replaceLength = m.Length;
                replaceStart = m.Index + koef;
                build.Replace(m.Value, "<" + searchWordConteiner + ">" + m.Value + "</" + searchWordConteiner + ">", replaceStart, replaceLength);
                koef = build.Length - text.Length;
            }
        }
        #endregion

        return build.ToString();
    }

    /// <summary>
    /// Недоработан
    /// </summary>
    /// <param name="originalStr">Недоработан</param>
    /// <param name="clearStr">Недоработан</param>
    /// <param name="indexStart">Недоработан</param>
    /// <returns>Недоработан</returns>
    string Analiz(string originalStr, string clearStr, int indexStart)
    {
        int i = originalStr.IndexOf("<", 150);
        int j = originalStr.IndexOf(">", 150);
        // тегов не обнаружено
        if (i == -1 && j == -1)
            return originalStr;

        return String.Empty;
    }
}