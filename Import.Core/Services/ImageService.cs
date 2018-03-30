using Import.Core.Helpers;
using SevenZip;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Import.Core.Services
{
    /// <summary>
    /// Обработчик изображений
    /// </summary>
    public class ImageService
    {
        /// <summary>
        /// Параметры
        /// </summary>
        private ReceiverParamsHelper ParamsHelper { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ImageService(ReceiverParamsHelper helper)
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
                SrvcLogger.Debug("{work}", $"директория: {ParamsHelper.DirName}");

                DirectoryInfo di = new DirectoryInfo(ParamsHelper.DirName);
                FileInfo archive = di.GetFiles("*.rar")
                    .Where(w => w.FullName.ToLower().Contains("image"))
                    .OrderByDescending(p => p.LastWriteTime)
                    .FirstOrDefault();

                var files = ExtractArchive(archive);
                if (files != null && files.Count() > 0)
                {
                    ResizingImages(files);
                }
                else
                {
                    SrvcLogger.Debug("{work}", $"распаковка {archive.Name} не дала результатов");
                }

                #region удаляет временную директорию
                string tempPath = $"{ParamsHelper.SaveDirName}temp\\";
                if (Directory.Exists(tempPath))
                {
                    try
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch (IOException)
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Directory.Delete(tempPath, true);
                    }
                    catch (Exception e)
                    {
                        SrvcLogger.Error("{error}", e.ToString());
                    }
                }
                #endregion
            }
            else
            {
                SrvcLogger.Debug("{work}", $"директория {ParamsHelper.DirName} не найдена");
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
        private void ResizingImages(FileInfo[] fi)
        {
            foreach (var img in fi)
            {
                try
                {
                    if (ParamsHelper.AllowedPicTypes.Contains(img.Extension.ToLower()))
                    {
                        string barcode = img.Name.Substring(0, img.Name.LastIndexOf("_"));
                        string imageName = img.Name.Substring(0, img.Name.LastIndexOf("."));
                        string saveImgPath = $"{ParamsHelper.SaveDirName}{barcode}";

                        if (!Directory.Exists(saveImgPath))
                        {
                            Directory.CreateDirectory(saveImgPath);
                        }

                        ImageSaverHelper[] imageSizes = new ImageSaverHelper[]
                        {
                            new ImageSaverHelper(img.FullName, $"{saveImgPath}\\{imageName}_mini.jpg",
                                                 200, 200, "center", "center", null),
                            new ImageSaverHelper(img.FullName, $"{saveImgPath}\\{imageName}_preview.jpg",
                                                 400, 400, "center", "center", null),
                            new ImageSaverHelper(img.FullName, $"{saveImgPath}\\{imageName}.jpg",
                                                 1150, 0, null, null, "width")
                        };
                        SaveImages(imageSizes);
                    }
                }
                catch (Exception e)
                {
                    SrvcLogger.Error("{error}", e.ToString());
                }
            }
        }

        /// <summary>
        /// Разархивирование архива с изображениями
        /// </summary>
        /// <param name="archive"></param>
        private FileInfo[] ExtractArchive(FileInfo archive)
        {
            FileInfo[] result = null;
            SevenZipExtractor.SetLibraryPath(ParamsHelper.SevenZipPath);
            using (SevenZipExtractor zip = new SevenZipExtractor(archive.FullName))
            {
                try
                {
                    SrvcLogger.Debug("{work}", $"распаковка архива: {archive.Name}");
                    string tempPath = $"{ParamsHelper.SaveDirName}temp\\";
                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                    zip.ExtractArchive(tempPath);

                    DirectoryInfo di = new DirectoryInfo(tempPath);
                    result = di.GetDirectories().First().GetFiles();
                }
                catch (Exception e)
                {
                    SrvcLogger.Error("{error}", e.ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// Сохраняет изображение
        /// </summary>
        /// <param name="imagHelper"></param>
        private void SaveImages(ImageSaverHelper[] imageHelpers)
        {
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 70L);

            foreach (var item in imageHelpers)
            {
                try
                {
                    using (Bitmap img = (Bitmap)Bitmap.FromFile(item.FullName))
                    {
                        Bitmap _img = null;
                        if (String.IsNullOrWhiteSpace(item.Orientation))
                        {
                            _img = Imaging.Resize(img, item.Width, item.Height, item.PositionTop, item.PositionLeft);
                        }
                        else
                        {
                            _img = Imaging.Resize(img, item.Width, item.Orientation);
                        }
                        _img.Save(item.SavePath, myImageCodecInfo, myEncoderParameters);
                        _img.Dispose();
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
