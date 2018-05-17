using AutoMapper;
using Import.Core.Helpers;
using Import.Core.Models;
using Import.Core.Services;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Import.Core
{
    /// <summary>
    /// Логика импорта
    /// </summary>
    public static class Importer
    {
        /// <summary>
        /// Общее кол-во шагов
        /// </summary>
        private static int allStepsCount;

        /// <summary>
        /// Строка подключения
        /// </summary>
        private const string CONNECTION = "cmsdbConnection";

        /// <summary>
        /// Уникальные продукты
        /// </summary>
        private static Product[] distinctProducts = null;

        /// <summary>
        /// Параметры для рассылки оповещения по результатам импорта
        /// </summary>
        private static EmailParamsHelper emailHelper = null;

        /// <summary>
        /// Текст письма
        /// </summary>
        public static string EmailBody = null;

        /// <summary>
        /// Словарь для логирования
        /// </summary>
        private static Dictionary<string, string> dictionary;

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
        public static int Step { get; set; } = 0;

        /// <summary>
        /// Шаги
        /// </summary>
        public static List<string> Steps = new List<string>();

        /// <summary>
        /// Флаг завершённости
        /// </summary>
        public static bool IsCompleted { get; set; } = false;

        /// <summary>
        /// кол-во успешных процессов
        /// </summary>
        public static int CountSuccess = 0;

        /// <summary>
        /// кол-во процессов, завершившихся неудачей
        /// </summary>
        public static int CountFalse = 0;

        /// <summary>
        /// Затраченное время
        /// </summary>
        public static string Total = "0 час. 0 мин. 0 сек. 0 мс.";

        /// <summary>
        /// Логгирование
        /// </summary>
        public static List<string> Log = new List<string>();

        /// <summary>
        /// Конструктор
        /// </summary>
        static Importer()
        {
            // маппинг объектов и сущностей
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Catalog, import_catalogs>()
                   .ForMember(d => d.id, opt => opt.MapFrom(src => src.Id))
                   .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Title))
                   .ForMember(d => d.c_alias, opt => opt.MapFrom(src => Transliteration.Translit(src.Title)))
                   .ForMember(d => d.d_date, opt => opt.MapFrom(src => DateTime.Now))
                   .ForMember(d => d.uui_parent, opt =>
                                    opt.MapFrom(src => src.ParentId.Equals("0")
                                    ? Guid.Empty : Guid.Parse(src.ParentId)));
                cfg.CreateMap<Product, import_products>()
                   .ForMember(d => d.id, opt => opt.MapFrom(src => src.Id))
                   .ForMember(d => d.c_title, opt => opt.MapFrom(src => src.Title))
                   .ForMember(d => d.c_code, opt => opt.MapFrom(src => src.Code))
                   .ForMember(d => d.n_count, opt => opt.MapFrom(src => src.Count))
                   .ForMember(d => d.d_date, opt => opt.MapFrom(src => src.Date))
                   .ForMember(d => d.c_barcode, opt => opt.MapFrom(src => src.BarcodeList
                                                                             .Select(s => s.Value.Trim())
                                                                             .FirstOrDefault()))
                   .ForMember(d => d.c_price_title, opt =>
                                    opt.MapFrom(src => src.PriceList.Select(s => s.Title).FirstOrDefault()))
                   .ForMember(d => d.m_price_value, opt =>
                                    opt.MapFrom(src => src.PriceList.Select(s => !String.IsNullOrEmpty(s.Value)
                                    ? Decimal.Parse(s.Value.Replace(".", ",")) : 0).FirstOrDefault()))
                   .ForMember(d => d.c_photo, opt => opt.MapFrom(src => src.ImageList
                                                                           .Where(w => w.IsMain || !w.IsMain)
                                                                           .Select(s => s.Name.Trim().Replace(" ", ""))
                                                                           .FirstOrDefault()));
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
            ReceiverParamsHelper receiverParams = new ReceiverParamsHelper();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Preparing();

            if (files != null && files.Any(a => a != null))
            {
                Step++;
                files = files.Where(w => w != null).ToArray();
                var _files = FilesOrdering(files);
                SrvcLogger.Info("{preparing}", $"кол-во файлов: {_files.Count()}");
                string ff = String.Empty;
                foreach (var f in _files)
                {
                    ff += f.ToString() + "; ";
                }

                SrvcLogger.Info("{preparing}", $"файлы: {ff}");

                try
                {
                    if (_files.Any(a => a.Name.Contains(".xml")))
                    {
                        if (_files.Any(a => a.Name.Contains(".zip")))
                        {
                            SetSteps(3);
                        }
                        else
                        {
                            SetSteps(1);
                        }
                    }
                    else if (_files.Any(a => a.Name.Contains(".zip")))
                    {
                        SetSteps(2);
                    }

                    UpdateCurrentStep();
                    using (var db = new dbModel(CONNECTION))
                    {
                        foreach (var file in _files)
                        {
                            if (file != null)
                            {
                                SrvcLogger.Info("{preparing}", $"импорт данных из: '{file.Name}'");
                                Log.Insert(0, $"Чтение данных: {file.Name}");

                                using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open))
                                {
                                    SrvcLogger.Info("{preparing}", $"данные прочитаны из файла: {file.Name}");
                                    Log.Insert(0, "Данные прочитаны");

                                    var helper = new InsertHelper
                                    {
                                        FileStream = fileStream,
                                        Db = db,
                                        Entity = Entity.Catalogs
                                    };

                                    if (file.Name.StartsWith("cat"))
                                    {
                                        InsertWithLogging(helper);
                                    }
                                    else if (file.Name.StartsWith("prod"))
                                    {
                                        foreach (Entity entity in Enum.GetValues(typeof(Entity)))
                                        {
                                            if (!entity.Equals(Entity.Catalogs))
                                            {
                                                helper.Entity = entity;
                                                InsertWithLogging(helper);
                                            }
                                        }
                                        Step++;

                                        SrvcLogger.Info("{work}", "перенос данных из буферных таблиц");
                                        Log.Insert(0, "Перенос данных из буферных таблиц");
                                        Finalizer(db);
                                        Step++;
                                    }
                                    else if (file.Name.Contains(".zip"))
                                    {
                                        ImageService imageService = new ImageService(receiverParams);
                                        imageService.Execute(file);
                                    }
                                }
                            }
                        }

                        stopwatch.Stop();
                        EmailBody += ResultLogging(stopwatch);
                        SendEmail(EmailBody, db);
                        CreateXmlExport(receiverParams.DirName);
                        DropImportFiles(_files);
                    }
                }
                catch (Exception e)
                {
                    SrvcLogger.Error("{error}", e.ToString());
                }
            }
            else
            {
                SrvcLogger.Info("{work}", "файлов для импорта не найдено");
            }
        }

        /// <summary>
        /// Обнуляем значения свойств перед новым импортом
        /// </summary>
        private static void Preparing()
        {
            try
            {
                using (var db = new dbModel(CONNECTION))
                {
                    CleaningTempTables(db);
                }

                distinctProducts = null;
                emailHelper = new EmailParamsHelper();
                EmailBody = String.Empty;
                CountSuccess = CountFalse = 0;
                Total = "0 час. 0 мин. 0 сек. 0 мс.";
                Log = new List<string>();
                Steps = new List<string>();
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
                SrvcLogger.Info("{work}", $"{dictionary[title]} начало");
                EmailBody += $"<p><b>{dictionary[title]}</b> начало</p>";

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

                SrvcLogger.Info("{work}", $"{dictionary[title]} конец");
                EmailBody += $"<p><b>{dictionary[title]}</b> конец</p>";
                UpdateCurrentStep();
            }
            catch (Exception e)
            {
                string errorMessage = e.ToString();
                EmailBody += $"<p>{errorMessage}</p>";
                SrvcLogger.Error("{error}", $"ошибка при импорте {dictionary[title]}");
                SrvcLogger.Error("{error}", errorMessage);
                CountFalse++;
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
            SrvcLogger.Info("{work}", $"{dictionary[entity.Title]} кол-во: {entity.List.Count()}");
            Log.Insert(0, $"Кол-во {dictionary[entity.Title]}: {entity.List.Count()}");
            EmailBody += $"<p><b>{dictionary[entity.Title]}</b> кол-во: {entity.List.Count()}</p>";

            try
            {
                using (var tr = entity.Db.BeginTransaction())
                {
                    entity.Db.BulkCopy(entity.List);
                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                string errorMessage = e.ToString();
                EmailBody += $"<p>{errorMessage}</p>";
                SrvcLogger.Error("{error}", errorMessage);
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
        private static Product[] AddProducts(InsertHelper insert)
        {
            Product[] result = null;
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
                UpdateCurrentStep();
            }
            catch (Exception e)
            {
                string errorMessage = e.ToString();
                EmailBody += $"<p>{errorMessage}</p>";
                SrvcLogger.Error("{error}", errorMessage);
                CountFalse++;
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
                SrvcLogger.Info("{work}", "рассылка оповещения");
                Log.Insert(0, "Рассылка оповещений");

                var receiverEmails = GetAdminEmails(db);
                receiverEmails.AddRange(emailHelper.EmailTo);

                foreach (var emailTo in receiverEmails)
                {
                    var from = new MailAddress(emailHelper.EmailFromAddress, emailHelper.EmailFromName);
                    var to = new MailAddress(emailTo);

                    var message = new MailMessage(from, to)
                    {
                        Subject = "Сервис импорта сайта Малышок-ПрессМарк",
                        Body = body,
                        IsBodyHtml = true
                    };

                    var smtp = new SmtpClient(emailHelper.EmailHost, emailHelper.EmailPort)
                    {
                        Credentials = new NetworkCredential(emailHelper.EmailFromAddress, emailHelper.EmailPassword),
                        EnableSsl = emailHelper.EmailEnableSsl,
                    };
                    smtp.Send(message);
                }
                Log.Insert(0, "Рассылка оповещений завершена");
                SrvcLogger.Info("{work}", "рассылка оповещения проведена");
                CountSuccess++;
                Step++;
                Percent = 100;
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", "рассылка оповещений завершилась ошибкой");
                SrvcLogger.Error("{error}", e.ToString());
                CountFalse++;
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
                    .Select(s => s.c_tech_email).FirstOrDefault();

                return email.Split(';')
                    .Select(s => s.Trim())
                    .Where(w => !String.IsNullOrWhiteSpace(w))
                    .ToList();
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Возвращает последовательность импорта
        /// в зависимости от кол-ва
        /// загруженных файлов
        /// </summary>
        /// <param name="caseType"></param>
        /// <returns></returns>
        private static void SetSteps(int caseType)
        {
            Steps.Add("Загрузка файлов");
            switch (caseType)
            {
                case 1:
                    Steps.Add("Запись в буферные таблицы");
                    Steps.Add("Совмещение данных");
                    allStepsCount = 8;
                    break;
                case 2:
                    Steps.Add("Распаковка архива изображений");
                    allStepsCount = 3;
                    break;
                case 3:
                    Steps.Add("Запись в буферные таблицы");
                    Steps.Add("Совмещение данных");
                    Steps.Add("Распаковка архива изображений");
                    allStepsCount = 9;
                    break;
            }
            Steps.Add("Рассылка оповещений");
        }

        /// <summary>
        /// Логирование по завершению импорта
        /// </summary>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        private static string ResultLogging(Stopwatch stopwatch)
        {
            TimeSpan time = stopwatch.Elapsed;
            Total = $"{time.Hours} час. {time.Minutes} мин."
                + $" {time.Seconds} сек. {time.Milliseconds} мс.";

            string falses = $"кол-во ошибок: {CountFalse}";
            string successes = $"кол-во успешных процессов: {CountSuccess}";
            SrvcLogger.Info("{work}", $"импорт завершён");
            SrvcLogger.Info("{work}", $"{falses}");
            SrvcLogger.Info("{work}", $"{successes}");
            Log.Insert(0, "Импорт завершён");

            return $"импорт завершён {falses}; {successes}";
        }

        /// <summary>
        /// Распаковывает архив с файлами импорта
        /// </summary>
        /// <returns></returns>
        private static FileInfo[] ExtractArchive(FileInfo archive)
        {
            try
            {
                SrvcLogger.Info("{work}", $"распаковка архива: {archive.Name}");
                SrvcLogger.Info("{work}", $"директория: {archive.DirectoryName}");
                ZipFile.ExtractToDirectory(archive.FullName, archive.DirectoryName);

                DirectoryInfo di = new DirectoryInfo(archive.DirectoryName);
                if (di != null)
                {
                    FileInfo[] temp = di.GetFiles()
                        .Where(w => !w.Name.ToLower().Contains(".zip"))
                        .ToArray();

                    var cat = temp.Where(w => w.Name.ToLower().Contains(".xml"))
                                              .Where(w => w.Name.ToLower().Contains("cat"))
                                              .SingleOrDefault();
                    SrvcLogger.Info("{work}", $"категории: {cat.Name}");

                    var prod = temp.Where(w => w.Name.ToLower().Contains(".xml"))
                                              .Where(w => w.Name.ToLower().Contains("prod"))
                                              .SingleOrDefault();
                    SrvcLogger.Info("{work}", $"продукция: {prod.Name}");

                    FileInfo[] result = { cat, prod };
                    return result;
                }
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", e.ToString());
            }
            return null;
        }

        /// <summary>
        /// Формирует файлы импорта в нужном порядке
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private static FileInfo[] FilesOrdering(FileInfo[] files)
        {
            List<FileInfo> result = new List<FileInfo>();

            if (files.Any(a => a.Name.ToLower().Contains(".zip")))
            {
                if (files.Any(a => a.Name.ToLower().Contains("prod")))
                {
                    var prodArchive = files.Where(a => a.Name.ToLower().Contains("prod")
                                                    && a.Name.ToLower().Contains(".zip"))
                                           .SingleOrDefault();
                    SrvcLogger.Info("{work}", $"архив с файлами xml: {prodArchive.Name}");
                    result.AddRange(ExtractArchive(prodArchive));
                }
                var imgArchive = files.Where(w => w.Name.ToLower().Contains("im")
                                                && w.Name.ToLower().Contains(".zip"))
                                      .SingleOrDefault();
                if (imgArchive != null)
                {
                    SrvcLogger.Info("{work}", $"{imgArchive.Name} добавлен");
                    result.Add(imgArchive);
                }
                var certificatesArchice = files.Where(w => w.Name.ToLower().Contains("certi")
                                                        && w.Name.ToLower().Contains(".zip"))
                                               .SingleOrDefault();
                if (certificatesArchice != null)
                {
                    SrvcLogger.Info("{work}", $"{certificatesArchice.Name} добавлен");
                    result.Add(certificatesArchice);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Обновляет информацию по текущему шагу
        /// </summary>
        public static void UpdateCurrentStep()
        {
            int delta = 100 / allStepsCount;
            CountSuccess++;
            Percent += delta;
        }

        /// <summary>
        /// Создаёт xml-файл для обратной совместимости
        /// </summary>
        /// <param name="path"></param>
        private static void CreateXmlExport(string path)
        {
            try
            {
                SrvcLogger.Info("{work}", "создание xml-файла для обратной совместимости");
                var productList = GetProducts();
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                DataContractSerializer serializer = new DataContractSerializer(typeof(List<ProductExport>));
                using (FileStream writer = new FileStream($"{path}export.xml", FileMode.Create))
                {
                    serializer.WriteObject(writer, productList);
                }
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{work}", e.ToString());
            }
        }

        /// <summary>
        /// Возвращает список продуктов, записанных в бд
        /// </summary>
        /// <returns></returns>
        private static ProductExport[] GetProducts()
        {
            using (var db = new dbModel(CONNECTION))
            {
                return db.content_productss
                    .Where(w => w.n_count > 0)
                    .Select(s => new ProductExport
                    {
                        Code = s.c_code
                    }).ToArray();
            }
        }

        /// <summary>
        /// Удаление файлов импорта
        /// </summary>
        /// <param name="files"></param>
        private static void DropImportFiles(FileInfo[] files)
        {
            try
            {
                SrvcLogger.Info("{work}", "удаление файлов импорта");
                foreach (var file in files)
                {
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{work}", e.ToString());
            }
        }

        /// <summary>
        /// Продукция для обратной совместимости с 1с
        /// </summary>
        [DataContract]
        private class ProductExport
        {
            /// <summary>
            /// Идентификатор
            /// </summary>
            [DataMember]
            public string Code { get; set; }
        }
    }
}
