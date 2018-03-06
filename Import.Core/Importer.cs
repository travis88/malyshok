using AutoMapper;
using Import.Core.Models;
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
                    Mapper.Initialize(cfg => cfg.CreateMap<import_products, ProductModel>()
                                        .ForMember(d => d.Title, opt => opt.MapFrom(src => src.c_title))
                                        .ForMember(d => d.Code, opt => opt.MapFrom(src => src.c_code))
                                        .ForMember(d => d.Barcode, opt => opt.MapFrom(src => src.c_barcode))
                                        .ForMember(d => d.Count, opt => opt.MapFrom(src => src.n_count))
                                        .ForMember(d => d.Price, opt => opt.MapFrom(src => src.m_price))
                                        .ForMember(d => d.Date, opt => opt.MapFrom(src => src.d_date))
                                        .ForMember(d => d.Standart, opt => opt.MapFrom(src => src.c_standart)));

                    #region продукция
                    try
                    {
                        AddProducts(db, arrayOfProducts.Products);
                    }
                    catch (Exception e)
                    {
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
        private static void AddProducts(dbModel db, IEnumerable<ProductModel> products)
        {
            //foreach (var org in distinctOrgs)
            //{
            //    Guid id = org.ID; // идентификатор
            //    string name = org.Name; // название
            //                            //string oid = org.OID; // OID
            //    string kpp = org.KPP; // кпп
            //                          //string orgn = org.OGRN; // OGRN

            //    list.Add(new ImportFrmpOrgs
            //    {
            //        Guid = id,
            //        CName = name,
            //        DModify = DateTime.Now
            //    });
            //}

            //// добавляем организации
            //db.BulkCopy(list);
            

        }
    }
}
