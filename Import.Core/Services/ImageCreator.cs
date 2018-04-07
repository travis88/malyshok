using Import.Core.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Import.Core.Services
{
    /// <summary>
    /// Создатель изображений
    /// </summary>
    public class ImageCreator
    {
        /// <summary>
        /// Параметры для кодирования
        /// </summary>
        private CodecImageParams CodecImageParams;

        /// <summary>
        /// Конструктор
        /// </summary>
        public ImageCreator()
        {
            CodecImageParams = GetCodeParams();
        }

        /// <summary>
        /// Возвращает параметры для кодирования
        /// </summary>
        /// <returns></returns>
        private CodecImageParams GetCodeParams()
        {
            CodecImageParams codecImageParams = new CodecImageParams
            {
                CodecInfo = GetEncoderInfo("image/jpeg"),
                EncoderParams = new EncoderParameters(1)
            };
            codecImageParams.EncoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 70L);
            return codecImageParams;
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
        /// Сохраняет изображение
        /// </summary>
        /// <param name="imagHelper"></param>
        public void SaveImages(ImageItemHelper[] imageHelpers)
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
                        Bitmap _img, bg = null;
                        if (String.IsNullOrWhiteSpace(item.Orientation))
                        {
                            _img = Imaging.Resize(img, item.Width, item.Height);
                            bg = DrawFilledRectangle(item.Width, item.Height);
                            _img = Superimpose(bg, _img);
                        }
                        else
                        {
                            if (img.Width > img.Height)
                            {
                                _img = Imaging.Resize(img, item.Width, item.Orientation);
                                if (_img.Height > item.Height)
                                {
                                    _img = Imaging.Resize(img, item.Height, "height");
                                }
                            }
                            else
                            {
                                _img = Imaging.Resize(img, item.Height, "height");
                                if (_img.Width > item.Width)
                                {
                                    _img = Imaging.Resize(img, item.Width, item.Orientation);
                                }
                            }
                        }
                        _img.Save(item.SavePath, CodecImageParams.CodecInfo, CodecImageParams.EncoderParams);
                        _img.Dispose();
                        bg.Dispose();
                    }
                }
                catch (Exception e)
                {
                    SrvcLogger.Error("{error}", e.ToString());
                }
            }
        }

        /// <summary>
        /// Создаёт белый квадрат
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Bitmap DrawFilledRectangle(int x, int y)
        {
            Bitmap bmp = new Bitmap(x, y);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, x, y);
                graphics.FillRectangle(Brushes.White, ImageSize);
            }
            return bmp;
        }

        /// <summary>
        /// Вписывает одно изображение в другое
        /// </summary>
        /// <param name="largeBmp"></param>
        /// <param name="smallBmp"></param>
        /// <returns></returns>
        private Bitmap Superimpose(Bitmap largeBmp, Bitmap smallBmp)
        {
            using (Graphics graphics = Graphics.FromImage(largeBmp))
            {
                int x = (largeBmp.Width - smallBmp.Width) / 2;
                int y = (largeBmp.Height - smallBmp.Height) / 2;
                graphics.DrawImage(smallBmp, new Point(x, y));
                return largeBmp;
            }
        }
    }
}
