using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с баннерами
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Получаем список секций баннеров
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override BannersSectionModel[] getSections()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_banner_sectionss
                    .OrderBy(o => o.n_sort)
                    .Select(s => new BannersSectionModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Sort = s.n_sort,
                        Disabled = s.b_disabled,
                        CountBanners = getCountBanners(s.id),
                        Width = s.n_width,
                        Height = s.n_height
                    });
                if (query.Any())
                    return query.ToArray();

                return null;
            }
        }

        /// <summary>
        /// Получаем отдельную секцию баннеров
        /// </summary>
        /// <param name="id">Id секции</param>
        /// <param name="domain">Домен</param>
        /// <param name="filter">Фильтр</param>
        /// <returns></returns>
        public override BannersSectionModel getSectionItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_banner_sectionss
                    .Where(w => w.id == id)
                    .Select(s => new BannersSectionModel()
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Sort = s.n_sort,
                        Disabled = s.b_disabled,
                        Width = s.n_width,
                        Height = s.n_height,
                        //BannerList = Заполняем в контроллере
                        //CountBanners =  Заполняем в контроллере //getCountBannersBySectionAndDomain(s.id),
                    });
                if (query.Any())
                    return query.FirstOrDefault();

                return null;
            }
        }

        /// <summary>
        /// Получим список баннеров для секции
        /// </summary>
        /// <param name="section">Секция</param>
        /// <param name="filter">Фильтр</param>
        /// <returns></returns>
        public override BannersListModel getBanners(Guid section, FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                if(filter == null)
                    throw new Exception("CmsRepository_Banners > getBanners: filter is null");

                var query = db.sv_sites_bannerss
                    .Where(b => b.site_alias == _domain)
                    .Where(b => b.banner_section == section);

                if(query.Any())
                {
                    int itemCount = query.Count();

                    var list = query
                        .OrderBy(b => b.banner_sort)
                        .Skip(filter.Size * (filter.Page - 1))
                        .Take(filter.Size)
                        .Select(s => new BannersModel()
                        {
                            Id = s.banner_id,
                            Site = s.banner_origin_site,
                            Title = s.banner_title,
                            Url = s.banner_url,
                            Text = s.banner_text,
                            Date = s.banner_date,
                            Sort = s.banner_sort,
                            Disabled = s.banner_disabled,
                            Section = s.banner_section,
                            CountClick = s.banner_clicks,
                            Photo = new Photo
                            {
                                Url = s.banner_image
                            },
                            Locked = s.banner_locked
                        });

                    return new BannersListModel
                        {
                            Data = list.ToArray(),
                            Pager = new Pager
                            {
                                page = filter.Page,
                                size = filter.Size,
                                items_count = itemCount,
                                page_count = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1
                                    : itemCount / filter.Size
                            }
                        };
                    }

                return null;
            }
        }

        /// <summary>
        /// Получаем кол-во баннеров для сайта и секции
        /// </summary>
        /// <param name="section">Секция баннеров</param>
        /// <returns></returns>
        public override int getCountBanners(Guid section)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.sv_sites_bannerss
                    .Where(b => b.site_alias == _domain)
                    .Where(b => b.banner_section == section);

                if (query.Any())
                {
                    return query.Count();
                }

                return 0;
            }
        }

        /// <summary>
        /// Получим баннер
        /// </summary>
        /// <param name="id">Id баннера</param>
        /// <returns></returns>
        public override BannersModel getBannerItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_bannerss
                    .Where(w => w.id == id)
                    .Select(s => new BannersModel
                    {
                        Id = s.id,
                        Site = s.f_site,
                        Title = s.c_title,
                        Url = s.c_url,
                        Text = s.c_text,
                        CountClick = s.n_count_click,
                        Date = s.d_date,
                        Sort = s.n_sort,
                        Disabled = s.b_disabled,
                        Section = s.f_section,
                        Photo = new Photo
                        {
                            Url = s.c_photo
                        },
                        Locked = s.b_locked
                        //Links  заполняем в контроллере
                    });
                if (!query.Any()) return null;
                else { return query.FirstOrDefault(); }
            }
        }

        /// <summary>
        /// Проверим существование баннера
        /// </summary>
        /// <param name="id">Идентификатор баннера</param>
        /// <returns></returns>
        public override bool checkBannerExist(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_bannerss.Where(w => w.id == id).Count() > 0;
            }
        }

        /// <summary>
        /// Создадим новый баннер
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="item">Модель баннера</param>
        /// <returns></returns>
        public override bool createBanner(Guid id, BannersModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var query = db.content_bannerss.Where(w => w.id.Equals(id));

                    if (!query.Any())
                    {
                        var queryMaxSort = db.content_bannerss
                            .Where(w => w.f_site.Equals(item.Site))
                            .Where(w => w.f_section.Equals(item.Section))
                            .Select(s => s.n_sort);

                        int maxSort = queryMaxSort.Any() ? queryMaxSort.Max() + 1 : 1;

                        db.content_bannerss
                            .Value(v => v.id, id)
                            .Value(v => v.f_site, item.Site)
                            .Value(v => v.c_title, item.Title)
                            .Value(v => v.c_photo, item.Photo != null ? item.Photo.Url : null)
                            .Value(v => v.c_url, item.Url)
                            .Value(v => v.c_text, item.Text)
                            .Value(v => v.d_date, item.Date)
                            .Value(v => v.n_sort, maxSort)
                            .Value(v => v.b_disabled, item.Disabled)
                            .Value(v => v.f_section, item.Section)
                            .Insert();


                        var siteId = currentSiteId();
                        var siteBanners = new content_content_link()
                        {
                            id = Guid.NewGuid(),
                            f_content = id,
                            f_content_type = ContentType.BANNER.ToString().ToLower(),
                            f_link = siteId,
                            f_link_type = ContentLinkType.SITE.ToString().ToLower(),
                            b_origin = true
                        };
                        db.Insert(siteBanners);

                        // логирование
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Banners,
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
                }
                return false;
            }
        }

        /// <summary>
        /// Обновляем запись баннера
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="item">Модель баннера</param>
        /// <param name="userId">Id-пользователя</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool updateBanner(Guid id, BannersModel item)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_bannerss.Where(w => w.id.Equals(id));

                if (query.Any())
                {
                    string img = (item.Photo != null) ? item.Photo.Url: null;

                    query
                        .Set(s => s.c_title, item.Title)
                        .Set(s => s.c_photo, item.Photo.Url)
                        .Set(s => s.c_url, item.Url)
                        .Set(s => s.c_text, item.Text)
                        .Set(s => s.d_date, item.Date)
                        .Set(s => s.b_disabled, item.Disabled)
                        .Set(s => s.f_section, item.Section)
                        .Set(s => s.b_locked, item.Locked)
                        .Update();

                    // логирование
                    //insertLog(userId, IP, "update", id, string.Empty, "Banners", item.Title);
                    var log = new LogModel()
                    {
                        Site =_domain,
                        Section = LogSection.Banners,
                        Action = LogAction.update,
                        PageId = id,
                        PageName = item.Title,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Удаляем баннер
        /// </summary>
        /// <param name="id">Id-баннера</param>
        /// <param name="userId">Id-пользователя</param>
        /// <param name="IP">ip-адрес</param>
        /// <returns></returns>
        public override bool deleteBanner(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var query = db.content_bannerss.Where(w => w.id == id);

                    if (query.Any())
                    {
                        string title = query.Select(s => s.c_title).FirstOrDefault();

                        db.content_bannerss
                            .Where(b => b.id.Equals(id))
                            .Delete();

                        db.content_content_links
                            .Where(b => b.f_content == id)
                            .Where(b => b.f_content_type == ContentType.BANNER.ToString())
                            .Delete();

                        // логирование
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Banners,
                            Action = LogAction.delete,
                            PageId = id,
                            PageName = title,
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
        /// Меняем приоритет сортировки в баннерах для определённой секции
        /// </summary>
        /// <param name="id">Id-баннера</param>
        /// <param name="permit">Приоритет</param>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public override bool permit_Banners(Guid id, int permit)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_bannerss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new BannersModel
                    {
                        Section = s.f_section,
                        Sort = s.n_sort
                    });

                if (data.Any())
                {
                    var query = data.FirstOrDefault();
                    if (permit > query.Sort)
                    {
                        db.content_bannerss
                            .Where(w => w.f_site.Equals(_domain))
                            .Where(w => w.f_section.Equals(query.Section))
                            .Where(w => w.n_sort > query.Sort && w.n_sort <= permit)
                            .Set(u => u.n_sort, u => u.n_sort - 1)
                            .Update();
                    }
                    else
                    {
                        db.content_bannerss
                            .Where(w => w.f_site.Equals(_domain))
                            .Where(w => w.f_section.Equals(query.Section))
                            .Where(w => w.n_sort < query.Sort && w.n_sort >= permit)
                            .Set(u => u.n_sort, u => u.n_sort + 1)
                            .Update();
                    }
                    db.content_bannerss
                        .Where(w => w.id.Equals(id))
                        .Set(u => u.n_sort, permit)
                        .Update();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
