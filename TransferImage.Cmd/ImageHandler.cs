using Import.Core.Helpers;
using Import.Core.Services;
using System;
using System.IO;
using System.Linq;

namespace TransferImage.Cmd
{
    /// <summary>
    /// Обработчик изображений
    /// </summary>
    public class ImageHandler
    {
        /// <summary>
        /// Параметры из конфига
        /// </summary>
        private TransferParams transferParams;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_params"></param>
        public ImageHandler(TransferParams _params)
        {
            transferParams = _params;
        }
        
        /// <summary>
        /// Обрабатывает изображения
        /// </summary>
        public void ResizingImages(FileInfo[] files)
        {
            int countFiles = files.Count();
            ImageCreator imageCreator = new ImageCreator();
            int i = 0; // итерация

            foreach (var img in files)
            {
                try
                {
                    if (transferParams.AllowedPicTypes.Contains(img.Extension.ToLower()))
                    {
                        string barcode = $"{img.Name.Substring(0, img.Name.LastIndexOf("_"))}";
                        string saveImgPath = $"{transferParams.To}{barcode}";

                        if (!Directory.Exists(saveImgPath))
                        {
                            Directory.CreateDirectory(saveImgPath);
                        }

                        ImageItemHelper[] imageSizes = new ImageItemHelper[]
                        {
                            new ImageItemHelper(img.FullName, $"{saveImgPath}\\{barcode}_1_mini.jpg",
                                                 200, 200, "center", "center", null),
                            new ImageItemHelper(img.FullName, $"{saveImgPath}\\{barcode}_1_preview.jpg",
                                                 400, 400, "center", "center", null),
                            new ImageItemHelper(img.FullName, $"{saveImgPath}\\{barcode}_1.jpg",
                                                 1150, 600, null, null, "width")
                        };
                        imageCreator.SaveImages(imageSizes);
                        i++;
                        if (i % 100 == 0)
                        {
                            SrvcLogger.Info("{info}", $"обработано {i} изображений из {countFiles}");
                        }
                    }
                }
                catch (Exception e)
                {
                    SrvcLogger.Error("{error}", e.ToString());
                }
            }
        }
    }
}
