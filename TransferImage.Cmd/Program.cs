using Import.Core.Services;
using System;
using System.IO;
using System.Linq;

namespace TransferImage.Cmd
{
    class Program
    {
        /// <summary>
        /// Точка входа
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            TransferParams transferParams = new TransferParams();
            Repository repository = new Repository();

            SrvcLogger.Info("{info}", $"перенос изображений из {transferParams.From}");
            SrvcLogger.Info("{info}", $"дата создания не ранее {transferParams.DateCreate}");

            var products = repository.GetProducts(transferParams.DateCreate);
            if (products != null && products.Count() > 0)
            {
                SrvcLogger.Info("{info}", $"кол-во товаров: {products.Count()}");
                var images = GetImages(products, transferParams);
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
        private static FileInfo[] GetImages(string[] products, TransferParams transferParams)
        {
            try
            {
                DirectoryInfo oldDirectory = new DirectoryInfo(transferParams.From);
                DirectoryInfo newDirectory = new DirectoryInfo(transferParams.To);

                if (oldDirectory != null && newDirectory != null)
                {
                    var existingDirs = newDirectory.GetDirectories();

                    return oldDirectory.GetFiles("*_2.jpg")
                        .Where(w => !existingDirs.Any(a => w.Name.Contains(a.Name)))
                        .Where(w => products.Any(a => w.Name.Contains(a)))
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
