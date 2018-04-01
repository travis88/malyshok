using System.Drawing.Imaging;

namespace Import.Core.Helpers
{
    /// <summary>
    /// Параметры для кодировки изображения
    /// </summary>
    public class CodecImageParams
    {
        /// <summary>
        /// Информация кодирования изображения
        /// </summary>
        public ImageCodecInfo CodecInfo { get; set; }

        /// <summary>
        /// Параметры для декодирования
        /// </summary>
        public EncoderParameters EncoderParams { get; set; }
    }
}
