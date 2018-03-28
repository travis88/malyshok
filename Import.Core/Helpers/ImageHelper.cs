using System;
using System.Drawing.Imaging;

namespace Import.Core.Helpers
{
    /// <summary>
    /// Обработчик изображений
    /// </summary>
    public class ImageHelper
    {


        /// <summary>
        /// Обрабатывает изображения
        /// </summary>
        public void Execute()
        {

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
    }
}
