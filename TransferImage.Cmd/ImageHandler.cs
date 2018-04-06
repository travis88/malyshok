using Import.Core.Helpers;
using Import.Core.Services;
using System;
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
            CodecImageParams codecImageParams = new CodecImageParams
            {
                CodecInfo = GetEncoderInfo("image/jpeg"),
                EncoderParams = new EncoderParameters(1)
            };
            codecImageParams.EncoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 70L);

            foreach (var img in files)
            {
                try
                {
                    if (transferParams.AllowedPicTypes.Contains(img.Extension.ToLower()))
                    {
                        string barcode = img.Name.Substring(0, img.Name.LastIndexOf("_"));
                        string imageName = img.Name.Substring(0, img.Name.LastIndexOf("."));
                        string saveImgPath = $"{transferParams.To}{barcode}";

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
                        //SaveImages(imageSizes, codecImageParams);
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
