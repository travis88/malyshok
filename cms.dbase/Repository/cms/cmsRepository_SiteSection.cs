using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с разделами сайта
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Получаем список разделов сайта
        /// </summary>
        /// <param name="filtr">Фильтр</param>
        /// <returns></returns>
        public override SiteSectionList getSiteSectionList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.front_page_viewss.AsQueryable();
                query = query.OrderBy(o => o.n_sort).ThenBy(o => o.f_site);
                if (query.Any())
                {
                    int ItemCount = query.Count();
                    var List = query
                                .Select(s => new SiteSectionModel()
                                {
                                    Id = s.id,
                                    Title = s.c_title,
                                    Alias = s.f_page_type,
                                    SiteId = s.f_site
                                })
                                .Skip(filtr.Size * (filtr.Page - 1))
                                .Take(filtr.Size);
                    SiteSectionModel[] SiteSectionInfo = List.ToArray();

                    return new SiteSectionList
                    {
                        Data = SiteSectionInfo,
                        Pager = new Pager
                        {
                            page = filtr.Page,
                            size = filtr.Size,
                            items_count = ItemCount,
                            page_count = (ItemCount % filtr.Size > 0) ? (ItemCount / filtr.Size) + 1 : ItemCount / filtr.Size
                        }
                    };

                }
                return null;
            }
        }

        /// <summary>
        /// Получаем эл-т разделов сайта
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override SiteSectionModel getSiteSectionItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.front_page_viewss
                           .Where(w => w.id == id)
                           .Select(s => new SiteSectionModel
                           {
                               Id = s.id,
                               Title = s.c_title,
                               Alias = s.f_page_type,
                               Url = s.c_url,
                               SiteId = s.f_site
                           });
                if (data.Any()) return data.Single();
                return null;

            }
        }

        /// <summary>
        /// Удаляем раздел сайта
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override bool deleteSiteSection(Guid id)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        var data = db.front_page_viewss.Where(p => p.id == id);
                        if (!data.Any())
                        {
                            throw new Exception("Запись с таким Id не найдена");
                        }
                        var cdSiteSection = data.SingleOrDefault();
                        db.Delete(cdSiteSection);

                        db.front_site_sections.Where(w => w.f_page_view == id).Delete();
                        db.front_sections.Where(w => w.c_default_view == id).Delete();

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Materials,
                            Action = LogAction.delete,
                            PageId = cdSiteSection.id,
                            PageName = cdSiteSection.c_title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                var message = String.Format("cmsRepository: deleteSiteSection; id={0}", id);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Error, message, ex));

                return false;
            }
        }
        
        /// <summary>
        /// Обновляем запись
        /// </summary>
        /// <param name="upd">раздел</param>
        /// <returns></returns>
        public override bool updateSiteSection(SiteSectionModel upd)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        front_page_views cdSiteSection = db.front_page_viewss
                                                .Where(p => p.id == upd.Id)
                                                .SingleOrDefault();
                        if (cdSiteSection == null)
                            throw new Exception("Запись с таким Id не найдена");

                        cdSiteSection.c_title = upd.Title;
                        cdSiteSection.c_url = upd.Url;
                        db.Update(cdSiteSection);

                        if (cdSiteSection.f_page_type != upd.Alias)
                        {
                            var cdFontSection = db.front_sections
                                .Where(s => s.c_alias == cdSiteSection.f_page_type);
                            if (cdFontSection == null)
                                throw new Exception("cmsRepository_SiteSection updateSiteSection: No such frontSection (" + cdSiteSection.f_page_type + ")!");

                            var cdFontSectionItem = cdFontSection.SingleOrDefault();
                            cdFontSectionItem.c_alias = upd.Alias;

                            db.Update(cdFontSectionItem); //Тут должно сработать каскадное изменение во всех таблицах
                        }

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.SiteSection,
                            Action = LogAction.update,
                            PageId = upd.Id,
                            PageName = upd.Title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Добавляем запись
        /// </summary>
        /// <param name="sitesection">Раздел</param>
        /// <returns></returns>
        public override bool insertSiteSection(SiteSectionModel sitesection)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    front_page_views cdSiteSection = db.front_page_viewss
                                            .Where(p => p.id == sitesection.Id)
                                            .SingleOrDefault();
                    if (cdSiteSection != null)
                    {
                        throw new Exception("Запись с таким Id уже существует");
                    }

                    cdSiteSection = new front_page_views
                    {
                        f_site = sitesection.SiteId,
                        id = sitesection.Id,
                        c_title = sitesection.Title,
                        f_page_type = sitesection.Alias,
                        c_url = sitesection.Url
                    };
                    db.Insert(cdSiteSection);
                    
                    front_section cdSection = new front_section
                    {
                        id = Guid.NewGuid(),
                        c_name = sitesection.Title,
                        c_alias = sitesection.Alias,
                        c_default_view = sitesection.Id
                    };
                    db.Insert(cdSection);

                    //сделаем этот шаблон для всех существующих сайтов
                    var allsites = db.cms_sitess.Select(s => s.c_alias).ToArray();
                    foreach (var siteId in allsites)
                    {
                        db.front_site_sections
                            .Value(v => v.f_site, siteId)
                            .Value(v => v.f_front_section, sitesection.Alias)
                            .Value(v => v.f_page_view, sitesection.Id)
                            .Insert();
                    }
                    
                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.SiteSection,
                        Action = LogAction.insert,
                        PageId = sitesection.Id,
                        PageName = sitesection.Title,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

                    tran.Commit();
                    return true;
                }
            }
        }

    }
}
