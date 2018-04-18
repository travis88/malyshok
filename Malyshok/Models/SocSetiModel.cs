using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Models
{

    #region VK
    public class VkLoginModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string user_id { get; set; }
    }
    public class VkUserInfo
    {
        public List<Response> response { get; set; }
    }
    public class Response
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string nickname { get; set; }
        public string domain { get; set; }
        public catalog city { get; set; }
        public catalog country { get; set; }
        public string photo_200_orig { get; set; }
        public bool has_photo { get; set; }
        public int hidden { get; set; }
    }
    public class catalog
    {
        public int id { get; set; }
        public string title { get; set; }
    }
    #endregion
    #region Facebook
    public class FbLoginModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
    public class FbUserInfo
    {
        //public List<Response> response { get; set; }
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }
    #endregion
}
