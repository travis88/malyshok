using cms.dbModel.entity;
using Disly.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Controllers
{
    public class TemplatesController : CoreController
    {

        public ActionResult AdminMenu(string viewName = "Templates/Menu/Default")
        {
            cmsMenuModel[] Menu = _cmsRepository.getCmsMenu(AccountInfo.id);

            return View(viewName, Menu);
        }

        public ActionResult Filtr(string Title, string Alias, string Icon, string Url, Catalog_list[] Items, string BtnName = "Добавить", string viewName = "Templates/Filtr/Default", bool readOnly = true)
        {
            string Link = Request.Url.PathAndQuery.ToLower();
            string nowValue = Request.QueryString[Alias];

            for (int i = 0; i < Items.Length; i++)
            {
                Items[i].link = addFiltrParam(Link, Alias.ToLower(), Items[i].value.ToLower());
                Items[i].url = Url.ToLower() + Items[i].value.ToLower() + "/";
                Items[i].selected = (nowValue == Items[i].value.ToLower()) ? "now" : String.Empty;
            }
            Link = addFiltrParam(Link, Alias.ToLower(), "");

            FiltrModel Model = new FiltrModel()
            {
                Title = Title,
                Alias = Alias,
                Icon = Icon,
                BtnName = BtnName,
                Link = Link,
                Url = Url.ToLower(),
                Items = Items,
                AccountGroup = AccountInfo.Group,
                ReadOnly = readOnly
            };

            return View(viewName, Model);
        }

        public ActionResult Pager(Pager Model, string startUrl, string viewName = "Templates/Pager/Default")
        {
            ViewBag.PagerSize = string.IsNullOrEmpty(Request.QueryString["size"]) ? Model.size.ToString() : Request.QueryString["size"];
            string qwer = String.Empty;

            int PagerLinkSize = 2;

            int FPage = (Model.page - PagerLinkSize < 1) ? 1 : Model.page - PagerLinkSize;
            int LPage = (Model.page + PagerLinkSize > Model.page_count) ? Model.page_count : Model.page + PagerLinkSize;

            if (String.IsNullOrEmpty(startUrl)) startUrl = Request.Url.Query;

            if (FPage > 1)
            {
                qwer = qwer + "1,";
            }
            if (FPage > 2)
            {
                qwer = qwer + "*,";
            }
            for (int i = FPage; i < LPage + 1; i++)
            {
                qwer = (@i < Model.page_count) ? qwer + @i + "," : qwer + @i;
            }
            if (LPage < Model.page_count - 1)
            {
                qwer = qwer + "*,";
            }
            if (Model.page_count > LPage)
            {
                qwer = qwer + @Model.page_count;
            }


            var viewModel = qwer.Split(',').
                Where(w => w != String.Empty).
                Select(s => new PagerModel
                {
                    text = (s == "*") ? "..." : s,
                    url = (s == "*") ? String.Empty : addFiltrParam(startUrl, "page", s),
                    isChecked = (s == Model.page.ToString())
                }).ToArray();

            if (viewModel.Length < 2) viewModel = null;

            return View(viewName, viewModel);
        }
    }
}