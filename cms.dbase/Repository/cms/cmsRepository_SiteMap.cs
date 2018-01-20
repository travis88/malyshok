using System;
using System.Collections.Generic;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с картой сайта
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Получаем список записей карты сайта
        /// </summary>
        /// <param name="site">Алиас сайта</param>
        /// <param name="filtr">Фильтр</param>
        /// <returns></returns>
        public override SiteMapList getSiteMapList(string site, FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                if (string.IsNullOrEmpty(filtr.Group))
                {
                    var query = db.content_sitemaps
                        .Where(w => w.f_site == site)
                        .Where(w => w.uui_parent == null)
                        .Where(w => !w.c_alias.Equals(" "));

                    if (query.Any())
                    {
                        int itemCount = query.Count();

                        var list = query.Select(s => new SiteMapModel
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
                            Blocked = s.b_blocked,
                            DisabledMenu = s.b_disabled_menu,
                            Sort = s.n_sort,
                            ParentId = s.uui_parent,
                            CountSibling = getCountSiblings(s.id)
                        })
                        .Skip(filtr.Size * (filtr.Page - 1))
                        .Take(filtr.Size);

                        var siteMapList = list.OrderBy(o => o.Sort).ToArray();

                        return new SiteMapList
                        {
                            Data = siteMapList,
                            Pager = new Pager
                            {
                                page = filtr.Page,
                                size = filtr.Size,
                                items_count = itemCount,
                                page_count = (itemCount % filtr.Size > 0) ? (itemCount / filtr.Size) + 1 : itemCount / filtr.Size
                            }
                        };
                    }
                }
                else
                {
                    Guid menuId = Guid.Empty;
                    var res = Guid.TryParse(filtr.Group, out menuId); // Вообще filtr.Group должен быть типа Guid!!!, если в него передается Guid
                    if (!res)
                        throw new Exception("getSiteMapList: не удалось Guid.TryParse(filtr.Group, out menuId)");
                    
                    var query = db.content_sv_sitemap_menus
                        .Where(w => w.f_site == site)
                        .Where(w => w.f_menutype == menuId)
                        .OrderBy(w => w.menu_sort);

                    if (query.Any())
                    {
                        int itemCount = query.Count();

                        var list = query.Select(s => new SiteMapModel
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
                            Blocked = s.b_blocked, //???
                            DisabledMenu = s.b_disabled_menu,
                            Sort = s.n_sort,
                            ParentId = s.uui_parent,
                            CountSibling = getCountSiblings(s.id)
                        }).Skip(filtr.Size * (filtr.Page - 1))
                          .Take(filtr.Size);

                        var siteMapList = list.ToArray();

                        return new SiteMapList
                        {
                            Data = siteMapList,
                            Pager = new Pager
                            {
                                page = filtr.Page,
                                size = filtr.Size,
                                items_count = itemCount,
                                page_count = (itemCount % filtr.Size > 0) ? (itemCount / filtr.Size) + 1 : itemCount / filtr.Size
                            }
                        };
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Получаем единичную запись карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override SiteMapModel getSiteMapItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemaps
                    .Where(w => w.id.Equals(id))
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
                        Blocked = s.b_blocked,
                        DisabledMenu = s.b_disabled_menu,
                        Sort = s.n_sort,
                        ParentId = s.uui_parent,
                        CountSibling = getCountSiblings(s.id),
                        MenuGroups = getSiteMapGroupMenu(id),
                        Photo = new Photo
                        {
                            Url = s.c_photo
                        }
                    });

                if (!data.Any())
                    return null;
                else
                    return data.FirstOrDefault();
            }
        }

        /// <summary>
        /// Получим группы меню для элемента карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override string[] getSiteMapGroupMenu(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemap_menutypess
                    .Where(w => w.f_sitemap.Equals(id))
                    .Select(s => s.f_menutype.ToString());

                if (!data.Any())
                    return null;
                else
                    return data.ToArray();
            }
        }

        /// <summary>
        /// Получим кол-во дочерних элементов карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override int getCountSiblings(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                int result = db.content_sitemaps
                    .Where(w => w.uui_parent.Equals(id))
                    .Count();

                return result;
            }
        }

        /// <summary>
        /// Проверяем существование элемента карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override bool checkSiteMap(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                int count = db.content_sitemaps.Where(w => w.id == id).Count();
                bool result = count > 0 ? true : false;

                return result;
            }
        }
        /// <summary>
        /// Определяет есть ли на этом уровне элемент с таким алиасом
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public override bool ckeckSiteMapAlias(string alias, string ParentId, Guid ThisGuid)
        {
            using (var db = new CMSdb(_context))
            {
                int _count=0;                
                var query = db.content_sitemaps.Where(w => w.c_alias == alias && w.id!=ThisGuid && w.f_site==_domain);
                query = (String.IsNullOrEmpty(ParentId)) 
                    ? query.Where(w => w.uui_parent == null) 
                    : query.Where(w => w.uui_parent == Guid.Parse(ParentId));
                _count= query.Count();
                if (_count > 0)
                {
                    return true;
                }                
                return false;
            }
        }

        /// <summary>
        /// Проверяем существование элемента карты сайта
        /// </summary>
        /// <param name="path"></param>
        /// <param name="alias"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool existSiteMap(string path, string alias, Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemaps
                    .Where(w => w.f_site.Equals(_domain))
                    .Where(w => w.c_path.ToLower().Equals(path.ToLower()))
                    .Where(w => w.c_alias.ToLower().Equals(alias.ToLower()))
                    .Where(w=>w.id!=Id);
                    

                return query.Any();
            }
        }

        /// <summary>
        /// Создаём новый раздел в карте сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <param name="item">Элемент карты сайта</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="IP">Ip-адрес</param>
        /// <returns></returns>
        public override bool createSiteMapItem(Guid id, SiteMapModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.content_sitemaps.Where(w => w.id.Equals(id));
                    if (!data.Any())
                    {
                        var queryMaxSort = db.content_sitemaps
                            .Where(w => w.f_site == item.Site)
                            .Where(w => w.c_path.Equals(item.Path))
                            .Select(s => s.n_sort);

                        int maxSort = queryMaxSort.Any() ? queryMaxSort.Max() + 1 : 1;

                        db.content_sitemaps
                            .Value(p => p.id, id)
                            .Value(p => p.f_site, item.Site)
                            .Value(p => p.f_front_section, item.FrontSection)
                            .Value(p => p.c_path, item.Path)
                            .Value(p => p.c_alias, item.Alias)
                            .Value(p => p.c_title, item.Title)
                            .Value(p => p.c_preview, item.Preview)
                            .Value(p => p.c_url, item.Url)
                            .Value(p => p.c_desc, item.Desc)
                            .Value(p => p.c_keyw, item.Keyw)
                            .Value(p => p.b_disabled, item.Disabled)
                            .Value(p => p.b_blocked, item.Blocked)
                            .Value(p => p.b_disabled_menu, item.DisabledMenu)
                            .Value(p => p.n_sort, maxSort)
                            .Value(p => p.c_text, item.Text)
                            .Value(p => p.uui_parent, item.ParentId)
                            .Value(p => p.c_photo, item.Photo != null ? item.Photo.Url : null)
                            .Insert();

                        // группы меню
                        if (item.MenuGroups != null)
                        {
                            foreach (var m in item.MenuGroups)
                            {
                                Guid menuId = Guid.Parse(m);

                                var _maxSortMenu = db.content_sitemap_menutypess
                                    .Where(w => w.f_site.Equals(item.Site))
                                    .Where(w => w.f_menutype.Equals(menuId))
                                    .Select(s => s.n_sort);

                                int mS = _maxSortMenu.Any() ? _maxSortMenu.Max() : 0;

                                var menu = db.content_sitemap_menutypess
                                    .Value(p => p.f_sitemap, id)
                                    .Value(p => p.f_menutype, menuId)
                                    .Value(p => p.f_site, item.Site)
                                    .Value(p => p.n_sort, mS + 1)
                                    .Insert();
                            }
                        }

                        // логирование
                        //insertLog(userId, IP, "insert", id, String.Empty, "SiteMap", item.Title);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.SiteMap,
                            Action = LogAction.insert,
                            PageId = id,
                            PageName = item.Title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Обновляем запись карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <param name="item">Элемент карты сайта</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="IP">Ip-адрес</param>
        /// <returns></returns>
        public override bool updateSiteMapItem(Guid id, SiteMapModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.content_sitemaps.Where(w => w.id.Equals(id));

                    if (data.Any())
                    {
                        var oldRecord = data.SingleOrDefault();

                        data.Where(w => w.id.Equals(id))
                            .Set(u => u.f_site, item.Site)
                            .Set(u => u.f_front_section, item.FrontSection)
                            .Set(u => u.c_path, item.Path)
                            .Set(u => u.c_alias, item.Alias)
                            .Set(u => u.c_title, item.Title)
                            .Set(u => u.c_text, item.Text)
                            .Set(u => u.c_preview, item.Preview)
                            .Set(u => u.c_url, item.Url)
                            .Set(u => u.c_desc, item.Desc)
                            .Set(u => u.c_keyw, item.Keyw)
                            .Set(u => u.b_disabled, item.Disabled)
                            .Set(u => u.b_blocked, item.Blocked)
                            .Set(u => u.b_disabled_menu, item.DisabledMenu)
                            .Set(u => u.c_photo, item.Photo != null ? item.Photo.Url : null)
                            .Update();

                        #region обновим алиасы для дочерних эл-тов
                        // заменяемый путь 
                        string _oldPath = oldRecord.c_path.Equals("/") ?
                            oldRecord.c_path + oldRecord.c_alias : oldRecord.c_path + "/" + oldRecord.c_alias;

                        // новый путь
                        string _newPath = item.Path.Equals("/") ?
                            item.Path + item.Alias : item.Path + "/" + item.Alias;

                        // список дочерних эл-тов для обновления алиаса
                        var listToUpdate = db.content_sitemaps
                            .Where(w => w.f_site.Equals(item.Site))
                            .Where(w => w.c_path.StartsWith(_oldPath));

                        if (listToUpdate.Any())
                        {
                            listToUpdate
                                .Set(u => u.c_path, u => u.c_path.Replace(_oldPath, _newPath))
                                .Update();
                        }
                        #endregion

                        #region группы меню
                        // группы меню
                        var menuOldQuery = db.content_sitemap_menutypess
                            .Where(w => w.f_site.Equals(_domain))
                            .Where(w => w.f_sitemap.Equals(id))
                            .Select(s => s.f_menutype);

                        if (menuOldQuery.Any())
                        {
                            var menuOld = menuOldQuery.ToArray();

                            if (item.MenuGroups != null)
                            {
                                //удаляем значения
                                var CurrenMenuGroup = item.MenuGroups.Select(s => Guid.Parse(s)).ToArray();
                                foreach (var d in menuOld)
                                {
                                    if (!CurrenMenuGroup.Contains(d))
                                    {
                                        #region удаляем элемент из группы
                                        //смещаем приоритеты в группах
                                        int current_sort = db.content_sitemap_menutypess
                                                             .Where(w => w.f_site == _domain)
                                                             .Where(w => w.f_sitemap == id)
                                                             .Where(w => w.f_menutype == d)
                                                             .Select(s => s.n_sort).Single();

                                        //удаляем 
                                        db.content_sitemap_menutypess
                                          .Where(w => (w.f_sitemap == id && w.f_menutype == d))
                                          .Delete();

                                        db.content_sitemap_menutypess
                                          .Where(w => w.f_site == _domain && w.f_menutype == d && w.n_sort >= current_sort)
                                          .Set(p => p.n_sort, p => p.n_sort - 1)
                                          .Update();

                                        #endregion
                                    }
                                }

                                //добавляем значения
                                foreach (var m in item.MenuGroups)
                                {
                                    Guid menuId = Guid.Parse(m);

                                    if (!menuOld.Contains(menuId))
                                    {
                                        var maxSortQuery = db.content_sitemap_menutypess
                                            .Where(w => w.f_site.Equals(item.Site))
                                            .Where(w => w.f_menutype.Equals(menuId));

                                        int maxSort = maxSortQuery.Any() ? maxSortQuery.Select(s => s.n_sort).Max() : 0;

                                        db.content_sitemap_menutypess
                                            .Value(p => p.f_sitemap, id)
                                            .Value(p => p.f_menutype, menuId)
                                            .Value(p => p.f_site, item.Site)
                                            .Value(p => p.n_sort, maxSort + 1)
                                            .Insert();
                                    }
                                    #region //
                                    //else
                                    //{
                                    //    //если в группе был и остается то нничего не делаем
                                    //    if (item.MenuGroups.Contains(menuId.ToString())){

                                    //    }
                                    //    else
                                    //    {
                                    //        #region удаляем элемент из группы
                                    //        //смещаем приоритеты в группах
                                    //        int current_sort = db.content_sitemap_menutypess
                                    //                                .Where(w => w.f_site.Equals(_domain))
                                    //                                .Where(w => w.f_sitemap.Equals(id))
                                    //                                .Where(w => w.f_menutype == menuId)
                                    //                                .Select(s => s.n_sort).Single();

                                    //        db.content_sitemap_menutypess
                                    //          .Where(w => w.f_site == _domain && w.f_menutype == menuId && w.n_sort > current_sort)
                                    //          .Set(p => p.n_sort, p => p.n_sort - 1)
                                    //          .Update();

                                    //        //удаляем 
                                    //        db.content_sitemap_menutypess
                                    //          .Where(w => (w.f_sitemap == id && w.f_menutype == menuId))
                                    //          .Delete(); 
                                    //        #endregion
                                    //    }                                        
                                    //}
                                    #endregion
                                }
                            }
                            else
                            {
                                //смещаем все приоритеты
                                var menu_type = db.content_sitemap_menutypess.Where(w => w.f_sitemap == id).Select(s=>s.f_menutype).ToArray();
                                foreach (var mt in menu_type)
                                {

                                    int current_sort = db.content_sitemap_menutypess
                                                        .Where(w => w.f_site == _domain)
                                                        .Where(w => w.f_sitemap == id)
                                                        .Where(w => w.f_menutype == mt)
                                                        .Select(s => s.n_sort).Single();

                                    db.content_sitemap_menutypess
                                          .Where(w => w.f_site == _domain && w.f_menutype == mt && w.n_sort >= current_sort)
                                          .Set(p => p.n_sort, p => p.n_sort - 1)
                                          .Update();

                                }

                                //элемент удаляется из всех групп меню
                                db.content_sitemap_menutypess
                                    .Where(w => w.f_sitemap.Equals(id)).Delete();
                            }
                        }
                        else
                        {
                            //случай когда до этого элемент карты не принадлежал ни к  одной группе
                            if (item.MenuGroups != null)
                            {
                                foreach (var m in item.MenuGroups)
                                {
                                    Guid menuId = Guid.Parse(m);

                                    var _maxSortMenu = db.content_sitemap_menutypess
                                        .Where(w => w.f_site.Equals(item.Site))
                                        .Where(w => w.f_menutype.Equals(menuId));

                                    int resmaxSortMenu = _maxSortMenu.Any() ? _maxSortMenu.Select(s => s.n_sort).Max() : 0;

                                    var res = db.content_sitemap_menutypess
                                        .Value(p => p.f_sitemap, id)
                                        .Value(p => p.f_menutype, menuId)
                                        .Value(p => p.f_site, item.Site)
                                        .Value(p => p.n_sort, resmaxSortMenu + 1)
                                        .Insert();
                                }
                            }
                        }
                        #endregion

                        // логирование
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.SiteMap,
                            Action = LogAction.update,
                            PageId = id,
                            PageName = item.Title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Получаем список типов страниц
        /// </summary>
        /// <returns></returns>
        public override SiteMapMenu[] getSiteMapFrontSectionList(string domain)
        {
            using (var db = new CMSdb(_context))
            {
                const string ORG = "org";

                var type = db.cms_sitess
                    .Where(w => w.c_alias.Equals(domain))
                    .Select(s => s.c_content_type).SingleOrDefault();

                IQueryable<front_page_views> query;

                if (type != ORG)
                {
                    query = db.front_page_viewss
                        .Where(w => w.f_site.Equals(ORG) || w.f_site.Equals(type));
                }
                else
                {
                    if (domain.Equals("main"))
                    {
                        query = db.front_page_viewss
                            .Where(w => w.f_site.Equals("main") || w.f_site.Equals(type));
                    }
                    else
                    {
                        query = db.front_page_viewss
                            .Where(w => w.f_site.Equals(type));
                    }
                }

                query = query.OrderBy(n => n.n_sort);

                var data = query
                    .Select(s => new SiteMapMenu
                    {
                        Text = s.c_title,
                        Value = s.f_page_type
                    });

                if (data.Any())
                    return data.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получаем единичную запись группы меню
        /// </summary>
        /// <returns></returns>
        public override SiteMapMenu getSiteMapMenu(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_sitemap_menuss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new SiteMapMenu
                    {
                        Id = s.id,
                        Text = s.c_title,
                        Sort = s.n_sort,
                        Value = s.c_alias
                    });

                if (!query.Any()) return null;
                else return query.FirstOrDefault();
            }
        }

        /// <summary>
        /// Список доступных типов меню для элемента карты сайта
        /// </summary>
        /// <returns></returns>
        public override Catalog_list[] getSiteMapMenuTypes()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemap_menuss
                    .OrderBy(o => o.n_sort)
                    .Select(s => new Catalog_list
                    {
                        text = s.c_title,
                        value = s.id.ToString(),
                        available = s.b_available
                    });
                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }

        /// <summary>
        /// Добавляем меню в карту сайта
        /// </summary>
        /// <param name="item">Элемент карты сайта</param>
        /// <returns></returns>
        public override bool createOrUpdateSiteMapMenu(SiteMapMenu item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var query = db.content_sitemap_menuss.Where(w => w.id.Equals(item.Id));
                    if (!query.Any())
                    {
                        var sortMax = db.content_sitemap_menuss.Select(s => s.n_sort);

                        int max = sortMax.Any() ? sortMax.Max() + 1 : 1;

                        db.content_sitemap_menuss
                            .Value(v => v.c_title, item.Text)
                            .Value(v => v.n_sort, max)
                            .Insert();
                    }
                    else
                    {
                        db.content_sitemap_menuss
                            .Where(w => w.id.Equals(item.Id))
                            .Set(u => u.c_title, item.Text)
                            .Update();
                    }


                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.SiteMap,
                        Action = LogAction.update,
                        PageId = item.Id,
                        PageName = item.Text,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Добавляем меню в карту сайта
        /// </summary>
        /// <param name="item">Элемент карты сайта</param>
        /// <returns></returns>
        public override bool deleteSiteMapMenu(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var query = db.content_sitemap_menuss.Where(w => w.id == id);
                    if (query.Any())
                    {
                        var siteMapMenuItem = query.SingleOrDefault();

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.SiteMap,
                            Action = LogAction.delete,
                            PageId = id,
                            PageName = siteMapMenuItem.c_title + "(" + siteMapMenuItem.c_alias + ")",
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        db.Delete(siteMapMenuItem);
                    }

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Удаляем элемент карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="IP">Ip-адрес</param>
        /// <returns></returns>
        public override bool deleteSiteMapItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var itemToDelete = db.content_sitemaps
                        .Where(w => w.id.Equals(id))
                        .Select(s => new SiteMapModel
                        {
                            Title = s.c_title,
                            Path = s.c_path,
                            Alias = s.c_alias,
                            Sort = s.n_sort
                        }).FirstOrDefault();

                    // Обновляем поле для сортировки для сестринских эл-тов
                    var listToUpdate = db.content_sitemaps
                        .Where(w => w.c_path.Equals(itemToDelete.Path))
                        .Where(w => w.n_sort > itemToDelete.Sort);

                    listToUpdate.Set(u => u.n_sort, u => u.n_sort - 1).Update();

                    // Удаляем дочерние эл-ты 
                    deleteSiteMapItemCascad(id);

                    //удаляем  текущий элемент
                    db.content_sitemaps.Where(w => w.id == id).Delete();
                    #region удаляем связи с группой меню
                    var menuTypesBigger = db.content_sitemap_menutypess
                                            .Where(w => w.f_site.Equals(_domain))
                                            .Where(w => w.f_sitemap.Equals(id))
                                            .Select(s => new { s.f_menutype, s.n_sort });

                    if (menuTypesBigger.Any())
                    {
                        foreach (var mt in menuTypesBigger.ToArray())
                        {
                            db.content_sitemap_menutypess
                                .Where(w => w.f_site.Equals(_domain))
                                .Where(w => w.f_menutype.Equals(mt.f_menutype))
                                .Where(w => w.n_sort > mt.n_sort)
                                .Set(s => s.n_sort, s => s.n_sort - 1)
                                .Update();
                        }
                    }
                    #endregion

                    #region //

                    //var pParentIdToDrop = itemToDelete.Id;

                    //var listToDelete = db.content_sitemaps
                    //    .Where(w => w.id.Equals(id) || w.uui_parent == pParentIdToDrop);

                    //if (listToDelete.Any())
                    //{
                    //    foreach (var item in listToDelete.ToArray())
                    //    {
                    //        // удаляем связи с группой меню
                    //        var menuTypesBigger = db.content_sitemap_menutypess
                    //            .Where(w => w.f_site.Equals(item.f_site))
                    //            .Where(w => w.f_sitemap.Equals(item.id))
                    //            .Select(s => new { s.f_menutype, s.n_sort });

                    //        if (menuTypesBigger.Any())
                    //        {
                    //            foreach (var mt in menuTypesBigger.ToArray())
                    //            {
                    //                db.content_sitemap_menutypess
                    //                    .Where(w => w.f_site.Equals(item.f_site))
                    //                    .Where(w => w.f_menutype.Equals(mt.f_menutype))
                    //                    .Where(w => w.n_sort > mt.n_sort)
                    //                    .Set(s => s.n_sort, s => s.n_sort - 1)
                    //                    .Update();
                    //            }
                    //        }

                    //        var itemD = db.content_sitemaps
                    //            .Where(w => w.id == item.id)
                    //            .SingleOrDefault();

                    //        db.Delete(itemD);

                    //        // Логирование
                    //        var log = new LogModel()
                    //        {
                    //            Site = _domain,
                    //            Section = LogSection.SiteMap,
                    //            Action = LogAction.delete,
                    //            PageId = id,
                    //            PageName = item.c_title,
                    //            UserId = _currentUserId,
                    //            IP = _ip,
                    //        };
                    //        insertLog(log);
                    //    }
                    //} 
                    #endregion

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// каскадное удаление веток карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool deleteSiteMapItemCascad(Guid id) {
            using (var db = new CMSdb(_context))
            {
                var childlist = db.content_sitemaps.Where(w => w.uui_parent == id);
                var childlistarray = childlist.Select(s => s.id);
                if (childlistarray.Any())
                {
                    foreach (var item in childlistarray.ToArray())
                    {
                        deleteSiteMapItemCascad(item);

                        #region удаляем связи с группой меню
                        var menuTypesBigger = db.content_sitemap_menutypess
                                                .Where(w => w.f_site.Equals(_domain))
                                                .Where(w => w.f_sitemap.Equals(item))
                                                .Select(s => new { s.f_menutype, s.n_sort });

                        if (menuTypesBigger.Any())
                        {
                            foreach (var mt in menuTypesBigger.ToArray())
                            {
                                db.content_sitemap_menutypess
                                    .Where(w => w.f_site.Equals(_domain))
                                    .Where(w => w.f_menutype.Equals(mt.f_menutype))
                                    .Where(w => w.n_sort > mt.n_sort)
                                    .Set(s => s.n_sort, s => s.n_sort - 1)
                                    .Update();
                            }
                        }
                        #endregion
                    }
                    childlist.Delete();
                }

                return true;

            }
        }

        /// <summary>
        /// Получаем список дочерних элементов для текущего
        /// </summary>
        /// <param name="parent">Родительский идентификатор</param>
        /// <returns></returns>
        public override SiteMapModel[] getSiteMapChildrens(Guid parent)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemaps
                    .Where(w => w.uui_parent.Equals(parent))
                    .OrderBy(o => o.n_sort)
                    .Select(s => new SiteMapModel
                    {
                        Id = s.id,
                        Path = s.c_path,
                        Alias = s.c_alias,
                        Title = s.c_title,
                        Disabled = s.b_disabled,
                        Blocked = s.b_blocked,
                        DisabledMenu = s.b_disabled_menu,
                        Sort = s.n_sort,
                        CountSibling = getCountSiblings(s.id)
                    });

                if (!data.Any())
                    return null;
                else
                    return data.ToArray();
            }
        }

        /// <summary>
        /// Получаем хлебные крошки для карты сайта
        /// </summary>
        /// <param name="id">Идентификатор элемента карты сайта</param>
        /// <returns></returns>
        public override BreadCrumbSiteMap[] getSiteMapBreadCrumbs(Guid? id)
        {
            List<BreadCrumbSiteMap> breadCrumbList = new List<BreadCrumbSiteMap>();

            if (!id.Equals(null))
            {
                BreadCrumbSiteMap item = getSiteMapBreadCrumbItem((Guid)id);

                while (item != null)
                {
                    breadCrumbList.Add(item);
                    if (!item.ParentId.Equals(null))
                    {
                        item = getSiteMapBreadCrumbItem((Guid)item.ParentId);
                    }
                    else { item = null; }
                }
            }

            breadCrumbList.Reverse();

            return breadCrumbList != null ? breadCrumbList.ToArray() : null;
        }

        /// <summary>
        /// Единичная запись хлебных крошек для карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override BreadCrumbSiteMap getSiteMapBreadCrumbItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sitemaps
                    .Where(w => w.id.Equals(id))
                    .Select(s => new BreadCrumbSiteMap
                    {
                        Id = s.id,
                        Title = s.c_title,
                        ParentId = s.uui_parent
                    });

                if (!data.Any())
                    return null;
                else
                    return data.FirstOrDefault();
            }
        }

        /// <summary>
        /// Меняем приоритет для сортировки карты сайта
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <param name="permit">Порядока сортировки</param>
        /// <param name="domain">Алиас сайта</param>
        /// <param name="menuSort">Сортировка в записи для типа меню</param>
        /// <returns></returns>
        public override bool permit_SiteMap(Guid id, int permit, string menuSort)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    if (string.IsNullOrEmpty(menuSort))
                    {
                        var data = db.content_sitemaps
                            .Where(w => w.id.Equals(id))
                            .Select(s => new SiteMapModel
                            {
                                Path = s.c_path,
                                Sort = s.n_sort
                            }).FirstOrDefault();

                        if (permit > data.Sort)
                        {
                            db.content_sitemaps
                                .Where(w => w.f_site.Equals(_domain))
                                .Where(w => w.c_path.Equals(data.Path))
                                .Where(w => w.n_sort > data.Sort && w.n_sort <= permit)
                                .Set(u => u.n_sort, u => u.n_sort - 1)
                                .Update();
                        }
                        else
                        {
                            db.content_sitemaps
                                .Where(w => w.f_site.Equals(_domain))
                                .Where(w => w.c_path.Equals(data.Path))
                                .Where(w => w.n_sort < data.Sort && w.n_sort >= permit)
                                .Set(u => u.n_sort, u => u.n_sort + 1)
                                .Update();
                        }
                        db.content_sitemaps
                            .Where(w => w.id.Equals(id))
                            .Set(u => u.n_sort, permit)
                            .Update();
                    }
                    else
                    {
                        Guid m = Guid.Parse(menuSort);

                        var data = db.content_sv_sitemap_menus
                            .Where(w => w.id.Equals(id))
                            .Select(s => new SiteMapModel
                            {
                                MenuGr = s.f_menutype,
                                Sort = s.menu_sort
                            }).FirstOrDefault();

                        if (permit > data.Sort)
                        {
                            db.content_sitemap_menutypess
                                .Where(w => w.f_site.Equals(_domain))
                                .Where(w => w.f_menutype.Equals(m))
                                .Where(w => w.n_sort > data.Sort && w.n_sort <= permit)
                                .Set(u => u.n_sort, u => u.n_sort - 1)
                                .Update();
                        }
                        else
                        {
                            db.content_sitemap_menutypess
                                .Where(w => w.f_site.Equals(_domain))
                                .Where(w => w.f_menutype.Equals(m))
                                .Where(w => w.n_sort < data.Sort && w.n_sort >= permit)
                                .Set(u => u.n_sort, u => u.n_sort + 1)
                                .Update();
                        }
                        db.content_sitemap_menutypess
                            .Where(w => w.f_sitemap.Equals(id))
                            .Where(w => w.f_menutype.Equals(m))
                            .Set(u => u.n_sort, permit)
                            .Update();
                    }
                    tran.Commit();
                    return true;
                }
            }
        }
    }
}
