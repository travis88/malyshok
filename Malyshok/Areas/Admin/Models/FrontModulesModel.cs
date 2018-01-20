using System;

namespace Disly.Areas.Admin.Models
{
    public class FrontModule : CoreViewModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string type { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public string url { get; set; }
    }
}
