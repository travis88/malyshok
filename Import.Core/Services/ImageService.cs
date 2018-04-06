using Import.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
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
        public void Execute(FileInfo archive)
        {
            string tempPath = $"{ParamsHelper.SaveDirName}temp\\";
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            archive.CopyTo($"{tempPath}{archive.Name}");

            var files = ExtractArchive(new FileInfo($"{tempPath}{archive.Name}"), tempPath);
            if (files != null && files.Count() > 0)
            {
                Importer.Log.Insert(0, $"Кол-во изображений в архиве: {files.Count()}");
                ResizingImages(files);
                Importer.Step++;
                Importer.UpdateCurrentStep();
            }
            else
            {
                SrvcLogger.Info("{work}", $"распаковка {archive.Name} не дала результатов");
                Importer.CountFalse++;
            }

            #region удаляет временную директорию
            if (Directory.Exists(tempPath))
            {
                try
                {
                    Directory.Delete(tempPath, true);
                    SrvcLogger.Info("{work}", $"директория {tempPath} удалена");
                }
                catch (IOException)
                {
                    Directory.Delete(tempPath, true);
                    SrvcLogger.Info("{work}", $"директория {tempPath} удалена");
                }
                catch (UnauthorizedAccessException)
                {
                    Directory.Delete(tempPath, true);
                    SrvcLogger.Info("{work}", $"директория {tempPath} удалена");
                }
                catch (Exception e)
                {
                    SrvcLogger.Error("{error}", e.ToString());
                }
            }
            #endregion
        }

        /// <summary>
        /// Разархивирование архива с изображениями
        /// </summary>
        /// <param name="archive"></param>
        private FileInfo[] ExtractArchive(FileInfo archive, string tempPath)
        {
            FileInfo[] result = null;
            try
            {
                SrvcLogger.Info("{work}", $"распаковка архива: {archive.Name}");
                ZipFile.ExtractToDirectory(archive.FullName, tempPath);

                DirectoryInfo di = new DirectoryInfo(tempPath);
                if (di != null)
                {
                    if (di.GetDirectories().Count() > 0)
                    {
                        List<FileInfo> files = new List<FileInfo>();
                        foreach (var dir in di.GetDirectories())
                        {
                            files.AddRange(dir.GetFiles());
                        }
                        result = files.ToArray();
                    }
                    else
                    {
                        result = di.GetFiles();
                    }
                }
                if (result != null)
                {
                    SrvcLogger.Info("{work}", $"архив распакован, кол-во файлов: {result.Count()}");
                }
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", e.ToString());
            }
            return result;
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
            CodecImageParams codecImageParams = new CodecImageParams
            {
                CodecInfo = GetEncoderInfo("image/jpeg"),
                EncoderParams = new EncoderParameters(1)
            };
            codecImageParams.EncoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 70L);

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
                        SaveImages(imageSizes, codecImageParams);
                    }
                }
                catch (Exception e)
                {
                    SrvcLogger.Error("{error}", e.ToString());
                }
            }
        }

        /// <summary>
        /// Сохраняет изображение
        /// </summary>
        /// <param name="imagHelper"></param>
        private void SaveImages(ImageSaverHelper[] imageHelpers, CodecImageParams codecImageParams)
        {
            foreach (var item in imageHelpers)
            {
                try
                {
                    if (File.Exists(item.SavePath))
                    {
                        File.Delete(item.SavePath);
                    }
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
                        _img.Save(item.SavePath, codecImageParams.CodecInfo, codecImageParams.EncoderParams);
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
