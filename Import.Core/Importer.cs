using AutoMapper;
using Import.Core.Helpers;
using Import.Core.Models;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Import.Core
{
    /// <summary>
    /// Логика импорта
    /// </summary>
    public static class Importer
    {
        /// <summary>
        /// Строка подключения
        /// </summary>
        private static string connection = "cmsdbConnection";

        /// <summary>
        /// Кол-во продуктов
        /// </summary>
        public static int CountProducts { get; set; }

        /// <summary>
        /// Проценты
        /// </summary>
        public static int Percent { get; set; } = 0;

        /// <summary>
        /// Шаг
        /// </summary>
        public static int Step { get; set; }

        /// <summary>
        /// Флаг завершённости
        /// </summary>
        public static bool IsCompleted { get; set; } = false;

        /// <summary>
        /// кол-во успешных процессов
        /// </summary>
        private static int countSuccess = 0;

        /// <summary>
        /// кол-во процессов, завершившихся неудачей
        /// </summary>
        private static int countFalse = 0;

        private static ProductModel[] distinctProducts = null;

        /// <summary>
        /// Конструктор
        /// </summary>
        static Importer()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CatalogModel, import_catalogs>()
                   .ForMember(d => d.id, opt => opt.MapFrom(src => src.Id))
                   .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Title))
                   .ForMember(d => d.c_alias, opt => opt.MapFrom(src => Transliteration.Translit(src.Title)))
                   .ForMember(d => d.d_date, opt => opt.MapFrom(src => DateTime.Now))
                   .ForMember(d => d.uui_parent, opt => opt.MapFrom(src => src.ParentId.Equals("0") ? Guid.Empty : Guid.Parse(src.ParentId)));
                cfg.CreateMap<ProductModel, import_products>()
                   .ForMember(d => d.id, opt => opt.MapFrom(src => src.Id))
                   .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Title))
                   .ForMember(d => d.c_code, opt => opt.MapFrom(src => src.Code))
                   .ForMember(d => d.n_count, opt => opt.MapFrom(src => src.Count))
                   .ForMember(d => d.d_date, opt => opt.MapFrom(src => src.Date));
                cfg.CreateMap<Barcode, import_product_barcodes>()
                    .ForMember(d => d.f_product, opt => opt.MapFrom(src => src.ProductId))
                    .ForMember(d => d.c_value, opt => opt.MapFrom(src => src.Value));
                cfg.CreateMap<Price, import_product_prices>()
                    .ForMember(d => d.f_product, opt => opt.MapFrom(src => src.ProductId))
                    .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Title))
                    .ForMember(d => d.m_value, opt => opt.MapFrom(src => !String.IsNullOrEmpty(src.Value) ? Decimal.Parse(src.Value.Replace(".", ",")) : 0));
                cfg.CreateMap<Image, import_product_images>()
                   .ForMember(d => d.f_product, opt => opt.MapFrom(src => src.ProductId))
                   .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Name))
                   .ForMember(d => d.b_main, opt => opt.MapFrom(src => src.IsMain));
                cfg.CreateMap<Certificate, import_product_certificates>()
                   .ForMember(d => d.f_product, opt => opt.MapFrom(src => src.ProductId))
                   .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Name))
                   .ForMember(d => d.b_hygienic, opt => opt.MapFrom(src => src.IsHygienic));
                cfg.CreateMap<CatalogProductLink, import_product_categories>()
                   .ForMember(d => d.f_product, opt => opt.MapFrom(src => src.ProductId))
                   .ForMember(d => d.f_catalog, opt => opt.MapFrom(src => src.CatalogId));
            });
        }

        /// <summary>
        /// Основной метод
        /// </summary>
        public static void DoImport(FileInfo[] files)
        {
            Preparing();

            if (files != null)
            {
                files = files.OrderBy(o => o.FullName)
                             .Select(s => s).ToArray();

                using (var db = new dbModel(connection))
                {
                    foreach (var file in files)
                    {
                        SrvcLogger.Debug("{preparing}", "файл для импорта данных '" + file.FullName + "'");
                        SrvcLogger.Debug("{preparing}", "начало чтения XML-данных");

                        using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open))
                        {
                            SrvcLogger.Debug("{preparing}", String.Format("XML-данные успешно прочитаны из файла {0}", file.Name));

                            #region comments
                            //using (var tr = db.BeginTransaction())
                            //{
                            //if (file.FullName.Contains("catalog"))
                            //{
                            //try
                            //{
                            //    SrvcLogger.Debug("{work}", "категории начало");
                            //    var serializer = new XmlSerializer(typeof(CatalogList));
                            //    var arrayOfCatalogs = (CatalogList)serializer.Deserialize(fileStream);
                            //    var distinctCatalogs = (from c in arrayOfCatalogs.Catalogs
                            //                            select c).GroupBy(g => g.Id)
                            //                                     .Select(s => s.First()).ToArray();
                            //    var catalogs = Mapper.Map<List<import_catalogs>>(distinctCatalogs);
                            //    AddCategories(db, catalogs);
                            //    SrvcLogger.Debug("{work}", "категории конец");
                            //    countSuccess++;
                            //}
                            //catch (Exception e)
                            //{
                            //    SrvcLogger.Error("{error}", "ошибка при импорте каталогов");
                            //    SrvcLogger.Error("{error}", e.ToString());
                            //    countFalse++;
                            //}
                            //}
                            //else if (file.FullName.Contains("product"))
                            //{
                            //try
                            //{
                            #region comments
                            //SrvcLogger.Debug("{work}", "продукция начало");
                            //var serializer = new XmlSerializer(typeof(ArrayOfProducts));
                            //var arrayOfProducts = (ArrayOfProducts)serializer.Deserialize(fileStream);
                            //var distinctProducts = (from p in arrayOfProducts.Products
                            //                        select p).GroupBy(g => g.Id)
                            //                                 .Select(s => s.First()).ToArray();
                            //var products = Mapper.Map<List<import_products>>(distinctProducts);
                            //AddProducts(db, products);
                            //SrvcLogger.Debug("{work}", "продукция конец");

                            //AddProducts(fileStream, db);
                            #endregion
                            #endregion

                            InsertHelper helper = new InsertHelper
                            {
                                FileStream = fileStream,
                                Db = db,
                                Entity = Entity.Catalogs
                            };

                            if (file.FullName.Contains("catalog"))
                            {
                                InsertWithLogging(helper);
                            }
                            else
                            {
                                foreach (Entity entity in Enum.GetValues(typeof(Entity)))
                                {
                                    if (!entity.Equals(Entity.Catalogs))
                                    {
                                        helper.Entity = entity;
                                        InsertWithLogging(helper);
                                    }
                                }
                            }

                            #region comments
                            #region связи штрих-кодов и товаров
                            //SrvcLogger.Debug("{work}", "связи штрих-кодов и товаров начало");
                            //var queryBarcodeList = (from p in distinctProducts
                            //                        select new { p.Id, p.BarcodeList })
                            //                        .Select(s => new
                            //                        {
                            //                            List = s.BarcodeList
                            //                            .Select(g => new Barcode
                            //                            {
                            //                                ProductId = s.Id,
                            //                                Value = g.Value
                            //                            }).ToArray()
                            //                        })
                            //                        .SelectMany(s => s.List)
                            //                        .ToArray();

                            //var barcodeProdLinks = Mapper.Map<List<import_product_barcodes>>(queryBarcodeList);
                            //AddBarcodeProdLinks(db, barcodeProdLinks);
                            //SrvcLogger.Debug("{work}", "связи штрих-кодов и товаров конец");
                            #endregion

                            #region связи цен и товаров
                            //SrvcLogger.Debug("{work}", "связи цен и товаров начало");
                            //var queryPriceList = (from p in distinctProducts
                            //                      select new { p.Id, p.PriceList })
                            //                      .Select(s => new
                            //                      {
                            //                          List = s.PriceList
                            //                          .Select(g => new Price
                            //                          {
                            //                              ProductId = s.Id,
                            //                              Title = g.Title,
                            //                              Value = g.Value.Replace(".", ",")
                            //                          }).ToArray()
                            //                      })
                            //                      .SelectMany(s => s.List)
                            //                      .ToArray();

                            //var uniquePriceProds = queryPriceList
                            //    .GroupBy(g => new { g.ProductId, g.Title, g.Value }, (key, group) => new Price
                            //    {
                            //        ProductId = key.ProductId,
                            //        Title = key.Title,
                            //        Value = key.Value
                            //    });

                            //var priceProdLinks = Mapper.Map<List<import_product_prices>>(uniquePriceProds);
                            //AddPriceProdLinks(db, priceProdLinks);
                            //SrvcLogger.Debug("{work}", "связи цен и товаров конец");
                            #endregion

                            #region связи изображений и товаров
                            //SrvcLogger.Debug("{work}", "связи изображений и товаров начало");
                            //var queryImageList = (from p in distinctProducts
                            //                      select new { p.Id, p.ImageList })
                            //                      .Select(s => new
                            //                      {
                            //                          List = s.ImageList
                            //                          .Select(g => new Image
                            //                          {
                            //                              ProductId = s.Id,
                            //                              Name = g.Name,
                            //                              IsMain = g.IsMain
                            //                          }).ToArray()
                            //                      })
                            //                      .SelectMany(s => s.List)
                            //                      .ToArray();

                            //var imageProdLinks = Mapper.Map<List<import_product_images>>(queryImageList);
                            //AddImageProdLinks(db, imageProdLinks);
                            //SrvcLogger.Debug("{work}", "связи изображений и товаров конец");
                            #endregion

                            #region связи сертификатов и товаров
                            //SrvcLogger.Debug("{work}", "связи сертификатов и товаров начало");
                            //var queryCertificateList = (from p in distinctProducts
                            //                            select new { p.Id, p.Certificates })
                            //                            .Select(s => new
                            //                            {
                            //                                List = s.Certificates
                            //                                .Select(g => new Certificate
                            //                                {
                            //                                    ProductId = s.Id,
                            //                                    Name = g.Name,
                            //                                    IsHygienic = g.IsHygienic
                            //                                }).ToArray()
                            //                            })
                            //                            .SelectMany(s => s.List)
                            //                            .ToArray();

                            //var certificateProdLinks = Mapper.Map<List<import_product_certificates>>(queryCertificateList);
                            //AddCertificateProdLinks(db, certificateProdLinks);
                            //SrvcLogger.Debug("{work}", "связи сертификатов и товаров конец");
                            #endregion

                            #region связи категорий и товаров
                            //SrvcLogger.Debug("{work}", "связи категорий и товаров начало");
                            //var queryCatalogList = (from p in distinctProducts
                            //                        select new { p.Id, p.Categories })
                            //                        .Select(s => new
                            //                        {
                            //                            List = s.Categories
                            //                                .Select(g => new CatalogProductLink
                            //                                {
                            //                                    ProductId = s.Id,
                            //                                    CatalogId = g.Id
                            //                                }).ToArray()
                            //                        })
                            //                        .SelectMany(s => s.List)
                            //                        .Distinct()
                            //                        .ToArray();

                            //var uniqueCatalogProds = queryCatalogList
                            //    .GroupBy(g => new { g.CatalogId, g.ProductId }, (key, group) => new CatalogProductLink
                            //    {
                            //        ProductId = key.ProductId,
                            //        CatalogId = key.CatalogId
                            //    });

                            //var catalogProdLinks = Mapper.Map<List<import_product_categories>>(uniqueCatalogProds);
                            //AddCatalogProdLinks(db, catalogProdLinks);
                            //SrvcLogger.Debug("{work}", "связи категорий и товаров конец");
                            #endregion

                            //countSuccess++;
                            //}
                            //catch (Exception e)
                            //{
                            //    SrvcLogger.Error("{error}", "ошибка при импорте продукции");
                            //    SrvcLogger.Error("{error}", e.ToString());
                            //    countFalse++;
                            //}
                            //}
                            //}
                            #endregion
                        }
                    }
                    SrvcLogger.Debug("{work}", "запуск переноса данных из буферных таблиц");
                    Finalizer(db);
                    SrvcLogger.Debug("{work}", "импорт завершён");
                    SrvcLogger.Debug("{work}", String.Format("кол-во ошибок {0}", countFalse));
                    SrvcLogger.Debug("{work}", String.Format("кол-во успешных процессов {0}", countSuccess));
                }
            }
        }

        /// <summary>
        /// Обнуляем значения свойств и чистим буферный таблицы перед новым импортом
        /// </summary>
        private static void Preparing()
        {
            distinctProducts = null;
            countSuccess = countFalse = 0;
            if (!IsCompleted)
            {
                Percent = Step = CountProducts = 0;
            }

            using (var db = new dbModel(connection))
            {
                db.import_catalogss.Delete();
                db.import_productss.Delete();
            }
        }

        /// <summary>
        /// Попытка вставки списка данных с логированием
        /// </summary>
        /// <param name="insert"></param>
        private static void InsertWithLogging(InsertHelper insert)
        {
            string title = insert.Entity.ToString().ToLower();
            try
            {
                SrvcLogger.Debug("{work}", String.Format("{0} начало", title));

                switch (insert.Entity)
                {
                    case Entity.Catalogs:
                        AddCategories(insert);
                        break;
                    case Entity.Products:
                        distinctProducts = AddProducts(insert);
                        break;
                    case Entity.CatalogProductLinks:
                        AddCatalogProdLinks(insert);
                        break;
                    case Entity.Barcodes:
                        AddBarcodeProdLinks(insert);
                        break;
                    case Entity.Prices:
                        AddPriceProdLinks(insert);
                        break;
                    case Entity.Images:
                        AddImageProdLinks(insert);
                        break;
                    case Entity.Certificates:
                        AddCertificateProdLinks(insert);
                        break;
                }

                SrvcLogger.Debug("{work}", String.Format("{0} конец", title));
                countSuccess++;
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", String.Format("ошибка при импорте {0}", title));
                SrvcLogger.Error("{error}", e.ToString());
                countFalse++;
            }
        }

        /// <summary>
        /// Добавляет список сущностей в таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="list"></param>
        private static void AddEntities<T>(EntityHelper<T> entity)
        {
            SrvcLogger.Debug("{work}", String.Format("кол-во {0}: {1}", entity.Title, entity.List.Count()));
            try
            {
                using (var tr = entity.Db.BeginTransaction())
                {
                    entity.Db.BulkCopy(entity.List);
                    tr.Commit();
                }
                countSuccess++;
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", e.ToString());
                countFalse++;
            }
        }

        /// <summary>
        /// Добавляет категории
        /// </summary>
        /// <param name="db"></param>
        /// <param name="catalogs"></param>
        private static void AddCategories(InsertHelper insert)
        {
            var serializer = new XmlSerializer(typeof(CatalogList));
            var arrayOfCatalogs = (CatalogList)serializer.Deserialize(insert.FileStream);
            var distinctCatalogs = (from c in arrayOfCatalogs.Catalogs
                                    select c).GroupBy(g => g.Id)
                                             .Select(s => s.First()).ToArray();

            var list = Mapper.Map<List<import_catalogs>>(distinctCatalogs);
            string title = insert.Entity.ToString().ToLower();
            var entity = new EntityHelper<import_catalogs>
            {
                Db = insert.Db,
                List = list,
                Title = title
            };
            AddEntities(entity);
        }

        /// <summary>
        /// Добавляет продукцию
        /// </summary>
        /// <param name="db"></param>
        /// <param name="products"></param>
        private static ProductModel[] AddProducts(InsertHelper insert)
        {
            ProductModel[] result = null;
            var serializer = new XmlSerializer(typeof(ArrayOfProducts));
            var arrayOfProducts = (ArrayOfProducts)serializer.Deserialize(insert.FileStream);
            var distinctProducts = (from p in arrayOfProducts.Products
                                    select p).GroupBy(g => g.Id)
                                             .Select(s => s.First()).ToArray();

            var list = Mapper.Map<List<import_products>>(distinctProducts);
            string title = insert.Entity.ToString().ToLower();
            var entity = new EntityHelper<import_products>
            {
                Db = insert.Db,
                List = list,
                Title = title
            };
            AddEntities(entity);
            result = distinctProducts;
            return result;
        }

        /// <summary>
        /// Добавляет связи категорий с товарами
        /// </summary>
        /// <param name="db"></param>
        /// <param name="links"></param>
        private static void AddCatalogProdLinks(InsertHelper insert)
        {
            var queryCatalogList = (from p in distinctProducts
                                    select new { p.Id, p.Categories })
                                                            .Select(s => new
                                                            {
                                                                List = s.Categories
                                                                    .Select(g => new CatalogProductLink
                                                                    {
                                                                        ProductId = s.Id,
                                                                        CatalogId = g.Id
                                                                    }).ToArray()
                                                            })
                                                            .SelectMany(s => s.List)
                                                            .Distinct()
                                                            .ToArray();

            var uniqueCatalogProds = queryCatalogList
                .GroupBy(g => new { g.CatalogId, g.ProductId }, (key, group) => new CatalogProductLink
                {
                    ProductId = key.ProductId,
                    CatalogId = key.CatalogId
                });

            var list = Mapper.Map<List<import_product_categories>>(uniqueCatalogProds);
            string title = insert.Entity.ToString().ToLower();
            var entity = new EntityHelper<import_product_categories>
            {
                Db = insert.Db,
                List = list,
                Title = title
            };
            AddEntities(entity);
        }

        /// <summary>
        /// Добавляет связи штрих-кодов с товарами
        /// </summary>
        /// <param name="db"></param>
        /// <param name="links"></param>
        private static void AddBarcodeProdLinks(InsertHelper insert)
        {
            var queryBarcodeList = (from p in distinctProducts
                                    select new { p.Id, p.BarcodeList })
                                                            .Select(s => new
                                                            {
                                                                List = s.BarcodeList
                                                                .Select(g => new Barcode
                                                                {
                                                                    ProductId = s.Id,
                                                                    Value = g.Value
                                                                }).ToArray()
                                                            })
                                                            .SelectMany(s => s.List)
                                                            .ToArray();

            var list = Mapper.Map<List<import_product_barcodes>>(queryBarcodeList);
            string title = insert.Entity.ToString().ToLower();
            var entity = new EntityHelper<import_product_barcodes>
            {
                Db = insert.Db,
                List = list,
                Title = title
            };
            AddEntities(entity);
        }

        /// <summary>
        /// Добавляет связи цен с товарами
        /// </summary>
        /// <param name="db"></param>
        /// <param name="links"></param>
        private static void AddPriceProdLinks(InsertHelper insert)
        {
            var queryPriceList = (from p in distinctProducts
                                  select new { p.Id, p.PriceList })
                                                  .Select(s => new
                                                  {
                                                      List = s.PriceList
                                                      .Select(g => new Price
                                                      {
                                                          ProductId = s.Id,
                                                          Title = g.Title,
                                                          Value = g.Value.Replace(".", ",")
                                                      }).ToArray()
                                                  })
                                                  .SelectMany(s => s.List)
                                                  .ToArray();

            var uniquePriceProds = queryPriceList
                .GroupBy(g => new { g.ProductId, g.Title, g.Value }, (key, group) => new Price
                {
                    ProductId = key.ProductId,
                    Title = key.Title,
                    Value = key.Value
                });

            var list = Mapper.Map<List<import_product_prices>>(uniquePriceProds);
            string title = insert.Entity.ToString().ToLower();
            var entity = new EntityHelper<import_product_prices>
            {
                Db = insert.Db,
                List = list,
                Title = title
            };
            AddEntities(entity);
        }

        /// <summary>
        /// Добавляет связи изображений с товарами
        /// </summary>
        /// <param name="db"></param>
        /// <param name="links"></param>
        private static void AddImageProdLinks(InsertHelper insert)
        {
            var queryImageList = (from p in distinctProducts
                                  select new { p.Id, p.ImageList })
                                                  .Select(s => new
                                                  {
                                                      List = s.ImageList
                                                      .Select(g => new Image
                                                      {
                                                          ProductId = s.Id,
                                                          Name = g.Name,
                                                          IsMain = g.IsMain
                                                      }).ToArray()
                                                  })
                                                  .SelectMany(s => s.List)
                                                  .ToArray();

            var list = Mapper.Map<List<import_product_images>>(queryImageList);
            string title = insert.Entity.ToString().ToLower();
            var entity = new EntityHelper<import_product_images>
            {
                Db = insert.Db,
                List = list,
                Title = title
            };
            AddEntities(entity);
        }

        /// <summary>
        /// Добавляет связи сертификатов с товарами
        /// </summary>
        /// <param name="db"></param>
        /// <param name="links"></param>
        private static void AddCertificateProdLinks(InsertHelper insert)
        {
            var queryCertificateList = (from p in distinctProducts
                                        select new { p.Id, p.Certificates })
                                                        .Select(s => new
                                                        {
                                                            List = s.Certificates
                                                            .Select(g => new Certificate
                                                            {
                                                                ProductId = s.Id,
                                                                Name = g.Name,
                                                                IsHygienic = g.IsHygienic
                                                            }).ToArray()
                                                        })
                                                        .SelectMany(s => s.List)
                                                        .ToArray();

            var list = Mapper.Map<List<import_product_certificates>>(queryCertificateList);
            string title = insert.Entity.ToString().ToLower();
            var entity = new EntityHelper<import_product_certificates>
            {
                Db = insert.Db,
                List = list,
                Title = title
            };
            AddEntities(entity);
        }

        /// <summary>
        /// Запускает хранимку для переноса данных из буферных таблиц в боевые
        /// </summary>
        /// <param name="db"></param>
        private static void Finalizer(dbModel db)
        {
            try
            {
                using (var tr = db.BeginTransaction())
                {
                    db.import();
                }
                countSuccess++;
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", e.ToString());
                countFalse++;
            }
        }
    }
}
