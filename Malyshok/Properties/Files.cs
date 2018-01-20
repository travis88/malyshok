using cms.dbModel.entity;
using Disly.Areas.Admin.Service;
using Portal.Code;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

public class Files
{
    /// <summary>
    /// Сохранить изображение с размерами
    /// </summary>
    /// <param name="hpf">HPF сслыка на изображение</param>
    /// <param name="Path">Путь для сохранения</param>
    /// <param name="maxWidth">Максимальная ширина</param>
    /// <param name="maxHeight">Максимальная высота</param>
    /// http://hellomvc.blogspot.ru/2011/03/uploading-with-resize.html
    /// 
    public static string SaveImageResize(HttpPostedFileBase hpf, string Path ,int FinWidth, int FinHeight)
    {
        ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
        EncoderParameters myEncoderParameters = new EncoderParameters(1);
        myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

        Bitmap _File  = (Bitmap)Bitmap.FromStream(hpf.InputStream);
        _File = Imaging.Resize(_File, FinWidth, FinHeight, "top","left");
        //_File = Imaging.Crop(_File, FinWidth, FinHeight, 0, 0);

        //    Path = Server.MapPath(Path);
        if (!Directory.Exists(Path)) { Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Path)); }
        
        string imageName = Transliteration.Translit(hpf.FileName.Substring(0, hpf.FileName.IndexOf("."))).Replace(" ", "_");
        string extension =hpf.FileName.Substring(hpf.FileName.IndexOf("."));
        string filePath = HttpContext.Current.Server.MapPath(Path + imageName + extension);

        if (File.Exists(filePath))
            File.Delete(filePath);
        _File.Save(filePath, myImageCodecInfo, myEncoderParameters);
        _File.Dispose();

        return Path + imageName + extension;
    }

    public static string SaveImageResizeRename(HttpPostedFileBase hpf, string Path, string Name, int FinWidth, int FinHeight)
    {
        ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
        EncoderParameters myEncoderParameters = new EncoderParameters(1);
        myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

        Bitmap _File = (Bitmap)Bitmap.FromStream(hpf.InputStream);

        string orientation = string.Empty;


        if (FinWidth != 0 && FinHeight != 0)
        {
            _File = Imaging.Resize(_File, FinWidth, FinHeight, "top", "center");
        }
        else if (FinWidth != 0 && FinHeight == 0)
        {
            orientation = "width";
            _File = Imaging.Resize(_File, FinWidth, orientation);
        }
        else if (FinWidth == 0 && FinHeight != 0)
        {
            orientation = "height";
            _File = Imaging.Resize(_File, FinHeight, orientation);
        }
        
        if (!Directory.Exists(Path)) { Directory.CreateDirectory(HttpContext.Current.Server.MapPath(Path)); }
        
        string extension = hpf.FileName.Substring(hpf.FileName.IndexOf("."));
        string filePath = HttpContext.Current.Server.MapPath(Path + Name + extension);

        if (File.Exists(filePath))
            File.Delete(filePath);
        _File.Save(filePath, myImageCodecInfo, myEncoderParameters);
        _File.Dispose();

        return Path + Name + extension;
    }

    public static string SaveImageResizeProp(HttpPostedFileBase hpf, string Path, int maxWidth, int maxHeight)
    {
        string filePath = string.Empty;
        if (hpf != null && hpf.ContentLength != 0 && hpf.ContentLength <= 307200)
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

                        int idx = hpf.FileName.LastIndexOf('.');
                        string ext = hpf.FileName.Substring(idx, hpf.FileName.Length - idx);

                        // Формируем имя для картинки
                        Random rnd = new Random();
                        int imageName = rnd.Next();

                        filePath = HttpContext.Current.Server.MapPath(Path + imageName + ext);
                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);
                        newBMP.Save(filePath);

                    }
                }
                    }
                }
        return filePath;
            }


    public class FileAnliz
    {
        /// <summary>
        /// Определяет размер файла
        /// </summary>
        /// <param name="url">Полный путь к файлу</param>
        /// <returns></returns>
        public static string Size(string url)
        {
            string Result = String.Empty;

            FileInfo _File = new FileInfo(HttpContext.Current.Request.MapPath(url));

            if (!File.Exists(HttpContext.Current.Request.MapPath(url)))
                return Result = "0 byte";

            int FileSize = Convert.ToInt32(_File.Length.ToString());
            string FileSizeName = "byte";

            if (FileSize < 1024)
            {
                //FileSize = FileSize;
                //FileSizeName = FileSizeName;
            }
            else if (FileSize <= 1048576)
            {
                FileSize = Convert.ToInt32(Convert.ToDouble(_File.Length) / 1024); FileSizeName = "kb";
            }
            else
            {
                FileSize = Convert.ToInt32(Convert.ToDouble(_File.Length) / 1024);
                FileSize = Convert.ToInt32(Convert.ToDouble(FileSize) / 1024);

                FileSizeName = "mb";
            }
            Result = FileSize.ToString() + " " + FileSizeName;

            return Result;
        }

        /// <summary>
        /// Определяет размер файла у переданного файла
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string SizeFromUpload(HttpPostedFileBase file)
        {
            string Resalt = String.Empty;

            int FileSize = Convert.ToInt32(file.ContentLength.ToString());
            string FileSizeName = "byte";

            if (FileSize < 1024)
            {
                //FileSize = FileSize;
                //FileSizeName = FileSizeName;
            }
            else if (FileSize <= 1048576)
            {
                FileSize = Convert.ToInt32(Convert.ToDouble(file.ContentLength) / 1024); FileSizeName = "kb";
            }
            else
            {
                FileSize = Convert.ToInt32(Convert.ToDouble(file.ContentLength) / 1024);
                FileSize = Convert.ToInt32(Convert.ToDouble(FileSize) / 1024);

                FileSizeName = "mb";
            }
            Resalt = FileSize.ToString() + " " + FileSizeName;

            return Resalt;
        }

        /// <summary>
        /// Определяеь расширение файла
        /// </summary>
        /// <param name="url">Полный путь к файлу</param>
        /// <returns></returns>
        public static string Extension(string url)
        {
            string Resalt = String.Empty;

            FileInfo _File = new FileInfo(HttpContext.Current.Request.MapPath(url));
            Resalt = _File.Extension.ToString();

            return Resalt;
        }
    }

    private static ImageCodecInfo GetEncoderInfo(String mimeType)
    {
        foreach (var enc in ImageCodecInfo.GetImageEncoders())
            if (enc.MimeType.ToLower() == mimeType.ToLower())
                return enc;
        return null;
    }

    /// <summary>
    /// Получаем параметры изображения
    /// </summary>
    /// <param name="url">Ссылка на файл</param>
    /// <returns></returns>
    public static Photo getInfoImage(string url)
    {
        try
        {
            var serverPath = HttpContext.Current.Server.MapPath(url);
            if (System.IO.File.Exists(serverPath))
                return new Photo
                {
                    Name = Path.GetFileName(HttpContext.Current.Server.MapPath(url)),
                    Size = Files.FileAnliz.Size(url),
                    Url = url
                };
        }
        catch (Exception ex)
        {
            AppLogger.Warn("Несуществующий файл: " + url, ex);
        }

        return new Photo
        {
            Name = null,
            Size = null,
            Url = null
        };
    }

    public static void deleteImage(string url)
    {
        try
        {
            var serverPath = HttpContext.Current.Server.MapPath(url);
            if (System.IO.File.Exists(serverPath))
                System.IO.File.Delete(serverPath);
        }
        catch (Exception ex)
        {
            AppLogger.Warn("Попытка удалить несуществующий файл: " + url, ex);
        }
    }
}
