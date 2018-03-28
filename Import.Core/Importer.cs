﻿using AutoMapper;
using Import.Core.Helpers;
using Import.Core.Models;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        /// Текущий шаг
        /// </summary>
        public static int Step { get; set; }

        /// <summary>
        /// Шаги
        /// </summary>
        public static string[] Steps = { "Загрузка файлов",
                                         "Запись в буфферные таблицы",
                                         "Совмещение данных",
                                         "Импорт завершён",
                                         "Рассылка оповещений" };

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
        /// Уникальные продукты
        /// </summary>
        private static ProductModel[] distinctProducts = null;

        /// <summary>
        /// Параметры для рассылки оповещения по результатам импорта
        /// </summary>
        private static EmailParamsHelper emailHelper = null;

        /// <summary>
        /// Текст письма
        /// </summary>
        private static string EmailBody = null;

        /// <summary>
        /// Время начала
        /// </summary>
        private static DateTime Begin;

        /// <summary>
        /// Время окончания
        /// </summary>
        private static DateTime End;

        /// <summary>
        /// Затраченное время
        /// </summary>
        public static string Total = "0 часов 0 минут 0 секунд 0 милисекунд";

        /// <summary>
        /// Логгирование
        /// </summary>
        public static List<string> Log = new List<string>();

        /// <summary>
        /// Словарь для логирования
        /// </summary>
        private static Dictionary<string, string> dictionary;

        /// <summary>
        /// Конструктор
        /// </summary>
        static Importer()
        {
            // маппинг объектов и сущностей
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CatalogModel, import_catalogs>()
                   .ForMember(d => d.id, opt => opt.MapFrom(src => src.Id))
                   .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Title))
                   .ForMember(d => d.c_alias, opt => opt.MapFrom(src => Transliteration.Translit(src.Title)))
                   .ForMember(d => d.d_date, opt => opt.MapFrom(src => DateTime.Now))
                   .ForMember(d => d.uui_parent, opt =>
                                    opt.MapFrom(src => src.ParentId.Equals("0")
                                    ? Guid.Empty : Guid.Parse(src.ParentId)));
                cfg.CreateMap<ProductModel, import_products>()
                   .ForMember(d => d.id, opt => opt.MapFrom(src => src.Id))
                   .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Title))
                   .ForMember(d => d.c_code, opt => opt.MapFrom(src => src.Code))
                   .ForMember(d => d.n_count, opt => opt.MapFrom(src => src.Count))
                   .ForMember(d => d.d_date, opt => opt.MapFrom(src => src.Date))
                   .ForMember(d => d.c_barcode, opt => opt.MapFrom(src => src.BarcodeList.Select(s => s.Value).FirstOrDefault()))
                   .ForMember(d => d.c_price_title, opt =>
                                    opt.MapFrom(src => src.PriceList.Select(s => s.Title).FirstOrDefault()))
                   .ForMember(d => d.m_price_value, opt =>
                                    opt.MapFrom(src => src.PriceList.Select(s => !String.IsNullOrEmpty(s.Value)
                                    ? Decimal.Parse(s.Value.Replace(".", ",")) : 0).FirstOrDefault()))
                   .ForMember(d => d.c_photo, opt => opt.MapFrom(src => src.ImageList
                                                                           .Where(w => w.IsMain || !w.IsMain)
                                                                           .Select(s => s.Name).FirstOrDefault()));
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
                        SrvcLogger.Debug("{preparing}", String.Format("файл для импорта данных '{0}'", file.FullName));
                        SrvcLogger.Debug("{preparing}", "начало чтения XML-данных");
                        Log.Insert(0, String.Format("Чтение файла: {0}", file.Name));

                        using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open))
                        {
                            SrvcLogger.Debug("{preparing}", String.Format("XML-данные успешно прочитаны из файла {0}", file.Name));
                            Log.Insert(0, String.Format("Данные прочитаны"));

                            var helper = new InsertHelper
                            {
                                FileStream = fileStream,
                                Db = db,
                                Entity = Entity.Catalogs
                            };

                            if (file.FullName.Contains("cat"))
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
                        }
                    }
                    Step = 2;
                    Percent = 40;
                    SrvcLogger.Debug("{work}", "запуск переноса данных из буферных таблиц");
                    Log.Insert(0, "Перенос данных из буферных таблиц");

                    Finalizer(db);

                    Step = 3;
                    Percent = 60;
                    string falses = String.Format("кол-во ошибок {0}", countFalse);
                    string successes = String.Format("кол-во успешных процессов {0}", countSuccess);
                    string completedMessage = String.Format("импорт завершён {0} {1}; {2} {3}",
                        Environment.NewLine, falses, Environment.NewLine, successes);
                    SrvcLogger.Debug("{work}", completedMessage);
                    Log.Insert(0, "Импорт завершён");

                    Step = 4;
                    Percent = 80;
                    EmailBody += completedMessage;
                    SendEmail(EmailBody, db);
                    Step = 5;
                    Percent = 100;

                    End = DateTime.Now;
                    var t = End - Begin;
                    Total = String.Format("{0} часов {1} минут {2} секунд {3} милисекунд", Math.Truncate(t.TotalHours),
                                           Math.Truncate(t.TotalMinutes), Math.Truncate(t.TotalSeconds), Math.Truncate((double)t.Milliseconds));
                }
            }
        }

        /// <summary>
        /// Обнуляем значения свойств перед новым импортом
        /// </summary>
        private static void Preparing()
        {
            try
            {
                using (var db = new dbModel(connection))
                {
                    CleaningTempTables(db);
                }

                distinctProducts = null;
                emailHelper = new EmailParamsHelper();
                EmailBody = String.Empty;
                Begin = DateTime.Now;
                countSuccess = countFalse = 0;
                Total = "0 часов 0 минут 0 секунд 0 милисекунд";
                Log = new List<string>();
                dictionary = new Dictionary<string, string>
                {
                    { "catalogs", "категории" },
                    { "products", "товары" },
                    { "catalogproductlinks", "связи категорий и товаров" },
                    { "images", "изображения" },
                    { "certificates", "сертификаты" },
                };
                if (!IsCompleted)
                {
                    Percent = Step = CountProducts = 0;
                }
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", e.ToString());
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
                string errorMessage = e.ToString();
                EmailBody += String.Format("<p>{0}</p>", errorMessage);
                SrvcLogger.Error("{error}", String.Format("ошибка при импорте {0}", title));
                SrvcLogger.Error("{error}", errorMessage);
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
            Log.Insert(0, String.Format("Кол-во {0}: {1}", dictionary[entity.Title], entity.List.Count()));

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
                string errorMessage = e.ToString();
                EmailBody += String.Format("<p>{0}</p>", errorMessage);
                SrvcLogger.Error("{error}", errorMessage);
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
            CountProducts = distinctProducts.Count();
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
                distinctProducts = null;

                using (var tr = db.BeginTransaction())
                {
                    db.import();
                    tr.Commit();
                }
                countSuccess++;
            }
            catch (Exception e)
            {
                string errorMessage = e.ToString();
                EmailBody += String.Format("<p>{0}</p>", errorMessage);
                SrvcLogger.Error("{error}", errorMessage);
                countFalse++;
            }
        }

        /// <summary>
        /// Рассылает оповещения
        /// </summary>
        /// <param name="body"></param>
        private static void SendEmail(string body, dbModel db)
        {
            try
            {
                SrvcLogger.Debug("{work}", "рассылка оповещения");
                Log.Insert(0, "Рассылка оповещений");

                var receiverEmails = GetAdminEmails(db);
                receiverEmails.AddRange(emailHelper.EmailTo);

                foreach (var emailTo in receiverEmails)
                {
                    var from = new MailAddress(emailHelper.EmailFromAddress, emailHelper.EmailFromName);
                    var to = new MailAddress(emailTo);

                    var message = new MailMessage(from, to);
                    message.Subject = "Импорт товаров Малышок-ПрессМарк";
                    message.Body = body;
                    message.IsBodyHtml = true;

                    var smtp = new SmtpClient(emailHelper.EmailHost, emailHelper.EmailPort);
                    smtp.Credentials = new NetworkCredential(emailHelper.EmailFromAddress, emailHelper.EmailPassword);
                    smtp.EnableSsl = emailHelper.EmailEnableSsl;

                    smtp.Send(message);
                }
                Log.Insert(0, "Рассылка оповещений завершена");
                SrvcLogger.Debug("{work}", "рассылка оповещения проведена");
            }
            catch (Exception e)
            {
                SrvcLogger.Debug("{error}", "рассылка оповещений завершилась ошибкой");
                SrvcLogger.Debug("{error}", e.ToString());
            }
        }

        /// <summary>
        /// Очищает буферные таблицы
        /// </summary>
        private static void CleaningTempTables(dbModel db)
        {
            try
            {
                using (var tr = db.BeginTransaction())
                {
                    db.import_productss.Delete();
                    db.import_catalogss.Delete();
                    db.import_product_categoriess.Delete();
                    db.import_product_certificatess.Delete();
                    db.import_product_imagess.Delete();
                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", e.ToString());
            }
        }

        /// <summary>
        /// Возвращает список email для получения информации по импорту
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static List<string> GetAdminEmails(dbModel db)
        {
            try
            {
                string email = db.cms_sitess
                    .Select(s => s.c_email).FirstOrDefault();

                return email.Split('|')
                    .Select(s => s.Trim()).ToList();
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", e.ToString());
                return null;
            }
        }
    }
}
