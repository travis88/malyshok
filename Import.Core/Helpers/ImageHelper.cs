using SevenZip;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Import.Core.Helpers
{
    /// <summary>
    /// Обработчик изображений
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// Параметры
        /// </summary>
        public ReceiverParamsHelper ParamsHelper { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ImageHelper(ReceiverParamsHelper helper)
        {
            ParamsHelper = helper;
        }

        /// <summary>
        /// Обрабатывает изображения
        /// </summary>
        public void Execute()
        {
            if (Directory.Exists(ParamsHelper.DirName))
            {
                SrvcLogger.Debug("{work}", $"Директория: {ParamsHelper.DirName}");

                DirectoryInfo di = new DirectoryInfo(ParamsHelper.DirName);
                FileInfo archive = di.GetFiles("*.rar")
                    .Where(w => w.FullName.ToLower().Contains("image"))
                    .OrderByDescending(p => p.LastWriteTime)
                    .FirstOrDefault();

                var files = ExtractArchive(archive);
                if (files != null && files.Count() > 0)
                {
                    CreatingResizingImages(files);
                }
                else
                {
                    SrvcLogger.Debug("{work}", $"Распаковка {archive.Name} не дала результатов");
                }
            }
            else
            {
                SrvcLogger.Debug("{work}", $"Директория {ParamsHelper.DirName} не найдена");
            }
        }

        /// <summary>
        /// Возвращает инфу для кодирования
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            foreach (var enc in ImageCodecInfo.GetImageEncoders())
            {
                if (enc.MimeType.ToLower() == mimeType.ToLower())
                {
                    return enc;
                }
            }
            return null;
        }

        /// <summary>
        /// Режет изображение под нужные размеры
        /// </summary>
        /// <param name="fi"></param>
        private void CreatingResizingImages(FileInfo[] fi)
        {
            foreach (var img in fi)
            {
                if (ParamsHelper.AllowedPicTypes.Contains(img.Extension.ToLower()))
                {
                    string barcode = img.Name.Substring(0, img.Name.LastIndexOf("_"));
                    ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 70L);
                    string saveImgPath = $"{ParamsHelper.SaveDirName}{barcode}";
                    
                    if (!Directory.Exists(saveImgPath))
                    {
                        Directory.CreateDirectory(saveImgPath);
                    }

                    // миниатюра
                    Bitmap mini = (Bitmap)Bitmap.FromFile(img.FullName);
                    mini = Imaging.Resize(mini, 200, 200, "center", "center");
                    mini.Save($"{saveImgPath}\\{barcode}_mini.jpg", myImageCodecInfo, myEncoderParameters);

                    // предпросмотр
                    Bitmap preview = (Bitmap)Bitmap.FromFile(img.FullName);
                    preview = Imaging.Resize(preview, 400, 400, "center", "center");
                    preview.Save($"{saveImgPath}\\{barcode}_preview.jpg", myImageCodecInfo, myEncoderParameters);

                    // галерея
                    Bitmap galery = (Bitmap)Bitmap.FromFile(img.FullName);
                    galery = Imaging.Resize(galery, 1150, "width");
                    galery.Save($"{saveImgPath}\\{barcode}_galery.jpg", myImageCodecInfo, myEncoderParameters);
                }
            }
        }

        /// <summary>
        /// Разархивирование архива с изображениями
        /// </summary>
        private FileInfo[] ExtractArchive(FileInfo archive)
        {
            SevenZipExtractor.SetLibraryPath(ParamsHelper.SevenZipPath);
            using (SevenZipExtractor zip = new SevenZipExtractor(archive.FullName))
            {
                try
                {
                    SrvcLogger.Debug("{work}", $"Распаковка архива: {archive.Name}");
                    string tempPath = $"{ParamsHelper.SaveDirName}temp\\";
                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                    zip.ExtractArchive(tempPath);

                    DirectoryInfo di = new DirectoryInfo(tempPath);
                    return di.GetDirectories().First().GetFiles();
                }
                catch (Exception e)
                {
                    SrvcLogger.Error("{error}", e.ToString());
                    return null;
                }
            }
        }
    }
}
