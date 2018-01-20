using System;
using System.Data;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

public class RequestUserInfo
{
    /// <summary>
    /// Определяет ip адрес пользователя
    /// </summary>
    public static string IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
    /// <summary>
    /// Определяет login пользователя
    /// </summary>
    public static string Login = HttpContext.Current.Request.ServerVariables["LOGON_USER"];

    /// <summary>
    /// Возвращает заначение парамметра из Cookies
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public static string CookiesValue(string Name)
    {
        Name = Name.ToLower();
        HttpCookie MyCookie = HttpContext.Current.Request.Cookies[Name];
        string Value = (MyCookie != null) ? Value = HttpUtility.UrlDecode(MyCookie.Value, Encoding.UTF8) : String.Empty;

        switch (Name)
        {
            #region SearchWord
            case "searchword":
                Value = Value.Replace(",", "").Replace(".", "").Replace("_", "").Replace("!", "").Replace("?", "").Replace("\"", "");
                break;
            #endregion
            #region По умолчанию
            default:
                break;
            #endregion
        }

        return Value;
    }

    /// <summary>
    /// Возвращает заначение парамметра из Cookies
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Val"></param>
    /// <returns></returns>
    public static void AddCookies(string Name, string Val)
    {
        HttpCookie MyCookie = new HttpCookie(Name);
        MyCookie.Value = HttpUtility.UrlEncode(Val, Encoding.UTF8);
        HttpContext.Current.Response.Cookies.Add(MyCookie);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Val"></param>
    /// <param name="Expires"></param>
    public static void AddCookies(string Name, string Val, int Expires)
    {
        HttpCookie MyCookie = new HttpCookie(Name);
        MyCookie.Expires = DateTime.Now.AddDays(Expires);
        MyCookie.Value = HttpUtility.UrlEncode(Val, Encoding.UTF8);
        HttpContext.Current.Response.Cookies.Add(MyCookie);
    }
    
    public static string Geolocation()
    {
        string Result = "Not Found";
        DataTable DT = new DataTable();

        //Создаем WebRequest с текущим IP
        WebRequest _objWebRequest = WebRequest.Create("http://ipgeobase.ru:7020/geo/?ip=" + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString());
        WebProxy _objWebProxy = new WebProxy("http://ipgeobase.ru:7020/geo/?ip=" + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), true);
        //WebRequest _objWebRequest = WebRequest.Create("http://api.ipinfodb.com/v3/ip-city/?format=xml&key=6b8a444a339e73189248ba76db01efd6d60233cef1ea9c8397743ed28bd105a3&ip=" + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString() + "&licenseKey=0");
        //WebProxy _objWebProxy = new WebProxy("http://api.ipinfodb.com/v3/ip-city/?format=xml&key=6b8a444a339e73189248ba76db01efd6d60233cef1ea9c8397743ed28bd105a3&ip=" + HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString() + "&licenseKey=0", true);

        _objWebRequest.Proxy = _objWebProxy;

        //Устанавливаем таймаут с секундах для WebRequest
        _objWebRequest.Timeout = 2000;

        try
        {
            WebResponse _objWebResponse = _objWebRequest.GetResponse();
            //Считываем Response в XMLTextReader
            XmlTextReader _objXmlTextReader = new XmlTextReader(_objWebResponse.GetResponseStream());

            DataSet _objDataSet = new DataSet();
            _objDataSet.ReadXml(_objXmlTextReader);

            DT = _objDataSet.Tables[0];

            if (DT.Rows.Count > 0) Result = DT.Rows[0]["city"].ToString().ToUpper();
        }
        catch { }

        return Result;
    }
}
