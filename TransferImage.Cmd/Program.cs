using Import.Core;
using Import.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TransferImage.Cmd
{
    class Program
    {
        /// <summary>
        /// Параметры для переноса
        /// </summary>
        private static TransferParams transferParams = new TransferParams();

        /// <summary>
        /// Репозиторий
        /// </summary>
        private static Repository repository = new Repository();

        /// <summary>
        /// Точка входа
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            SrvcLogger.Info("{info}", $"перенос изображений из {transferParams.From}");
            var products = repository.GetProducts(transferParams.DateCreate);
            if (products != null && products.Count() > 0)
            {
                SrvcLogger.Info("{info}", $"кол-во товаров: {products.Count()}");
                var images = GetImages(products);
                if (images != null && images.Count() > 0)
                {
                    SrvcLogger.Info("{info}", $"кол-во изображений: {images.Count()}");
                    ImageHandler handler = new ImageHandler(transferParams);
                    handler.ResizingImages(images);
                    SrvcLogger.Info("{info}", "перенос изображений завершён");
                }
                else
                {
                    SrvcLogger.Info("{info}", "подходящих изображений не найдено");
                }
            }
            else
            {
                SrvcLogger.Info("{info}", $"товаров с датой создания после {transferParams.DateCreate} не найдено");
            }
        }

        /// <summary>
        /// Возвращает список изображений из директории
        /// </summary>
        /// <returns></returns>
        private static FileInfo[] GetImages(IEnumerable<content_products> products)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(transferParams.From);
                if (di != null)
                {
                    return di.GetFiles("*_2.jpg")
                        .Where(w => products.Any(a => w.Name.Contains(a.c_barcode)))
                        .ToArray();
                }
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", e.ToString());
            }
            return null;
        }
    }
}
