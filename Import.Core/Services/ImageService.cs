using Import.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

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
        /// Префикс для названия директории для сохранения
        /// </summary>
        private string prefixFolder = @"ProdContent\";

        /// <summary>
        /// Архив для сохранения изображений продукции
        /// </summary>
        private bool isImages = true;

        /// <summary>
        /// Обрабатывает изображения
        /// </summary>
        public void Execute(FileInfo archive)
        {
            if (archive.Name.ToLower().Contains("certi"))
            {
                prefixFolder = @"Certificates\";
                isImages = false;
            }
            string tempPath = $"{ParamsHelper.SaveDirName}{prefixFolder}temp\\";
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            archive.CopyTo($"{tempPath}{archive.Name}");

            var files = ExtractArchive(new FileInfo($"{tempPath}{archive.Name}"), tempPath);
            if (files != null && files.Count() > 0)
            {
                Importer.Log.Insert(0, $"Кол-во изображений в архиве: {files.Count()}");
                Importer.EmailBody += $"<p>кол-во изображений в архиве <b>{archive.Name}</b>: {files.Count()}</p>";
                ResizingImages(files);
                Importer.Step++;
                Importer.UpdateCurrentStep();
            }
            else
            {
                SrvcLogger.Info("{work}", $"распаковка {archive.Name} не дала результатов");
                Importer.EmailBody += $"<p>распаковка <b>{archive.Name}</b> не дала результатов</p>";
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
                Importer.EmailBody += $"<p>распаковка архива: <b>{archive.Name}</b></p>";
                ZipFile.ExtractToDirectory(archive.FullName, tempPath, Encoding.UTF8);

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
        /// Режет изображение под нужные размеры
        /// </summary>
        /// <param name="fi"></param>
        private void ResizingImages(FileInfo[] fi)
        {
            ImageCreator imageCreator = new ImageCreator();

            foreach (var img in fi)
            {
                try
                {
                    if (ParamsHelper.AllowedPicTypes.Contains(img.Extension.ToLower()))
                    {
                        imageCreator.SaveImages(GetImageHelpers(img));
                    }
                }
                catch (Exception e)
                {
                    SrvcLogger.Error("{error}", e.ToString());
                }
            }
        }

        /// <summary>
        /// Возвращает эл-ты необходимые для сохранения
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private ImageItemHelper[] GetImageHelpers(FileInfo img)
        {
            if (isImages)
            {
                string barcode = img.Name.Substring(0, img.Name.LastIndexOf("_"));
                string imageName = img.Name.Substring(0, img.Name.LastIndexOf("."));
                string saveImgPath = $"{ParamsHelper.SaveDirName}{prefixFolder}{barcode}";
                if (!Directory.Exists(saveImgPath))
                {
                    Directory.CreateDirectory(saveImgPath);
                }

                return new ImageItemHelper[]
                {
                    new ImageItemHelper(img.FullName, $"{saveImgPath}\\{imageName}_mini.jpg",
                                        200, 200, "center", "center", null),
                    new ImageItemHelper(img.FullName, $"{saveImgPath}\\{imageName}_preview.jpg",
                                        400, 400, "center", "center", null),
                    new ImageItemHelper(img.FullName, $"{saveImgPath}\\{imageName}.jpg",
                                        1150, 600, null, null, "width")
                };
            }
            else
            {
                string saveImgPath = $"{ParamsHelper.SaveDirName}{prefixFolder}";
                if (!Directory.Exists(saveImgPath))
                {
                    Directory.CreateDirectory(saveImgPath);
                }

                return new ImageItemHelper[]
                {
                    new ImageItemHelper(img.FullName, $"{saveImgPath}\\{img.Name}",
                                        1240, 1754, null, null, "height")
                };
            }
        }
    }
}
