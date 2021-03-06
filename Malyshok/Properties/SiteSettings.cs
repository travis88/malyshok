﻿using System;
using System.Configuration;

public class Settings
{
    //public const string PrevUrl = "~/";
    //public static bool DebugInfo = Convert.ToBoolean(ConfigurationManager.AppSettings["DebugInfo"]);

    public static string SiteTitle = ConfigurationManager.AppSettings["SiteTitle"];
    public static string SiteDesc = ConfigurationManager.AppSettings["SiteDesc"];
    public static string BaseURL = ConfigurationManager.AppSettings["BaseURL"];

    public static string CaptchaKey = ConfigurationManager.AppSettings["captchaKey"];
    public static string SecretKey = ConfigurationManager.AppSettings["secretKey"];

    public static string UserFiles = ConfigurationManager.AppSettings["UserFiles"];
    public static string BannersDir = ConfigurationManager.AppSettings["BannersDir"];
    public static string LogoDir = ConfigurationManager.AppSettings["LogoDir"];
    public static string OrgDir = ConfigurationManager.AppSettings["OrgDir"];
    public static string SiteMapDir = ConfigurationManager.AppSettings["SiteMapDir"];
    public static string ProdContent = ConfigurationManager.AppSettings["ProdContent"];
    public static string Certificates = ConfigurationManager.AppSettings["Certificates"];

    public static  int ProdListSize = Convert.ToInt32(ReadAppSetting("ProdListSize"));
    public static int NoveltiesDay = Convert.ToInt32(ReadAppSetting("NoveltiesDay"));

    public static string fbApp = ConfigurationManager.AppSettings["FacebookApp"];
    public static string fbAppServKey = ConfigurationManager.AppSettings["FacebookServKey"];

    public static string vkApp = ConfigurationManager.AppSettings["vkApp"];
    public static string vkAppKey = ConfigurationManager.AppSettings["vkAppKey"];
    public static string vkAppServKey = ConfigurationManager.AppSettings["vkAppServKey"];


    public static string EventsDir = ReadAppSetting("EventsDir");
    public static string MaterialsDir = ReadAppSetting("MaterialsDir");
    public static string ProductsDir = ReadAppSetting("ProductsDir");
    public static string FeedbacksDir = ReadAppSetting("FeedbacksDir");
    public static string PhotoDir = ReadAppSetting("PhotoDir");
    public static string ImportDir = ReadAppSetting("ImportDir");
    public static string OrdersDir = ReadAppSetting("OrdersDir");

    public static string mailServer = ReadAppSetting("MailServer");

    public static int mailServerPort = Convert.ToInt32(ReadAppSetting("MailServerPort"));
    public static bool mailServerSSL = Convert.ToBoolean(ReadAppSetting("MailServerSSL"));

    public static string mailUser = ReadAppSetting("MailFrom");
    public static string mailPass = ReadAppSetting("MailPass");
    public static string mailEncoding = ReadAppSetting("MailEncoding");
    public static string mailAddresName = ReadAppSetting("MailAdresName");
    public static string mailTo = ReadAppSetting("MailTo");

    public static string MedCap = ConfigurationManager.AppSettings["MedCap"];
    public static string Quote = ConfigurationManager.AppSettings["Quote"];
    public static string Concept = ConfigurationManager.AppSettings["Concept"];
    public static string Coordination = ConfigurationManager.AppSettings["Coordination"];

    public static string DocTypes = ReadAppSetting("DocTypes");//ConfigurationManager.AppSettings["MaterialPreviewImgSize"];
    public static string PicTypes = ReadAppSetting("PicTypes");//ConfigurationManager.AppSettings["MaterialPreviewImgSize"];
    public static string MaterialPreviewImgSize = ReadAppSetting("MaterialPreviewImgSize");//ConfigurationManager.AppSettings["MaterialPreviewImgSize"];
    public static string MaterialContentImgSize = ReadAppSetting("MaterialContentImgSize"); //ConfigurationManager.AppSettings["MaterialContentImgSize"];

    public static string GalleryPreviewImgSize = ReadAppSetting("GalleryPreviewImgSize"); //ConfigurationManager.AppSettings["GalleryPreviewImgSize"];
    public static string GalleryContentImgSize = ReadAppSetting("GalleryContentImgSize"); //ConfigurationManager.AppSettings["GalleryContentImgSize"];

    public static string HospitalReg = ConfigurationManager.AppSettings["HospitalReg"];
    public static string ScheduleReg = ConfigurationManager.AppSettings["ScheduleReg"];

    //Read AppSettings
    static string ReadAppSetting(string key)
    {
        string result = null;
        try
        {
            var appSettings = ConfigurationManager.AppSettings;
            result = appSettings[key] ?? ""; // "Not Found";
            return result;
        }
        catch (ConfigurationErrorsException)
        {
            throw new Exception("Error reading app settings" + key);
        }
    }

}