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
        public static void DoImport(Stream xml)
        {
            if (!IsCompleted)
            {
                Percent = Step = CountProducts = 0;
            }

            //SrvcLogger.Debug("{PREPARING}", "Начало чтения XML-данных");

            XmlSerializer serializer = new XmlSerializer(typeof(ArrayOfProducts));

            //SrvcLogger.Debug("{PREPARING}", "Данные успешно прочитаны из файла");

            var arrayOfProducts = (ArrayOfProducts)serializer.Deserialize(xml);


            if (arrayOfProducts != null && arrayOfProducts.Products != null)
            {
                //int count = arrayOfProducts.Products.Count();
                //SrvcLogger.Debug("{PREPARING}", String.Format("Кол-во записей {0}", count));
                //CountProducts = count;

                #region comment
                //for (int i = 0; i <= 100; i++)
                //{
                //    Thread.Sleep(250);
                //    Percent = i;

                //    if (i == 100)
                //    {
                //        i = 0;
                //        Step++;
                //        if (Step == 3)
                //        {
                //            IsCompleted = true;
                //            break;
                //        }
                //    }
                //}
                #endregion

                using (var db = new dbModel(connection))
                {
                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<ProductModel, import_products>()
                           .ForMember(d => d.id, opt => opt.MapFrom(src => src.Id))
                           .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Title))
                           .ForMember(d => d.c_code, opt => opt.MapFrom(src => src.Code))
                           .ForMember(d => d.c_barcode, opt => opt.MapFrom(src => src.Barcode))
                           .ForMember(d => d.n_count, opt => opt.MapFrom(src => src.Count))
                           .ForMember(d => d.m_price, opt => opt.MapFrom(src => src.Price))
                           .ForMember(d => d.d_date, opt => opt.MapFrom(src => src.Date))
                           .ForMember(d => d.c_standart, opt => opt.MapFrom(src => src.Standart));

                        cfg.CreateMap<CategoryModel, import_catalogs>()
                           .ForMember(d => d.id, opt => opt.MapFrom(src => src.Id))
                           .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Title))
                           .ForMember(d => d.c_alias, opt => opt.MapFrom(src => Transliteration.Translit(src.Title)));
                    });

                    var products = Mapper.Map<List<import_products>>(arrayOfProducts.Products);

                    #region продукция
                    try
                    {
                        AddProducts(db, products);
                    }
                    catch (Exception e)
                    {
                        SrvcLogger.Error("{error}", "Ошибка при добавлении продукции");
                        SrvcLogger.Error("{error}", e.ToString());
                    }
                    #endregion

                    #region категории
                    try
                    {

                    }
                    catch (Exception e)
                    {
                        SrvcLogger.Error("{error}", "Ошибка при добавлении категорий");
                        SrvcLogger.Error("{error}", e.ToString());
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Добавляет продукцию
        /// </summary>
        /// <param name="products"></param>
        private static void AddProducts(dbModel db, IEnumerable<import_products> products)
        {
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
        /// <param name="categories"></param>
        private static void AddCategories(dbModel db, IEnumerable<import_catalogs> categories)
        {
            using (var tr = db.BeginTransaction())
            {
                db.BulkCopy(categories);
                tr.Commit();
            }
        }
    }
}
