using Import.Core.Helpers;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace TransferImage.Cmd
{
    /// <summary>
    /// Обработчик изображений
    /// </summary>
    public class ImageHandler
    {
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
        private void ResizingImages(FileInfo[] files)
        {
            CodecImageParams codecImageParams = new CodecImageParams
            {
                CodecInfo = GetEncoderInfo("image/jpeg"),
                EncoderParams = new EncoderParameters(1)
            };
            codecImageParams.EncoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 70L);
        }
    }
}
