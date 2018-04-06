using Import.Core.Helpers;
using Import.Core.Services;
using System;
using System.Drawing;
using System.Drawing.Imaging;
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
        /// Обрабатывает изображения
        /// </summary>
        public void ResizingImages(FileInfo[] files)
        {
            int countFiles = files.Count();

            CodecImageParams codecImageParams = new CodecImageParams
            {
                CodecInfo = GetEncoderInfo("image/jpeg"),
                EncoderParams = new EncoderParameters(1)
            };
            codecImageParams.EncoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 70L);

            int i = 0; // итерация
            foreach (var img in files)
            {
                try
                {
                    if (transferParams.AllowedPicTypes.Contains(img.Extension.ToLower()))
                    {
                        string imageName = img.Name.Substring(0, img.Name.LastIndexOf("_"));
                        string saveImgPath = $"{transferParams.To}{imageName}";

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
                            new ImageSaverHelper(img.FullName, $"{saveImgPath}\\{imageName}_1.jpg",
                                                 1150, 0, null, null, "width")

                            //миниатюра      200х200
                            //предпроссмотр  400х400
                            //галерея        1150х600   (пропорционально по максимальной стороне)
                        };

                        SaveImages(imageSizes, codecImageParams);
                        i++;
                        if (i % 50 == 0)
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
