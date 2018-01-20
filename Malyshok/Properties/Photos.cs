using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

public class Photos
{
    /// <summary>
    /// Сохранить изображение с размерами
    /// </summary>
    /// <param name="hpf">HPF сслыка на изображение</param>
    /// <param name="Path">Путь для сохранения</param>
    /// <param name="fileName">Имя для сохраняемого на сервере файла (с расширением)</param>
    /// <param name="maxWidth">Максимальная ширина</param>
    /// <param name="maxHeight">Максимальная высота</param>
    /// http://hellomvc.blogspot.ru/2011/03/uploading-with-resize.html

    public static string SaveImageResizeProp(HttpPostedFileBase hpf, string Path, string fileName, int maxWidth, int maxHeight)
    {
        string filePath = string.Empty;
        if (hpf != null)
        {
            using (System.Drawing.Bitmap originalPic = new System.Drawing.Bitmap(hpf.InputStream, false))
            {
                // Вычисление новых размеров картинки
                int width = originalPic.Width; //текущая ширина
                int height = originalPic.Height; //текущая высота
                int widthDiff = (width - maxWidth); //разница с допуст. шириной
                int heightDiff = (height - maxHeight); //разница с допуст. высотой

                // Определение размеров, которые необходимо изменять
                bool doWidthResize = (maxWidth > 0 && width > maxWidth &&
                                    widthDiff > -1 && widthDiff > heightDiff);
                bool doHeightResize = (maxHeight > 0 && height > maxHeight &&
                                    heightDiff > -1 && heightDiff > widthDiff);

                // Ресайз картинки
                if (doWidthResize || doHeightResize || (width.Equals(height)
                                && widthDiff.Equals(heightDiff)))
                {
                    int iStart;
                    Decimal divider;
                    if (doWidthResize)
                    {
                        iStart = width;
                        divider = Math.Abs((Decimal)iStart / maxWidth);
                        width = maxWidth;
                        height = (int)Math.Round((height / divider));
                    }
                    else
                    {
                        iStart = height;
                        divider = Math.Abs((Decimal)iStart / maxHeight);
                        height = maxHeight;
                        width = (int)Math.Round((width / divider));
                    }
                }

                // Сохраняем файл в папку пользователя
                using (System.Drawing.Bitmap newBMP = new System.Drawing.Bitmap(originalPic, width, height))
                {
                    using (System.Drawing.Graphics oGraphics = System.Drawing.Graphics.FromImage(newBMP))
                    {
                        oGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        oGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        oGraphics.DrawImage(originalPic, 0, 0, width, height);

                        //int idx = hpf.FileName.LastIndexOf('.');
                        //string ext = hpf.FileName.Substring(idx, hpf.FileName.Length - idx);

                        // Формируем имя для картинки
                        //Random rnd = new Random();
                        //int imageName = rnd.Next();

                        filePath = HttpContext.Current.Server.MapPath(Path + fileName);
                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);
                        newBMP.Save(filePath);

                    }
                }
            }
        }
        return filePath;
    }
}
