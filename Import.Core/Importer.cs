using AutoMapper;
using Import.Core.Models;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            countSuccess = countFalse = 0;

            if (!IsCompleted)
            {
                Percent = Step = CountProducts = 0;
            }

            if (files != null)
            {
                files = files.OrderBy(o => o.FullName)
                             .Select(s => s).ToArray();

                foreach (var file in files)
                {
                    SrvcLogger.Debug("{preparing}", "файл для импорта данных '" + file.FullName + "'");
                    SrvcLogger.Debug("{preparing}", "начало чтения XML-данных");

                    using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open))
                    {
                        SrvcLogger.Debug("{preparing}", String.Format("XML-данные успешно прочитаны из файла {0}", file.Name));

                        using (var db = new dbModel(connection))
                        {
                            if (file.FullName.Contains("catalog"))
                            {
                                try
                                {
                                    SrvcLogger.Debug("{work}", "категории начало");
                                    var serializer = new XmlSerializer(typeof(CatalogList));
                                    var arrayOfCatalogs = (CatalogList)serializer.Deserialize(fileStream);
                                    var distinctCatalogs = (from c in arrayOfCatalogs.Catalogs
                                                            select c).GroupBy(g => g.Id)
                                                                     .Select(s => s.First()).ToArray();
                                    var catalogs = Mapper.Map<List<import_catalogs>>(distinctCatalogs);
                                    AddCategories(db, catalogs);
                                    SrvcLogger.Debug("{work}", "категории конец");
                                    countSuccess++;
                                }
                                catch (Exception e)
                                {
                                    SrvcLogger.Error("{error}", "ошибка при импорте каталогов");
                                    SrvcLogger.Error("{error}", e.ToString());
                                    countFalse++;
                                }
                            }
                            else if (file.FullName.Contains("product"))
                            {
                                try
                                {
                                    #region продукция
                                    SrvcLogger.Debug("{work}", "продукция начало");
                                    var serializer = new XmlSerializer(typeof(ArrayOfProducts));
                                    var arrayOfProducts = (ArrayOfProducts)serializer.Deserialize(fileStream);
                                    var distinctProducts = (from p in arrayOfProducts.Products
                                                            select p).GroupBy(g => g.Id)
                                                                     .Select(s => s.First()).ToArray();
                                    var products = Mapper.Map<List<import_products>>(distinctProducts);
                                    AddProducts(db, products);
                                    SrvcLogger.Debug("{work}", "продукция конец");
                                    #endregion

                                    #region связи штрих-кодов и товаров
                                    SrvcLogger.Debug("{work}", "связи штрих-кодов и товаров начало");
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

                                    var barcodeProdLinks = Mapper.Map<List<import_product_barcodes>>(queryBarcodeList);
                                    AddBarcodeProdLinks(db, barcodeProdLinks);
                                    SrvcLogger.Debug("{work}", "связи штрих-кодов и товаров конец");
                                    #endregion

                                    #region связи цен и товаров
                                    SrvcLogger.Debug("{work}", "связи цен и товаров начало");
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

                                    var priceProdLinks = Mapper.Map<List<import_product_prices>>(queryPriceList);
                                    AddPriceProdLinks(db, priceProdLinks);
                                    SrvcLogger.Debug("{work}", "связи цен и товаров конец");
                                    #endregion

                                    #region связи изображений и товаров
                                    SrvcLogger.Debug("{work}", "связи изображений и товаров начало");
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

                                    var imageProdLinks = Mapper.Map<List<import_product_images>>(queryImageList);
                                    AddImageProdLinks(db, imageProdLinks);
                                    SrvcLogger.Debug("{work}", "связи изображений и товаров конец");
                                    #endregion

                                    #region связи сертификатов и товаров
                                    SrvcLogger.Debug("{work}", "связи сертификатов и товаров начало");
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

                                    var certificateProdLinks = Mapper.Map<List<import_product_certificates>>(queryCertificateList);
                                    AddCertificateProdLinks(db, certificateProdLinks);
                                    SrvcLogger.Debug("{work}", "связи сертификатов и товаров конец");
                                    #endregion

                                    #region связи категорий и товаров
                                    SrvcLogger.Debug("{work}", "связи категорий и товаров начало");
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
                                                            .ToArray();

                                    var catalogProdLinks = Mapper.Map<List<import_product_categories>>(queryCatalogList);
                                    AddCatalogProdLinks(db, catalogProdLinks);
                                    SrvcLogger.Debug("{work}", "связи категорий и товаров конец");
                                    #endregion

                                    countSuccess++;
                                }
                                catch (Exception e)
                                {
                                    SrvcLogger.Error("{error}", "ошибка при импорте продукции");
                                    SrvcLogger.Error("{error}", e.ToString());
                                    countFalse++;
                                }
                            }

                        }
                    }
                }

                SrvcLogger.Debug("{work}", "запуск переноса данных из буферных таблиц");
                Finalizer();
                SrvcLogger.Debug("{work}", "импорт завершён");
                SrvcLogger.Debug("{work}", String.Format("кол-во ошибок {0}", countFalse));
                SrvcLogger.Debug("{work}", String.Format("кол-во успешных процессов {0}", countSuccess));
            }
        }

        /// <summary>
        /// Добавляет категории
        /// </summary>
        /// <param name="db"></param>
        /// <param name="catalogs"></param>
        private static void AddCategories(dbModel db, IEnumerable<import_catalogs> catalogs)
        {
            SrvcLogger.Debug("{work}", String.Format("кол-во каталогов {0}", catalogs.Count()));
            try
            {
                using (var tr = db.BeginTransaction())
                {
                    db.BulkCopy(catalogs);
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
        /// Добавляет продукцию
        /// </summary>
        /// <param name="db"></param>
        /// <param name="products"></param>
        private static void AddProducts(dbModel db, IEnumerable<import_products> products)
        {
            SrvcLogger.Debug("{work}", String.Format("кол-во товаров {0}", products.Count()));
            try
            {
                using (var tr = db.BeginTransaction())
                {
                    db.BulkCopy(products);
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
        /// Добавляет связи изображений с товарами
        /// </summary>
        /// <param name="db"></param>
        /// <param name="links"></param>
        private static void AddImageProdLinks(dbModel db, IEnumerable<import_product_images> links)
        {
            SrvcLogger.Debug("{work}", String.Format("кол-во изображений {0}", links.Count()));
            try
            {
                using (var tr = db.BeginTransaction())
                {
                    db.BulkCopy(links);
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
        /// Добавляет связи сертификатов с товарами
        /// </summary>
        /// <param name="db"></param>
        /// <param name="links"></param>
        private static void AddCertificateProdLinks(dbModel db, IEnumerable<import_product_certificates> links)
        {
            SrvcLogger.Debug("{work}", String.Format("кол-во сертификатов {0}", links.Count()));
            try
            {
                using (var tr = db.BeginTransaction())
                {
                    db.BulkCopy(links);
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
        /// Добавляет связи штрих-кодов с товарами
        /// </summary>
        /// <param name="db"></param>
        /// <param name="links"></param>
        private static void AddBarcodeProdLinks(dbModel db, IEnumerable<import_product_barcodes> links)
        {
            SrvcLogger.Debug("{work}", String.Format("кол-во штрих-кодов {0}", links.Count()));
            try
            {
                using (var tr = db.BeginTransaction())
                {
                    db.BulkCopy(links);
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
        /// Добавляет связи цен с товарами
        /// </summary>
        /// <param name="db"></param>
        /// <param name="links"></param>
        private static void AddPriceProdLinks(dbModel db, IEnumerable<import_product_prices> links)
        {
            SrvcLogger.Debug("{work}", String.Format("кол-во цен {0}", links.Count()));
            try
            {
                using (var tr = db.BeginTransaction())
                {
                    db.BulkCopy(links);
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
        /// Добавляет связи категорий с товарами
        /// </summary>
        /// <param name="db"></param>
        /// <param name="links"></param>
        private static void AddCatalogProdLinks(dbModel db, IEnumerable<import_product_categories> links)
        {
            SrvcLogger.Debug("{work}", String.Format("кол-во каталогов {0}", links.Count()));
            try
            {
                using (var tr = db.BeginTransaction())
                {
                    db.BulkCopy(links);
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
        /// Запускает хранимку для переноса данных из буферных таблиц в боевые
        /// </summary>
        /// <param name="db"></param>
        private static void Finalizer()
        {
            using (var db = new dbModel(connection))
            {
                try
                {
                    db.import();
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
}
