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
        private string _domain = "main";

        /// <summary>
        /// Конструктор
        /// </summary>
        public FrontRepository()
        {
            _context = "defaultConnection";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }

        public FrontRepository(string ConnectionString)
        {
            _context = ConnectionString;
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }
        
        /// <summary>
        /// Получение вьюхи
        /// </summary>
        /// <param name="siteSection">Секция</param>
        /// <returns></returns>
        public override string getView(string siteSection) //string siteId,
        {
            using (var db = new CMSdb(_context))
            {
                string ViewPath = "~/Error/404/";

                var query = (from s in db.front_site_sections
                             join v in db.front_page_viewss
                             on s.f_page_view equals v.id
                             where (s.f_site.Equals(_domain) && s.f_front_section.Equals(siteSection))
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



        public override SiteMapModel[] getMenu(string section)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_sv_sitemap_menus
                    .Where(w => w.menu_alias == section)
                    .Where(w => w.b_disabled == false && w.b_disabled_menu == false)
                    .OrderBy(o => o.n_sort)
                    .Select(c => new SiteMapModel
                    {
                        Title = c.c_title,
                        Alias = c.c_alias,
                        Path = c.c_path,
                        FrontSection = c.f_front_section,
                        Url = c.c_url
                    });

                return data.ToArray();
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
        /// Список прикрепленных документов к элементу карты сайта
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
        public override Breadcrumbs[] getBreadCrumb(string Url)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.SP_site_PagePath(Url)
                    .Select(s => new Breadcrumbs
                    {
                        Title = s.c_title,
                        Url = s.c_path + s.c_alias
                    });

                return query.ToArray();
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
                // список групп
                var groups = db.content_materials_groupss
                    .Select(s => s.id).ToArray();

                List<MaterialFrontModule> list = new List<MaterialFrontModule>();

                var query = db.content_materialss
                        .Where(w => w.d_date <= DateTime.Now);

                var data = query
                    .Where(w => w.b_disabled == false)
                    .OrderByDescending(o => o.d_date)
                    .Select(s => new MaterialFrontModule
                    {
                        Title = s.c_title,
                        Alias = s.c_alias,
                        Date = s.d_date,
                        Photo = s.c_preview
                    });


                // берём последние 3 новости данной группы
                if (data.Any())
                    list.AddRange(data.Take(6));

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

                    var query = db.content_materialss
                                .Where(w => w.b_disabled == false && w.d_date <= DateTime.Now);

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
                        var category = db.content_materials_groupss.Where(w => w.c_alias == filter.Category).First().id;
                        query = query
                                    .Join(
                                            db.content_materials_groups_links
                                            .Where(o => o.f_group == category),
                                            e => e.id, o => o.f_material, (o, e) => o
                                         );
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
            string domain = _domain;
            using (var db = new CMSdb(_context))
            {
                int _year = Convert.ToInt32(year);
                int _month = Convert.ToInt32(month);
                int _day = Convert.ToInt32(day);

                var contentType = ContentType.MATERIAL.ToString().ToLower();

                var query = db.content_materialss
                            .Where(w => (w.n_year == _year) && (w.n_month == _month) && (w.n_day == _day) && (w.c_alias.ToLower() == alias.ToLower()));
                if (query.Any())
                {
                    var material = query.Select(s => new MaterialsModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Text = s.c_text,
                        Date = s.d_date,
                        PreviewImage = new Photo
                        {
                            Url = s.c_preview
                        }
                    }).SingleOrDefault();

                    return material;
                }

                return null;
            }
            throw new NotImplementedException();
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


        public override CategoryModel[] getProdCatalogModule() {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_categoriess.Where(w => w.uui_parent == null);
                if (query.Any())
                {
                    var data = query.OrderBy(o => o.n_sort)
                                  .Select(s => new CategoryModel()
                                  {
                                      Title = s.c_title,
                                      Alias = s.c_alias,
                                      Parent = s.uui_parent,
                                      Id = s.id
                                  }).ToArray();
                    return data;
                }
                return null;
            }
        }
        public override CategoryModel[] getProdCatalogModule(string ParentPath)
        {
            using (var db = new CMSdb(_context))
            {
                ParentPath = (!string.IsNullOrEmpty(ParentPath)) ? ParentPath : "/";

                var query = db.content_categoriess.Where(w => w.c_path.IndexOf(ParentPath) == 0);
                if (query.Any())
                {
                    var data = query.OrderBy(o => new { o.c_path, o.n_sort })
                                  .Select(s => new CategoryModel()
                                  {
                                      Title = s.c_title,
                                      Alias = s.c_alias,
                                      Path = s.c_path,
                                      Id = s.id
                                  }).ToArray();
                    return data;
                }

                return null;
            }
        }
        public override ProductList getProdList(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                var BasketList = db.content_order_detailss
                    .Where(w => w.f_order == filter.Order);

                if (String.IsNullOrEmpty(filter.Category))
                {
                    var query = db.content_productss.Where(w => w.n_count > 0);
                    query = query.OrderBy(w => new { w.d_date, w.c_title });

                    int itemCount = query.Count();

                    var ProdList = query
                        .Skip(filter.Size * (filter.Page - 1))
                        .Take(filter.Size);
                    
                    var Prod = (from p in ProdList
                                join b in BasketList on p.id equals b.f_prod_id into ps
                                from b in ps.DefaultIfEmpty()
                                select new { p, b.n_count })
                                .Select(s => new ProductModel
                                {
                                    Id = s.p.id,
                                    Title = s.p.c_title,
                                    Code = s.p.c_code,
                                    Barcode = s.p.c_barcode,
                                    Price = (decimal)s.p.m_price,
                                    Standart = s.p.c_standart,
                                    Count = (int)s.p.n_count,
                                    inBasket = s.n_count,
                                    Photo = new Photo()
                                    {
                                        Url = s.p.c_photo
                                    }
                                });

                    return new ProductList
                    {
                        Data = Prod.ToArray(),
                        Pager = new Pager
                        {
                            page = filter.Page,
                            size = filter.Size,
                            items_count = itemCount,
                            page_count = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
                        }
                    };

                }
                else
                {
                    var path = filter.Category.Substring(0, filter.Category.LastIndexOf("/"));
                    var alias = filter.Category.Substring(filter.Category.LastIndexOf("/") + 1);
                    
                    var query = db.content_categoriess
                        .Where(w => w.c_path.StartsWith(filter.Category) || (w.c_path == path + "/" && w.c_alias == alias))
                        .Select(s => new { s.id });

                    var prodQuery = query
                        .Join(db.sv_productss, c => c.id, p => p.f_category, (c, p) => p)
                        .OrderBy(w => new { w.d_date, w.c_title });

                    int itemCount = prodQuery.Count();

                    var ProdList = prodQuery
                        .Skip(filter.Size * (filter.Page - 1))
                        .Take(filter.Size);

                    var Prod = (from p in ProdList
                                join b in BasketList on p.id equals b.f_prod_id into ps
                                from b in ps.DefaultIfEmpty()
                                select new { p, b.n_count })
                                .Select(s => new ProductModel
                                {
                                    Id = s.p.id,
                                    Title = s.p.c_title,
                                    Code = s.p.c_code,
                                    Barcode = s.p.c_barcode,
                                    Price = (decimal)s.p.m_price,
                                    Standart = s.p.c_standart,
                                    Count = (int)s.p.n_count,
                                    inBasket = s.n_count,
                                    Photo = new Photo()
                                    {
                                        Url = s.p.c_photo
                                    }
                                });

                    var  List = new ProductList
                    {
                        Data = Prod.ToArray(),
                        Pager = new Pager
                        {
                            page = filter.Page,
                            size = filter.Size,
                            items_count = itemCount,
                            page_count = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
                        }
                    };


                    return List;
                }
            }
        }

        /// <summary>
        /// Проверка пользователя по E-Mail
        /// </summary>
        /// <param name="Mail"></param>
        /// <returns></returns>
        public override bool CheckCustomerMail(string Mail)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_userss.Where(w => w.c_email == Mail);
                if (query.Any())
                    return true;
                else
                    return false;
            }
        }
        public override string ConfirmMail(Guid Code)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_userss.Where(w => w.id == Code);
                if (data.Any())
                {
                    return data.Select(s => s.id).FirstOrDefault().ToString();
                }
                return null;
            }
        }
        public override UsersModel getCustomer(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_userss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new UsersModel
                    {
                        Id = s.id,
                        FIO = s.c_name,
                        Address = s.c_address,
                        Phone = s.c_phone,
                        EMail = s.c_email,
                        Organization = s.c_organization,
                        Disabled = s.b_disable,
                        Vk = s.c_vk,
                        Facebook = s.c_facebook
                    })
                    .SingleOrDefault();
            }
        }
        public override bool createCustomer(UsersModel item)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_userss
                    .Value(v => v.id, item.Id)
                    .Value(v => v.c_name, item.FIO)
                    .Value(v => v.c_address, item.Address)
                    .Value(v => v.c_phone, item.Phone)
                    .Value(v => v.c_email, item.EMail)
                    .Value(v => v.c_organization, item.Organization)
                    .Value(v => v.c_salt, item.Salt)
                    .Value(v => v.c_hash, item.Hash)
                    .Value(v => v.d_register_date, DateTime.Now)
                    .Value(v => v.b_disable, item.Disabled)
                    .Insert() > 0;
            }
        }
        //public override bool updateCustomer(UsersModel item) { }
        //public override bool deleteCustomer(Guid id) { }

        /// <summary>
        /// Заказы
        /// </summary>
        /// <returns></returns>
        public override bool CheckOrder(Guid OrderId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_orderss.Where(w => w.id == OrderId && w.f_status==0);
                if (query.Any())
                    return true;
                else
                    return false;
            }
        }
        public override Guid? CreateOrder() {
            using (var db = new CMSdb(_context))
            {
                Guid OrderId = Guid.NewGuid();

                db.content_orderss
                   .Value(v => v.id, OrderId)
                   .Insert();

                return OrderId;
            }
        }
        //public override OrderModel[] getOrders(Guid UserId)
        //{

        //}


        public override bool addInBasket(Guid OrderId, Guid ProdId, int Count)
        {
            using (var db = new CMSdb(_context))
            {
                // Получаем данные о продукте
                var query = db.content_productss.Where(w => w.id == ProdId);
                //
                var orderProd_query = db.content_order_detailss
                    .Where(w => w.f_order == OrderId && w.f_prod_id == ProdId);

                if (query.Any())
                {
                    var Prod = query.Select(s => new ProductModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Barcode = s.c_barcode,
                        Code = s.c_code,
                        Photo = new Photo { Url = s.c_photo },
                        Price = (decimal)s.m_price
                    })
                    .SingleOrDefault();

                    if (orderProd_query.Any())
                    {
                        return db.content_order_detailss
                            .Where(w => w.f_order == OrderId && w.f_prod_id == ProdId)
                            .Set(p => p.c_caption, Prod.Title)
                            .Set(p => p.c_code, Prod.Code)
                            .Set(p => p.c_barcode, Prod.Barcode)
                            .Set(p => p.c_photo, Prod.Photo.Url)
                            .Set(p => p.m_price, Prod.Price)
                            .Set(p => p.n_count, Count)
                            .Update() > 0;
                    }
                    else
                    {
                        return db.content_order_detailss
                            .Value(v => v.f_order, OrderId)
                            .Value(v => v.f_prod_id, ProdId)
                            .Value(v => v.c_caption, Prod.Title)
                            .Value(v => v.c_code, Prod.Code)
                            .Value(v => v.c_barcode, Prod.Barcode)
                            .Value(v => v.c_photo, Prod.Photo.Url)
                            .Value(v => v.m_price, Prod.Price)
                            .Value(v => v.n_count, Count)
                            .Insert() > 0;
                    }
                }

                return false;
            }
        }

        public override OrderModel getBasketInfo(Guid OrderId)
        {
            using (var db = new CMSdb(_context))
            {
                if (OrderId != null && db.content_orderss.Where(w => w.id == OrderId).Any())
                {
                    var data = db.BasketInfo(OrderId);

                    if (data.Any())
                    {
                        var BasketInfo = data.Select(s => new OrderModel
                        {
                            ProdCount = (int)s.ProdCount,
                            Total = (decimal)s.TotalSum
                        });

                        return BasketInfo.FirstOrDefault();
                    }
                }
            }

            return null;
        }

        public override ProductModel[] getBasketItems(Guid OrderId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_order_detailss
                    .Where(w => w.f_order == OrderId)
                    .Join(db.content_productss, o => o.f_prod_id, p => p.id, (o, p) => p)
                    .OrderByDescending(o => o.d_date);

                return query.Select(s => new ProductModel
                {
                    Id = s.id,
                    Title = s.c_title,
                    Code = s.c_code,
                    Barcode = s.c_barcode,
                    Price = (decimal)s.m_price,
                    Standart = s.c_standart,
                    Count = (int)s.n_count,
                    Photo = new Photo()
                    {
                        Url = s.c_photo
                    }
                }).ToArray();
            }
        }
    }
}
