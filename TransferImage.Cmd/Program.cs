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

            var barcodes = repository.GetProducts(transferParams.DateCreate)
                .Where(w => !String.IsNullOrWhiteSpace(w)).ToArray();
            if (barcodes != null && barcodes.Count() > 0)
            {
                SrvcLogger.Info("{info}", $"кол-во товаров: {barcodes.Count()}");
                var images = GetImages(barcodes, transferParams);
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
        private static FileInfo[] GetImages(string[] barcodes, TransferParams transferParams)
        {
            try
            {
                DirectoryInfo oldDirectory = new DirectoryInfo(transferParams.From);
                DirectoryInfo newDirectory = new DirectoryInfo(transferParams.To);

                if (oldDirectory != null && newDirectory != null)
                {
                    var existingDirs = newDirectory
                        .GetDirectories().Select(s => s.Name);

                    var result = oldDirectory.GetFiles("*_2.jpg")
                        .Where(w => !existingDirs.Any(a => w.Name.StartsWith(a)))
                        .Where(w => barcodes.Any(a => w.Name.StartsWith(a)))
                        .ToArray();

                    return result;
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
