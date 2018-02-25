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
        /// Основной метод
        /// </summary>
        public static void DoImport(Stream xml)
        {
            //SrvcLogger.Debug("{PREPARING}", "Начало чтения XML-данных");

            XmlSerializer serializer = new XmlSerializer(typeof(ArrayOfProducts));

            //SrvcLogger.Debug("{PREPARING}", "Данные успешно прочитаны из файла");

            var arrayOfProducts = (ArrayOfProducts)serializer.Deserialize(xml);


            if (arrayOfProducts != null && arrayOfProducts.Products != null)
            {
                int count = arrayOfProducts.Products.Count();
                //SrvcLogger.Debug("{PREPARING}", String.Format("Кол-во записей {0}", count));
                CountProducts = count;

                for (int i = 0; i <= 100; i++)
                {
                    Thread.Sleep(300);
                    Percent = i;

                    if (i == 100)
                    {
                        i = 0;
                        Step++;
                        if (Step == 3)
                            break;
                    }
                }
            }
        }
    }
}
