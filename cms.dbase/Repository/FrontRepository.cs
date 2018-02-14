using cms.dbase.models;
using cms.dbModel;
using cms.dbModel.entity;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cms.dbase
{
    public class FrontRepository : abstract_FrontRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        private string _domain = string.Empty;

        /// <summary>
        /// Конструктор
        /// </summary>
        public FrontRepository()
        {
            _context = "defaultConnection";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }

        public FrontRepository(string ConnectionString, string DomainUrl)
        {
            _context = ConnectionString;
            _domain = (!string.IsNullOrEmpty(DomainUrl)) ? getSiteId(DomainUrl) : "";
            //_domain = "rkod";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }
        #region redirect methods
        public override SitesModel getSiteInfoByOldId(int Id)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess
                    .Where(w => w.c_alias.Equals(domain))
                    .Select(s => new SitesModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        LongTitle = s.c_name_long,
                        Alias = s.c_alias,
                        Adress = s.c_adress,
                        Phone = s.c_phone,
                        Fax = s.c_fax,
                        Email = s.c_email,
                        Site = s.c_url,
                        Worktime = s.c_worktime,
                        Logo = new Photo
                        {
                            Url = s.c_logo
                        },
                        ContentId = (Guid)s.f_content,
                        ContentType = (ContentLinkType)Enum.Parse(typeof(ContentLinkType), s.c_content_type, true),
                        Type = s.c_content_type,
                        Facebook = s.c_facebook,
                        Vk = s.c_vk,
                        Instagramm = s.c_instagramm,
                        Odnoklassniki = s.c_odnoklassniki,
                        Twitter = s.c_twitter,
                        Theme = s.c_theme,
                        BackGroundImg = new Photo
                        {
                            Url = s.c_background_img
                        }
                    });

                if (data.Any())
                    return data.SingleOrDefault();

                return null;
            }
        }

        /// <summary>
        /// Элемент карты сайта по старому линку с ид
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override SiteMapModel getSiteMapByOldId(int id)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps.Where(w => w.n_old_id == id);
                var data = query.Select(s => new SiteMapModel
                {
                    Title = s.c_title,
                    Text = s.c_text,
                    Alias = s.c_alias,
                    Path = s.c_path,
                    Id = s.id,
                    FrontSection = s.f_front_section,
                    ParentId = s.uui_parent
                });

                if (data.Any())
                    return data.SingleOrDefault();

                return null;
            }
        }
        /// <summary>
        /// Новость по старому линку с ид
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override MaterialsModel getMaterialsByOldId(int id)
        {
            //string domain = _domain;
            //using (var db = new CMSdb(_context))
            //{
            //    var query = db.content_materialss
            //                .Where(w => w.n_old_id == id);

            //    if (query.Any())
            //    {
            //        var material = query.Select(s => new MaterialsModel
            //        {
            //            Id = s.id,
            //            Title = s.c_title,
            //            Text = s.c_text,
            //            Alias = s.c_alias,
            //            Date = s.d_date,
            //            Year = s.n_year,
            //            Month = s.n_month,
            //            Day = s.n_day,
            //            PreviewImage = new Photo
            //            {
            //                Url = s.c_preview
            //            }
            //        }).SingleOrDefault();

            //        db.content_materialss
            //            .Where(w => w.id.Equals(material.Id))
            //            .Set(u => u.n_count_see, u => u.n_count_see + 1)
            //            .Update();

            //        return material;
            //    }

            //    return null;
            //}
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Получение идентификатора сайта
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override string getSiteId(string domain)
        {
            try
            {
                if (string.IsNullOrEmpty(domain))
                    throw new Exception("FrontRepository: getSiteId Domain is empty!");

                using (var db = new CMSdb(_context))
                {
                    var data = db.cms_sites_domainss
                        .Where(w => w.c_domain == domain);

                    if (data.Any())
                    {
                        //Может быть найдено несколько записей по разным доменам, но ссылаются на один сайт
                        var _domain = data.FirstOrDefault();
                        return _domain.f_site;
                    }

                    throw new Exception("FrontRepository: getSiteId Domain '" + domain + "' was not found!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("FrontRepository: getSiteId Domain '" + domain + "' непредвиденная ошибка!" + ex.Message);
            }
        }

        /// <summary>
        /// Получение вьюхи
        /// </summary>
        /// <param name="siteSection">Секция</param>
        /// <returns></returns>
        public override string getView(string siteSection) //string siteId,
        {
            string siteId = _domain;
            using (var db = new CMSdb(_context))
            {
                string ViewPath = "~/Error/404/";

                var query = (from s in db.front_site_sections
                             join v in db.front_page_viewss
                             on s.f_page_view equals v.id
                             where (s.f_site.Equals(siteId) && s.f_front_section.Equals(siteSection))
                             select v.c_url);
                if (query.Any())
                    ViewPath = query.SingleOrDefault();

                return ViewPath;
            }
        }

        /// <summary>
        /// Получение информации по сайту
        /// </summary>
        /// <returns></returns>
        public override SitesModel getSiteInfo()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sitess
                    .Where(w => w.c_alias.Equals(domain))
                    .Select(s => new SitesModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        LongTitle = s.c_name_long,
                        Alias = s.c_alias,
                        Adress = s.c_adress,
                        Phone = s.c_phone,
                        Fax = s.c_fax,
                        Email = s.c_email,
                        Site = s.c_url,
                        Worktime = s.c_worktime,
                        Logo = new Photo
                        {
                            Url = s.c_logo
                        },
                        ContentId = (Guid)s.f_content,
                        ContentType = (ContentLinkType)Enum.Parse(typeof(ContentLinkType), s.c_content_type, true),
                        Type = s.c_content_type,
                        Scripts = s.c_scripts,
                        Facebook = s.c_facebook,
                        Vk = s.c_vk,
                        Instagramm = s.c_instagramm,
                        Odnoklassniki = s.c_odnoklassniki,
                        Twitter = s.c_twitter,
                        Theme = s.c_theme,
                        BackGroundImg = new Photo
                        {
                            Url = s.c_background_img
                        }
                    });

                if (data.Any())
                    return data.SingleOrDefault();

                return null;
            }
        }


        public override string getDomainSite()
        {
            using (var db = new CMSdb(_context))
            {
                string siteId = _domain;
                var data = db.cms_sites_domainss.Where(w => w.f_site == siteId);
                if (data.Any())
                {
                    return data.OrderBy(o => o.num).Select(s => s.c_domain).FirstOrDefault();
                }
                return null;
            }
        }
        /// <summary>
        /// Получение информации по сайту
        /// </summary>
        /// <returns></returns>
        public override UsersModel[] getSiteAdmins()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.cms_userss.Where(w => w.f_group == "admin");
                if (query.Any())
                {
                    var data = query.Select(s => new UsersModel
                    {
                        Id = s.id,
                        FIO = s.c_surname + " " + s.c_name + " " + s.c_patronymic,
                        EMail = s.c_email
                    }).ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// Получим список элементов карты сайта для контроллера
        /// </summary>
        /// <returns></returns>
        public override SiteMapModel[] getSiteMapListShort(string path)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site.Equals(domain))
                    .Where(w => !w.b_disabled)
                    .Where(w => string.IsNullOrWhiteSpace(path) || w.c_path.Equals(path))
                    .OrderBy(o => o.c_path)
                    .ThenBy(o => o.n_sort)
                    .Select(s => new SiteMapModel
                    {
                        Title = s.c_title,
                        Path = s.c_path,
                        Alias = s.c_alias,
                        FrontSection = s.f_front_section
                    });

                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получение меню из карты сайта
        /// </summary>
        /// <returns></returns>
        public override SiteMapModel[] getSiteMapList() //string domain
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sv_sitemap_menus
                    .Where(w => w.f_site.Equals(domain))
                    .Where(w => !w.b_disabled)
                    .OrderBy(o => o.n_sort)
                    .Select(s => new SiteMapModel
                    {
                        Id = s.id,
                        Site = s.f_site,
                        FrontSection = s.f_front_section,
                        Path = s.c_path,
                        Alias = s.c_alias,
                        Title = s.c_title,
                        Text = s.c_text,
                        Preview = s.c_preview,
                        Url = s.c_url,
                        Desc = s.c_desc,
                        Keyw = s.c_keyw,
                        Disabled = s.b_disabled,
                        DisabledMenu = s.b_disabled_menu,
                        Sort = s.n_sort,
                        ParentId = s.uui_parent,
                        MenuAlias = s.menu_alias,
                        MenuSort = s.menu_sort,
                        MenuGroups = getSiteMapGroupMenu(s.id),
                        Photo = new Photo { Url = s.c_photo }
                    });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получение групп меню для элемента карты сайта
        /// </summary>
        /// <param name="id">Идентификатор карты сайта</param>
        /// <returns></returns>
        public override string[] getSiteMapGroupMenu(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemap_menutypess
                    .Where(w => w.f_sitemap.Equals(id))
                    .Select(s => s.f_menutype.ToString());

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получение списка баннеров
        /// </summary>
        /// <returns></returns>
        public override BannersModel[] getBanners()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.sv_sites_bannerss
                    .Where(b => b.site_alias == _domain)
                    //.Where(b => b.f_section == section)
                    .Where(b => b.banner_disabled == false);

                if (query.Any())
                {
                    int itemCount = query.Count();

                    var list = query
                        .OrderBy(b => b.banner_sort)
                        .Select(s => new BannersModel()
                        {
                            Id = s.banner_id,
                            Title = s.banner_title,
                            Url = s.banner_url,
                            Text = s.banner_text,
                            Date = s.banner_date,
                            Sort = s.banner_sort,
                            SectionAlias = s.section_alias,
                            Photo = new Photo
                            {
                                Url = s.banner_image
                            }
                        });

                    return list.ToArray();
                }

                return null;
            }
        }

        /// <summary>
        /// Получаем баннер
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override BannersModel getBanner(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_bannerss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new BannersModel
                    {
                        Id = s.id,
                        Url = s.c_url
                    });

                if (query.Any())
                {
                    db.content_bannerss
                        .Where(w => w.id.Equals(id))
                        .Set(u => u.n_count_click, u => u.n_count_click + 1)
                        .Update();

                    return query.SingleOrDefault();
                }

                return null;
            }
        }

        /// <summary>
        /// карта сайта
        /// </summary>
        /// <param name="path"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public override SiteMapModel getSiteMap(string path, string alias) //, string domain
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site == domain)
                    .Where(w => w.b_disabled == false)
                    .Where(w => w.c_path.ToLower() == path.ToLower())
                    .Where(w => w.c_alias.ToLower() == alias.ToLower());

                var data = query.Select(s => new SiteMapModel
                {
                    Title = s.c_title,
                    Text = s.c_text,
                    Alias = s.c_alias,
                    Path = s.c_path,
                    Id = s.id,
                    FrontSection = s.f_front_section,
                    ParentId = s.uui_parent
                });

                if (data.Any())
                    return data.First();

                return null;
            }
        }

        /// <summary>
        /// Получим эл-т карты сайта по frontSection, что фактически является названием контроллера
        /// </summary>
        /// <param name="frontSection"></param>
        /// <returns></returns>
        public override SiteMapModel getSiteMap(string frontSection)
        {
            string domain = _domain;

            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site == domain)
                    .Where(w => w.b_disabled == false)
                    .Where(w => w.f_front_section.ToLower() == frontSection.ToLower());

                var data = query.Select(s => new SiteMapModel
                {
                    Title = s.c_title,
                    Text = s.c_text,
                    Alias = s.c_alias,
                    Path = s.c_path,
                    Id = s.id,
                    ParentId = s.uui_parent,
                    FrontSection = s.f_front_section
                });

                if (data.Any())
                    return data.First();

                return null;
            }
        }

        /// <summary>
        /// Получим сестринские эл-ты карты сайта по пути
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override string[] getSiteMapSiblings(string path)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site.Equals(_domain))
                    .Where(w => w.c_path.Equals(path))
                    .Select(s => s.c_alias);

                if (!query.Any()) return null;
                return query.ToArray();
            }
        }

        /// <summary>
        /// Получим сестринские эл-ты из карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override List<SiteMapModel> getSiteMapSiblingElements(string path)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.b_disabled == false)
                    .Where(w => w.b_disabled_menu == false)
                    .Where(w => w.f_site.Equals(_domain))
                    .Where(w => w.c_path.Equals(path))
                    .OrderBy(o => o.n_sort)
                    .Select(s => new SiteMapModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Alias = s.c_alias,
                        Path = s.c_path,
                        FrontSection = s.f_front_section,
                        Url = s.c_url,
                        ParentId = s.uui_parent
                    });

                if (!query.Any()) return null;
                return query.ToList();
            }
        }

        /// <summary>
        /// Дочерние элементы
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public override SiteMapModel[] getSiteMapChild(Guid ParentId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemaps
                                 .Where(w => w.b_disabled == false)
                                 .Where(w => w.b_disabled_menu == false)
                                 .Where(w => w.uui_parent.Equals(ParentId))
                                 .Where(w => w.b_disabled_menu == false)
                                 .OrderBy(o => o.n_sort)
                                 .Select(c => new SiteMapModel
                                 {
                                     Title = c.c_title,
                                     Alias = c.c_alias,
                                     Path = c.c_path,
                                     FrontSection = c.f_front_section,
                                     Url = c.c_url
                                 });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Список прикрепленных лдокументов к элементу карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override DocumentsModel[] getAttachDocuments(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_documentss.Where(w => w.id_page == id)
                 .OrderBy(o => o.n_sort)
                 .Select(s => new DocumentsModel
                 {
                     id = s.id,
                     Title = s.c_title,
                     FilePath = s.c_file_path,
                     idPage = s.id_page
                 });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получаем хленые крошки
        /// </summary>
        /// <param name="Url">относительная ссылка на страницу</param>
        /// <returns></returns>
        public override List<Breadcrumbs> getBreadCrumbCollection(string Url)
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                int _len = Url.Count();
                int _lastIndex = Url.LastIndexOf("/");
                List<Breadcrumbs> data = new List<Breadcrumbs>();

                while (_lastIndex > -1)
                {
                    string _path = Url.Substring(0, _lastIndex + 1).ToString();
                    string _alias = Url.Substring(_lastIndex + 1).ToString();
                    if (_alias == String.Empty) _alias = " ";

                    var getContentSitemaps = db.content_sitemaps
                                .Where(w => w.f_site == domain)
                                .Where(w => w.c_path == _path)
                                .Where(w => w.c_alias == _alias)
                                //.Take(1)
                                .Select(w => new Breadcrumbs
                                {
                                    Title = w.c_title,
                                    Url = w.c_path + w.c_alias
                                });
                    if (getContentSitemaps.Any())
                        try
                        {
                            var itemContentSitemap = getContentSitemaps.SingleOrDefault();
                            if (itemContentSitemap != null)
                                data.Add(itemContentSitemap);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("FrontRepository > getBreadCrumbCollection: Found more than one record " + ex);
                        }
                    Url = Url.Substring(0, _lastIndex);
                    _len = Url.Count();
                    _lastIndex = Url.LastIndexOf("/");

                }
                if (data.Any())
                {
                    data.Reverse();
                    return data;
                }

                return data;
            }
        }

        /// <summary>
        /// Получаем новости для модуля на главной странице
        /// </summary>
        /// <returns></returns>
        public override List<MaterialFrontModule> getMaterialsModule()
        {
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                var contentType = ContentType.MATERIAL.ToString().ToLower();

                // список id-новостей для данного сайта
                var materialIds = db.content_content_links.Where(e => e.f_content_type == contentType)
                    .Join(db.cms_sitess.Where(o => o.c_alias == domain),
                            e => e.f_link,
                            o => o.f_content,
                            (e, o) => e.f_content
                            );

                if (!materialIds.Any())
                    return null;

                // список групп
                var groups = db.content_materials_groupss
                    .Select(s => s.id).ToArray();

                List<MaterialFrontModule> list = new List<MaterialFrontModule>();

                foreach (var g in groups)
                {
                    var query = db.content_sv_materials_groupss
                        .Where(w => materialIds.Contains(w.id))
                        .Where(w => w.group_id.Equals(g));

                    if (g != Guid.Parse("651CFEB9-E157-4F42-B40D-DE5A7DC1A8FC"))
                    {
                        query = query.Where(w => w.d_date <= DateTime.Now);
                    }

                    var data = query
                        .Where(w => w.b_disabled == false)
                        .OrderByDescending(o => o.d_date)
                        .Select(s => new MaterialFrontModule
                        {
                            Title = s.c_title,
                            Alias = s.c_alias,
                            Date = s.d_date,
                            GroupName = s.group_title,
                            GroupAlias = s.group_alias,
                            Photo = s.c_preview
                        });


                    // берём последние 3 новости данной группы
                    if (data.Any())
                        list.AddRange(data.Take(2));
                }

                if (list.Any())
                    return list;

                return null;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получим список новостей для определенной сущности
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <returns></returns>
        public override MaterialsList getMaterialsList(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                if (!string.IsNullOrEmpty(filter.Domain))
                {
                    var contentType = ContentType.MATERIAL.ToString().ToLower();

                    //Запрос типа:
                    //Select t.*, s.* from[dbo].[content_content_link] t left join[dbo].[cms_sites] s
                    //on t.f_link = s.f_content Where s.c_alias = 'main'
                    var materials = db.content_content_links.Where(e => e.f_content_type == contentType)
                        .Join(db.cms_sitess.Where(o => o.c_alias == filter.Domain),
                                e => e.f_link,
                                o => o.f_content,
                                (e, o) => e.f_content
                                );

                    if (!materials.Any())
                        return null;

                    var query = db.content_materialss
                                .Where(w => materials.Contains(w.id));

                    if (filter.Disabled != null)
                    {
                        query = query.Where(w => w.b_disabled == filter.Disabled);
                    }
                    if (!String.IsNullOrEmpty(filter.SearchText))
                    {
                        query = query.Where(w => w.c_title.ToLower().Contains(filter.SearchText.ToLower()));
                    }
                    if (filter.Date != null)
                    {
                        query = query.Where(w => w.d_date >= filter.Date);
                    }
                    if (filter.DateEnd != null)
                    {
                        query = query.Where(w => w.d_date <= filter.DateEnd);
                    }

                    if (!String.IsNullOrEmpty(filter.Category))
                    {
                        if (filter.Category != "announcement")
                        {
                            query = query.Where(w => w.d_date <= DateTime.Now);
                        }

                        var category = db.content_materials_groupss.Where(w => w.c_alias == filter.Category).First().id;
                        query = query
                                    .Join(
                                            db.content_materials_groups_links
                                            .Where(o => o.f_group == category),
                                            e => e.id, o => o.f_material, (o, e) => o
                                         );
                        //query = query.Where(w => w.d_date <= DateTime.Now && w.);
                    }

                    query = query.OrderByDescending(w => w.d_date);

                    int itemCount = query.Count();

                    var materialsList = query
                            .Skip(filter.Size * (filter.Page - 1))
                            .Take(filter.Size)
                            .Select(s => new MaterialsModel
                            {
                                Id = s.id,
                                Title = s.c_title,
                                Alias = s.c_alias,
                                Year = s.n_year,
                                Month = s.n_month,
                                Day = s.n_day,
                                PreviewImage = new Photo()
                                {
                                    Url = s.c_preview
                                },
                                Text = s.c_text,
                                Url = s.c_url,
                                UrlName = s.c_url_name,
                                Date = s.d_date,
                                Keyw = s.c_keyw,
                                Desc = s.c_desc,
                                Disabled = s.b_disabled,
                                Important = s.b_important
                            });

                    if (materialsList.Any())
                        return new MaterialsList
                        {
                            Data = materialsList.ToArray(),
                            Pager = new Pager
                            {
                                page = filter.Page,
                                size = filter.Size,
                                items_count = itemCount,
                                page_count = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
                            }
                        };
                }

                return null;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Новость
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public override MaterialsModel getMaterialsItem(string year, string month, string day, string alias)
        {
            //string domain = _domain;
            //using (var db = new CMSdb(_context))
            //{
            //    int _year = Convert.ToInt32(year);
            //    int _month = Convert.ToInt32(month);
            //    int _day = Convert.ToInt32(day);


            //    var contentType = ContentType.MATERIAL.ToString().ToLower();


            //    var materials = db.content_content_links.Where(e => e.f_content_type == contentType)
            //                                            .Join(db.cms_sitess.Where(o => o.c_alias == domain),
            //                                                       e => e.f_link,
            //                                                       o => o.f_content,
            //                                                       (e, o) => e.f_content
            //                                                   );

            //    if (!materials.Any())
            //        return null;

            //    var query = db.content_materialss
            //                .Where(w => materials.Contains(w.id));



            //    query = query.Where(w => (w.n_year == _year) && (w.n_month == _month) && (w.n_day == _day) && (w.c_alias.ToLower() == alias.ToLower()));
            //    if (query.Any())
            //    {
            //        var material = query.Select(s => new MaterialsModel
            //        {
            //            Id = s.id,
            //            Title = s.c_title,
            //            Text = s.c_text,
            //            Date = s.d_date,
            //            PreviewImage = new Photo
            //            {
            //                Url = s.c_preview
            //            }
            //        }).SingleOrDefault();

            //        db.content_materialss
            //            .Where(w => w.id.Equals(material.Id))
            //            .Set(u => u.n_count_see, u => u.n_count_see + 1)
            //            .Update();

            //        return material;
            //    }

            //    return null;
            //}
            throw new NotImplementedException();
        }

        /// <summary>
        /// Выдает группы преесс-центра
        /// </summary>
        /// <returns></returns>
        public override MaterialsGroup[] getMaterialsGroup()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sv_materials_groups_for_sites
                    .Where(w => w.domain.Equals(_domain))
                    .OrderBy(o => o.n_sort)
                    .Select(s => new MaterialsGroup
                    {
                        Alias = s.c_alias,
                        Title = s.c_title
                    });

                if (query.Any()) return query.ToArray();
                return null;
            }
        }
        
        /// <summary>
        /// Получаем список фотоматериалов
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override PhotoModel[] getPhotoList(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_photoss.Where(w => w.f_album == id);
                if (query.Any())
                {
                    var data = query.OrderBy(o => o.n_sort)
                                  .Select(s => new PhotoModel()
                                  {
                                      PreviewImage = new Photo { Url = s.c_preview },
                                      PhotoImage = new Photo { Url = s.c_photo },
                                      PhotoOriginal = s.c_photo,
                                      Id = s.id,
                                      Title = s.c_title
                                  }).ToArray();
                    return data;
                }
                return null;
            }
        }
        
        #region private methods

        /// <summary>
        /// список доменных имен по алиасу сайта
        /// </summary>
        /// <param name="SiteId"></param>
        /// <returns></returns>
        private Domain[] getSiteDomains(string SiteId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sites_domainss.Where(w => w.f_site == SiteId);
                if (data.Any())
                    return data.Select(s => new Domain()
                    {
                        DomainName = s.c_domain,
                        id = s.id
                    }).ToArray();
                return null;
            }
        }

        #endregion
    }
}
