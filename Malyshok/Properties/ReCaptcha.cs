﻿using System;
using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// Клаcс для получения подтверждения Recaptcha
/// </summary>
public class ReCaptchaClass
{
    public static string Validate(string PrivateKey, string EncodedResponse)
    {
        var client = new System.Net.WebClient();

        //string PrivateKey = "6LcMEwYTAAAAAK2ErYn-tJ3W5TfJz_O_ahcXYTA4";

        var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, EncodedResponse));
        var captchaResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ReCaptchaClass>(GoogleReply);

        return captchaResponse.Success;
    }

    [JsonProperty("success")]
    public string Success
    {
        get { return m_Success; }
        set { m_Success = value; }
    }

    private string m_Success;
    [JsonProperty("error-codes")]
    public List<string> ErrorCodes
    {
        get { return m_ErrorCodes; }
        set { m_ErrorCodes = value; }
    }

    private List<string> m_ErrorCodes;
}
