using System;
using System.Net.Mail;
using System.Net;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data;
using Disly.Areas.Admin.Models;
using cms.dbase;
using System.Text.RegularExpressions;

/// <summary>
/// Сервис отправки писем
/// </summary>
public class Mailer
{

    /// <summary>
    /// Контекст доступа к базе данных
    /// </summary>
    //protected dbRepository _repository { get; private set; }
    //protected SettingsViewModel model = new SettingsViewModel();

    protected string server = String.Empty;
    protected int port = 25;
    protected bool ssl = false;

    protected string maillist = String.Empty;
    protected string mailfrom = String.Empty;
    protected string mailname = String.Empty;
    protected string password = String.Empty;

    protected string theme = "Обратная связь";
    protected string text = String.Empty;
    protected string styles = String.Empty;

    protected string attechments = String.Empty;
    protected string results = String.Empty;
    protected string dublicate = String.Empty;

    public string MailTo
    {
        set { maillist = value; }
        get { return maillist; }
    }

    public string Theme
    {
        set { theme = value; }
        get { return theme; }
    }

    public string Text
    {
        set { text = value; }
        get { return text; }
    }


    public string MailFrom
    {
        set { mailfrom = value; }
        get { return mailfrom; }
    }

    public string MailName
    {
        set { mailname = value; }
        get { return mailname; }
    }

    public string Server
    {
        set { server = value; }
        get { return server; }
    }
    public int Port
    {
        set { port = value; }
        get { return port; }
    }

    public string Password
    {
        set { password = value; }
        get { return password; }
    }

    public bool isSsl
    {
        set { ssl = value; }
        get { return ssl; }
    }

    public string Attachments
    {
        set { attechments = value; }
        get { return attechments; }
    }

    public String Dublicate
    {
        set { dublicate = value; }
        get { return dublicate; }
    }

    public void MailFromSettings()
    {
        //    try {

        //        _repository = new dbRepository("cmsdbConnection");
        //        SettingsViewModel model = new SettingsViewModel()
        //        {
        //            siteSettings = _repository.getCmsSettings()
        //        };
        //        string MailServer = model.siteSettings.MailServer;
        //        string MailFrom = model.siteSettings.MailFrom;
        //        string MailFromAlias = model.siteSettings.MailFromAlias;
        //        MailFromAlias = (MailFromAlias != String.Empty) ? MailFromAlias : Settings.MailAdresName;
        //        string MailPass = model.siteSettings.MailPass;
        //        string MailTo = model.siteSettings.MailTo;

        //        if (MailFrom == null || MailServer == null || MailPass == null)
        //        {
        //            MailFrom = Settings.MailFrom;
        //            MailServer = Settings.mailServer;
        //            MailPass = Settings.mailPWD;
        //        }


        //        if (mailfrom == String.Empty || server == String.Empty || password == String.Empty)
        //        {
        //            mailfrom = MailFrom;
        //            server = MailServer;
        //            password = MailPass;
        //        }
        //        if (mailname == String.Empty) mailname = MailFromAlias;
        //        if (maillist == String.Empty) maillist = MailTo;


        //        if (mailfrom == null || server == null || password == null)
        //        {
        //            mailname = Settings.MailAdresName;
        //            mailfrom = Settings.MailFrom;
        //            server = Settings.mailServer;
        //            password = Settings.mailPWD;
        //        }

        //}
        //    catch {
        //        mailname = Settings.MailAdresName;
        //        mailfrom = Settings.MailFrom;
        //        server = Settings.mailServer;
        //        password = Settings.mailPWD;
        //    }      

        if (mailfrom == String.Empty || server == String.Empty || password == String.Empty)
        {
            server = Settings.mailServer;
            port = Settings.mailServerPort;
            ssl = Settings.mailServerSSL;
            mailname = Settings.mailAddresName;
            mailfrom = Settings.mailUser;
            server = Settings.mailServer;
            password = Settings.mailPass;
        }
    }


    public void MailToSettings()
    {
        //_repository = new dbRepository("cmsdbConnection");
        //Guid GeneralUser= Guid.Parse("00000000-0000-0000-0000-000000000000");

        //UsersViewModel model = new UsersViewModel()
        //{
        //    User = _repository.getUser(GeneralUser)
        //};

        //if (model.User.C_EMail.ToString() != String.Empty)
        //    maillist = maillist + ";" + model.User.C_EMail.ToString();

        if (maillist == string.Empty) maillist = Settings.mailTo;
    }

    public string SendMail()
    {
        MailFromSettings();

        //Авторизация на SMTP сервере
        SmtpClient Smtp = new SmtpClient(server, port);
        Smtp.EnableSsl = ssl;
        Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        Smtp.UseDefaultCredentials = false;
        Smtp.Credentials = new NetworkCredential(mailfrom, password);

        // Формирование сообщения

        MailMessage _Message = new MailMessage();
        _Message.From = new MailAddress(mailfrom, mailname);
        if (dublicate != String.Empty) maillist += ";" + dublicate;
        string[] MailList = maillist.Split(';');
        Regex regex = new Regex(@"\b[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}\b");

        foreach (string adress in MailList)
        {
            MatchCollection normail = regex.Matches(adress);
            if (normail.Count > 0) _Message.To.Add(new MailAddress(adress));
        }

        _Message.Subject = theme;
        _Message.BodyEncoding = System.Text.Encoding.UTF8;
        _Message.IsBodyHtml = true;
        _Message.Body = "<DOCTYPE html><html><head></head><body>" + text + "</body></html>";

        if (Attachments != string.Empty)
        {
            _Message.Attachments.Add(new Attachment(Attachments));
        }

        try
        {
            Smtp.Send(_Message);//отправка
            results = "Сообщение отправлено";
        }
        catch (System.Net.WebException)
        {
            throw new Exception("Ошибка при отправке");
            //results = "Ошибка при отправке";
        }

        return results;
    }

}
