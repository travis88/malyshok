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
        /// Основной метод
        /// </summary>
        public static void DoImport(FileInfo[] files)
        {
            if (!IsCompleted)
            {
                Percent = Step = CountProducts = 0;
            }

            if (files != null)
            {
                files = files.OrderBy(o => o.FullName)
                             .Select(s => s).ToArray();

                #region mapping
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<CatalogModel, import_catalogs>()
                       .ForMember(d => d.id, opt => opt.MapFrom(src => src.Id))
                       .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Title))
                       .ForMember(d => d.c_alias, opt => opt.MapFrom(src => Transliteration.Translit(src.Title)))
                       .ForMember(d => d.d_date, opt => opt.MapFrom(src => DateTime.Now))
                       .ForMember(d => d.n_parent, opt => opt.MapFrom(src => src.ParentId));
                    cfg.CreateMap<ProductModel, import_products>()
                       .ForMember(d => d.id, opt => opt.MapFrom(src => src.Id))
                       .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Title))
                       .ForMember(d => d.c_code, opt => opt.MapFrom(src => src.Code))
                       .ForMember(d => d.c_barcode, opt => opt.MapFrom(src => src.Barcode))
                       .ForMember(d => d.n_count, opt => opt.MapFrom(src => src.Count))
                       .ForMember(d => d.m_price, opt => opt.MapFrom(src => src.Price))
                       .ForMember(d => d.d_date, opt => opt.MapFrom(src => src.Date))
                       .ForMember(d => d.c_standart, opt => opt.MapFrom(src => src.Standart));
                });
                #endregion

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
                                    var serializer = new XmlSerializer(typeof(CatalogList));
                                    var arrayOfCatalogs = (CatalogList)serializer.Deserialize(fileStream);
                                    var distinctCatalogs = (from c in arrayOfCatalogs.Catalogs
                                                            select c).GroupBy(g => g.Id)
                                                                     .Select(s => s.First()).ToArray();
                                    var catalogs = Mapper.Map<List<import_catalogs>>(distinctCatalogs);
                                    AddCategories(db, catalogs);
                                }
                                catch (Exception e)
                                {
                                    SrvcLogger.Error("{error}", "ошибка при импорте каталогов");
                                    SrvcLogger.Error("{error}", e.ToString());
                                }
                            }
                            else if (file.FullName.Contains("product"))
                            {
                                try
                                {
                                    var serializer = new XmlSerializer(typeof(ArrayOfProducts));
                                    var arrayOfProducts = (ArrayOfProducts)serializer.Deserialize(fileStream);
                                    var distinctProducts = (from p in arrayOfProducts.Products
                                                            select p).GroupBy(g => g.Id)
                                                                     .Select(s => s.First()).ToArray();
                                    var products = Mapper.Map<List<import_products>>(distinctProducts);
                                    AddProducts(db, products);
                                }
                                catch (Exception e)
                                {
                                    SrvcLogger.Error("{error}", "ошибка при импорте продукции");
                                    SrvcLogger.Error("{error}", e.ToString());
                                }
                            }
                        }
                    }
                }
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
            using (var tr = db.BeginTransaction())
            {
                db.BulkCopy(products);
                tr.Commit();
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
            using (var tr = db.BeginTransaction())
            {
                db.BulkCopy(catalogs);
                tr.Commit();
            }
        }
    }
}
