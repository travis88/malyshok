using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с продукцией
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Возвращает список продукции с пейджером
        /// </summary>
        /// <returns></returns>
        public override ProductList getProducts(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                IQueryable<content_products> query = db.content_productss;

                if (!String.IsNullOrWhiteSpace(filter.Category))
                {
                    query = query
                        .Where(w => w.contentproductcategorieslinkscontentproductss
                                        .Any(a => a.contentproductcategorieslinkscontentcategories1.id
                                                .Equals(Guid.Parse(filter.Category))));
                }
                int itemCount = query.Count();

                var list = query
                    .OrderByDescending(o => o.d_date)
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new ProductModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Code = s.c_code,
                        Barcode = s.c_barcode,
                        Date = s.d_date,
                        Photo = $"/prodcontent/{s.c_barcode}/{s.c_photo}",
                        Categories = s.contentproductcategorieslinkscontentproductss
                            .Select(d => new CategoryModel
                            {
                                Id = d.contentproductcategorieslinkscontentcategories1.id,
                                Title = d.contentproductcategorieslinkscontentcategories1.c_title
                            }).ToArray()
                    }).ToArray();


                if (list.Any())
                {
                    return new ProductList
                    {
                        Data = list.ToArray(),
                        Pager = new Pager
                        {
                            page = filter.Page,
                            size = filter.Size,
                            items_count = itemCount,
                            page_count = (itemCount % filter.Size > 0)
                                            ? (itemCount / filter.Size) + 1
                                            : itemCount / filter.Size
                        }
                    };
                }
                else return null;
            }
        }

        /// <summary>
        /// Возвращает единичную запись продукта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ProductModel getProduct(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = (from p in db.content_productss
                             join l in db.content_product_categories_linkss on p.id equals l.f_product
                             join c in db.content_categoriess on l.f_category equals c.id
                             where p.id.Equals(id)
                             select new { p, c });

                return query.ToArray()
                    .GroupBy(o => new { o.p.id })
                    .Select(s => new ProductModel
                    {
                        Id = s.Key.id,
                        Title = s.First().p.c_title,
                        Code = s.First().p.c_code,
                        Barcode = s.First().p.c_barcode,
                        Description = s.First().p.c_description,
                        Keyword = s.First().p.c_keyword,
                        Photo = s.First().p.c_photo,
                        Count = (int)s.First().p.n_count,
                        Price = (decimal)s.First().p.m_price,
                        Date = s.First().p.d_date,
                        Standart = s.First().p.c_standart,
                        Categories = s.Select(c => new CategoryModel
                        {
                            Id = c.c.id,
                            Title = c.c.c_title
                        }).ToArray()
                    }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Обновляет список категорий для продукта
        /// </summary>
        /// <param name="db"></param>
        /// <param name="prodId"></param>
        /// <param name="cats"></param>
        private void dbUpdateProductCategories(CMSdb db, Guid prodId, Guid[] cats)
        {
            // удаляем старые категории
            db.content_product_categories_linkss
                .Where(w => w.f_product.Equals(prodId))
                .Delete();

            // привязываем новые категории
            if (cats != null)
            {
                foreach (var cat in cats)
                {
                    if (!cat.Equals(Guid.Empty))
                    {
                        var prodCategory = new content_product_categories_links
                        {
                            f_product = prodId,
                            f_category = cat
                        };

                        db.Insert(prodCategory);
                    }
                }
            }
        }

        /// <summary>
        /// Обновляет данные по продукту
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool updateProduct(ProductModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool result = db.content_productss
                        .Where(w => w.id.Equals(item.Id))
                        .Set(s => s.c_title, item.Title)
                        .Set(s => s.c_code, item.Code)
                        .Set(s => s.c_barcode, item.Barcode)
                        .Set(s => s.c_description, item.Description)
                        .Set(s => s.c_keyword, item.Keyword)
                        .Set(s => s.n_count, item.Count)
                        .Set(s => s.c_photo, item.Photo)
                        .Set(s => s.d_date, item.Date)
                        .Set(s => s.c_standart, item.Standart)
                        .Set(s => s.m_price, item.Price)
                        .Update() > 0;

                    dbUpdateProductCategories(db, item.Id, item.Categories.Select(s => s.Id).ToArray());

                    tr.Commit();

                    return result;
                }

            }
        }

        /// <summary>
        /// Добавляет новый продукт
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool insertProduct(ProductModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool result = db.content_productss
                        .Value(v => v.id, item.Id)
                        .Value(v => v.c_title, item.Title)
                        .Value(v => v.c_code, item.Code)
                        .Value(v => v.c_barcode, item.Barcode)
                        .Value(v => v.c_description, item.Description)
                        .Value(v => v.c_keyword, item.Keyword)
                        .Value(v => v.c_photo, item.Photo)
                        .Value(v => v.n_count, item.Count)
                        .Value(v => v.m_price, item.Price)
                        .Value(v => v.d_date, item.Date)
                        .Value(v => v.c_standart, item.Standart)
                        .Insert() > 0;

                    dbUpdateProductCategories(db, item.Id, item.Categories.Select(s => s.Id).ToArray());

                    tr.Commit();

                    return result;
                }
            }
        }

        /// <summary>
        /// Удаляет продукт
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool deleteProduct(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_productss
                    .Where(w => w.id.Equals(id))
                    .Delete() > 0;
            }
        }
    }
}
