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
                    //Mapper.Initialize(cfg => cfg.CreateMap<CreateUserViewModel, User>()
                    //.ForMember("Name", opt => opt.MapFrom(c => c.FirstName + " " + c.LastName))
                    //.ForMember("Email", opt => opt.MapFrom(src => src.Login)))

                    Mapper.Initialize(confing =>
                    {
                        //config.CreateMap<Entities.Book, Models.BookDTO>();
                        //confing.CreateMap<pp, ProductModel[]>();
                    });

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

            //List<import_products> prods = new List<import_products>();

            //            Mapper.CreateMap<Src, Dest>()
            //.ForMember(d => d.UserName, opt => opt.MapFrom(/* ????? */));

            //AutoMapper.Mapper.Initialize(config =>
            //{
            //    config.CreateMap<Entities.Book, Models.BookDTO>();
            //    config.CreateMap<Models.BookDTO, Entities.Book>();
            //    config.CreateMap<Entities.Publisher, Models.PublisherDTO>();
            //    config.CreateMap<Models.PublisherDTO, Entities.Publisher>();
            //});

        }
    }
}
