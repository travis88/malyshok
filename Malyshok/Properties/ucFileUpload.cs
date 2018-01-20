using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// Имя, обьединяющее пользовательские элементы управления, созданные при помощи библиотеки
namespace UserControls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ucFileUpload runat=server></{0}:ucFileUpload>")]
    public class ucFileUpload : CompositeControl
    {
        #region Создаем переменные для настройки элемента управления
        protected string Css = String.Empty;

        protected string label = "Вложения";
        protected string help = "";
        protected string type = "all";
        protected string directory = "/UserFiles/Temp/";
        protected string filename = "";
        protected string nameprfix = "";
        protected string extension = "";
        
        protected int img_width = 0;
        protected int img_height = 0;

        public enum FildTypes { image, audio, video, document, all }    // Список типов текстового поля
        protected string PicTypes = "jpg,jpeg,png,gif,svg";
        protected string AudioTypes = "mp3,wma,wav";
        protected string VideoTypes = "flv,mp4,wmv,avi,mov,mpg";
        protected string DocTypes = "txt,doc,docx,rtf,xls,xlsx,xlsm,ppt,pps,htm,html,mht,pdf,rar,zip,mdb,swf,psd,css";

        protected bool importent = false;
        protected bool read_only = false;
        protected bool error = false;
        #endregion

        #region Создаем объекты для построения контрола
        private PlaceHolder _MainContener;
        private Panel _ControlBlock;
        private Panel _LabelBlock;
        private Panel _InputBlock;
        private Panel _PreviewBlock;
        private FileUpload _FileUpload;
        private HiddenField _FullName;
        private PlaceHolder _HelpIcon;
        private PlaceHolder _HelpBlock;
        #endregion
        
        private static readonly object EventSubmitKey = new object();
        
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Надпись над полем")]
        public string Label
        {
            set { label = value; }
            get { return label; }
        }
        
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Описание. Текст подсказки")]
        public override string ToolTip
        {
            set { help = value; }
            get { return help; }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Css стиль")]
        public override string CssClass
        {
            set { Css = value; }
            get { return Css; }
        }
                
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Определяет будет ли поле обязательным для заполнения")]
        public bool Importent
        {
            set { importent = value; }
            get { return importent; }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Определяет будет ли поле только для чтения")]
        public bool ReadOnly
        {
            set { read_only = value; }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Определяет расширения файлов, для диалога выбора файла. Выбирается из списка FildTypes")]
        public FildTypes Type
        {
            set { type = value.ToString().ToLower(); }
        }
        
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Определяет путь для для сохранения файлов")]
        public string Dir
        {
            get { return directory; }
            set { if (value != null) directory = value; }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Определяет имя сохраняемого файла")]
        public string FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Определяет префикс для сохраняемого файла")]
        public string FileNamePrefix
        {
            set { nameprfix = value; }
        }
        
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Определяет расширение в котором будет сохранен файл")]
        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Получает и отдает полный путь к файлу")]
        public string FullName
        {
            get { EnsureChildControls(); return _FullName.Value; }
            set { EnsureChildControls(); _FullName.Value = value; }
        }


        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Значение широины изображения")]
        public int ImgWidth
        {
            get { return img_width; }
            set { img_width = value; }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Значение высоты изображения")]
        public int ImgHeight
        {
            get { return img_height; }
            set { img_height = value; }
        }
        
        protected override void RecreateChildControls()
        {
            EnsureChildControls();
        }
        protected override void CreateChildControls()
        {
            Controls.Clear();

            ClientIDMode = ClientIDMode.Static;

            _MainContener = new PlaceHolder();

            // Блок для Названия
            _HelpIcon = new PlaceHolder();
            _MainContener.Controls.Add(_HelpIcon);

            // Блок для тестового поля
            _ControlBlock = new Panel();
            _ControlBlock.CssClass = "uc_fileupload";

            _LabelBlock = new Panel();
            _LabelBlock.CssClass = "uc_title";
            _ControlBlock.Controls.Add(_LabelBlock);

            _InputBlock = new Panel();
            _InputBlock.CssClass = "uc_control";
            
            _FileUpload = new FileUpload();
            _FileUpload.ID = ID + "_input";
            _FileUpload.CssClass = "";
            _InputBlock.Controls.Add(_FileUpload);

            _FullName = new HiddenField();
            _InputBlock.Controls.Add(_FullName);

            _PreviewBlock = new Panel();
            _PreviewBlock.CssClass = "uc_preview";
            _InputBlock.Controls.Add(_PreviewBlock);

            _ControlBlock.Controls.Add(_InputBlock);

            _MainContener.Controls.Add(_ControlBlock);

            // Блок для всплывающей подсказки
            _HelpBlock = new PlaceHolder();
            _MainContener.Controls.Add(_HelpBlock);

            this.Controls.Add(_MainContener);
        }

        /// <summary>
        /// Создает событие PreRender. (Унаследовано от Control.)
        /// </summary>
        /// <param name="e">Объект EventArgs, содержащий данные события.</param>
        protected override void OnPreRender(EventArgs e)
        {
            #region Ограничиваем список расширений
            string AllExtension = String.Empty;
            if (type == "image") { AllExtension = "." + PicTypes.Replace(",", ",."); }
            else if (type == "audio") { AllExtension = "." + AudioTypes.Replace(",", ",."); }
            else if (type == "video") { AllExtension = "." + VideoTypes.Replace(",", ",."); }
            else if (type == "document") { AllExtension = "." + DocTypes.Replace(",", ",."); }
            else { AllExtension = "." + PicTypes.Replace(",", ",.") + ",." + AudioTypes.Replace(",", ",.") + ",." + VideoTypes.Replace(",", ",.") + ",." + DocTypes.Replace(",", ",."); }

            _FileUpload.Attributes.Add("accept", AllExtension);
            #endregion
            #region Надпись
            if (label != String.Empty) _LabelBlock.Controls.Add(new LiteralControl(label));
            #endregion
            #region Подсказка
            if (help != String.Empty)
            {
                #region Иконка (Кнопка для вызова подсказки)
                Label HelpIcon = new Label();
                HelpIcon.CssClass = "icon-help-circled";
                _HelpIcon.Controls.Add(HelpIcon);
                #endregion

                Panel _Help = new Panel();
                _Help.CssClass = "uc_help";
                _Help.Controls.Add(new LiteralControl(help));
                _HelpBlock.Controls.Add(_Help);
            }
            #endregion
            #region Содержимое
            FilePreview();
            if (_PreviewBlock.Controls.Count > 0)
            {
                _FileUpload.CssClass = "UnLook";
                _ControlBlock.CssClass += " Open";
            }
            #endregion

            if (read_only) Css += " readonly";
            if (importent) Css += " importent";
            if (error) Css += " error";

            base.OnPreRender(e);
        }        
        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "uc_block " + Css);
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            _MainContener.RenderControl(writer);
            writer.Close();
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Определяем формат кодирования изображения")]
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            foreach (var enc in ImageCodecInfo.GetImageEncoders())
                if (enc.MimeType.ToLower() == mimeType.ToLower())
                    return enc;
            return null;
        }

        public void Save()
        {
            if (_FileUpload.HasFile)
            {
                string SaveDir = HttpContext.Current.Request.MapPath(directory);

                // Проверка, существования папки (если нет, то создаем её)
                if (!Directory.Exists(SaveDir)) { Directory.CreateDirectory(SaveDir); }

                if (extension == String.Empty) extension = new FileInfo(_FileUpload.PostedFile.FileName).Extension.ToLower().Replace(".", "");
                if (filename == String.Empty) filename = Translit(_FileUpload.PostedFile.FileName.ToLower().Replace(new FileInfo(_FileUpload.PostedFile.FileName).Extension.ToLower(), ""));

                string FullFileName = (nameprfix == String.Empty) ? filename + "." + extension : filename + "_" + nameprfix + "." + extension;

                if (type == "image" && (img_width != 0 || img_height != 0))
                {
                    #region Изменение размеров картинки
                    Stream _ImageStream = _FileUpload.PostedFile.InputStream;
                    Bitmap _Image = (Bitmap)Bitmap.FromStream(_ImageStream);
                    if (extension == "png") _Image.MakeTransparent();
                    ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/" + extension.Replace("jpg", "jpeg"));
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);

                    double ImgWidth = _Image.Width;
                    double ImgHeight = _Image.Height;

                    #region Определяем финальные размеры изображения и создаем контейнер для изображения
                    int FinW = img_width;
                    int FinH = img_height;

                    if (FinW == 0 && FinH == 0)
                    {
                        FinW = Convert.ToInt32(ImgWidth);
                        FinH = Convert.ToInt32(ImgHeight);
                    }
                    else if (FinW == 0 || FinH == 0)
                    {
                        if (FinW == 0)
                        {
                            if (ImgHeight > FinH) FinW = Convert.ToInt32(FinH / (ImgHeight / ImgWidth));
                            else FinW = Convert.ToInt32(ImgWidth);
                        }
                        else
                        {
                            if (ImgWidth > FinW) FinH = Convert.ToInt32(FinW / (ImgWidth / ImgHeight));
                            else FinH = Convert.ToInt32(ImgHeight);
                        }

                    }
                    Bitmap FinalImage = new Bitmap(FinW, FinH);
                    FinalImage.MakeTransparent();
                    Graphics gr = Graphics.FromImage(FinalImage);
                    if (extension == "png") gr.Clear(Color.Transparent);
                    else gr.Clear(Color.White);
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    #endregion
                    #region Создаем пропорционально уменьшенную копию орегинала до финальных размеров
                    Rectangle TempSize;
        
                    double TempWidth = 0;
                    double TempHeight = 0;
                    int xPos = 0;
                    int yPos = 0;
                    double D = 1;

                    if (ImgWidth > FinW && ImgHeight > FinH)
                    {
                        D = ImgWidth / ImgHeight;
                        TempWidth = FinW;
                        TempHeight = TempWidth / D;

                        if (TempHeight < FinH)
                        {
                            D = ImgHeight / ImgWidth;
                            TempHeight = FinH;
                            TempWidth = TempHeight / D;
                        }
                    }
                    else
                    {
                        TempWidth = ImgWidth;
                        TempHeight = ImgHeight;
                    }

                    if (Convert.ToInt32(TempWidth) > FinW) xPos = Convert.ToInt32(ImgWidth - (FinW * (ImgHeight / Convert.ToDouble(FinH)))) / 2;

                    TempSize = new Rectangle(0, 0, Convert.ToInt32(TempWidth), Convert.ToInt32(TempHeight));

                    gr.DrawImage(_Image, TempSize, xPos, yPos, Convert.ToInt32(ImgWidth), Convert.ToInt32(ImgHeight), GraphicsUnit.Pixel);
                    #endregion

                    FinalImage.Save(SaveDir + FullFileName, myImageCodecInfo, myEncoderParameters);
                    FinalImage.Dispose();
                    _Image.Dispose();
                    #endregion
                }
                else _FileUpload.PostedFile.SaveAs(SaveDir + "//" + FullFileName);

                _FullName.Value = directory + filename + "." + extension;
            }
        }

        private void FilePreview()
        {
            _PreviewBlock.Controls.Clear();

            #region Получаем данные о выбранном файле
            FileInfo _File = new FileInfo(HttpContext.Current.Request.MapPath(_FullName.Value));

            if (_File.Exists)
            {
                string Ex = _File.Extension.Replace(".", "");

                Panel _FileItem = new Panel();
                _FileItem.CssClass = "uc_preview_item";

                Panel _ImgBlock = new Panel();
                _ImgBlock.CssClass = "uc_preview_img";
                if (PicTypes.IndexOf(Ex) > -1)
                {
                    System.Web.UI.WebControls.Image _Img = new System.Web.UI.WebControls.Image();
                    _Img.ImageUrl = _FullName.Value;
                    _ImgBlock.Controls.Add(_Img);
                }
                else
                {
                    _ImgBlock.Controls.Add(new LiteralControl(Ex));
                }
                _FileItem.Controls.Add(_ImgBlock);

                Panel _InfoBlock = new Panel();
                _InfoBlock.CssClass = "uc_preview_info";
                
                #region Определение размеров файла
                int FileSize = Convert.ToInt32(_File.Length.ToString());
                string FileSizeName = "байт";

                if (FileSize < 1024) { 
                    FileSize = FileSize;
                    FileSizeName = FileSizeName; 
                }
                else if (FileSize <= 1048576) 
                {
                    FileSize=Convert.ToInt32(Convert.ToDouble(_File.Length) / 1024);FileSizeName = "кб";
                }
                else
                {
                    FileSize = Convert.ToInt32(Convert.ToDouble(_File.Length) / 1024);
                    FileSize = Convert.ToInt32(Convert.ToDouble(FileSize) / 1024);

                    FileSizeName = "мб";
                }

                _InfoBlock.Controls.Add(new LiteralControl("<div class=\"uc_preview_name\">" + _File.Name + " <span>(" + FileSize.ToString() + " " + FileSizeName + ")</span></div>"));
                #endregion

                _InfoBlock.Controls.Add(new LiteralControl("<div class=\"uc_preview_size\"><span>Дата изменения:</span> " + _File.LastWriteTime.ToString("dd.MM.yyyy")+ " / "+ _File.LastWriteTime.ToString("hh:mm:ss") + "</div>"));
                _InfoBlock.Controls.Add(new LiteralControl("<div class=\"uc_preview_btn\">"));
                _InfoBlock.Controls.Add(new LiteralControl("<a class=\"uc_preview_Del\" title=\"Удалить\" href=\"\">Удалить</a>"));
                _InfoBlock.Controls.Add(new LiteralControl("</div>"));

                _FileItem.Controls.Add(_InfoBlock);
                _PreviewBlock.Controls.Add(_FileItem);
            }
            #endregion
        }

        /// <summary>
        /// Функция транслитерации русского текста
        /// </summary>
        /// <param name="source">Исходная строка</param>
        /// <returns>транслитерированная строка</returns>
        public static string Translit(string source)
        {
            Dictionary<string, string> words = new Dictionary<string, string>();

            words.Add("а", "a");
            words.Add("б", "b");
            words.Add("в", "v");
            words.Add("г", "g");
            words.Add("д", "d");
            words.Add("е", "e");
            words.Add("ё", "yo");
            words.Add("ж", "zh");
            words.Add("з", "z");
            words.Add("и", "i");
            words.Add("й", "j");
            words.Add("к", "k");
            words.Add("л", "l");
            words.Add("м", "m");
            words.Add("н", "n");
            words.Add("о", "o");
            words.Add("п", "p");
            words.Add("р", "r");
            words.Add("с", "s");
            words.Add("т", "t");
            words.Add("у", "u");
            words.Add("ф", "f");
            words.Add("х", "h");
            words.Add("ц", "c");
            words.Add("ч", "ch");
            words.Add("ш", "sh");
            words.Add("щ", "sch");
            words.Add("ъ", "j");
            words.Add("ы", "i");
            words.Add("ь", "j");
            words.Add("э", "e");
            words.Add("ю", "yu");
            words.Add("я", "ya");
            words.Add("А", "A");
            words.Add("Б", "B");
            words.Add("В", "V");
            words.Add("Г", "G");
            words.Add("Д", "D");
            words.Add("Е", "E");
            words.Add("Ё", "Yo");
            words.Add("Ж", "Zh");
            words.Add("З", "Z");
            words.Add("И", "I");
            words.Add("Й", "J");
            words.Add("К", "K");
            words.Add("Л", "L");
            words.Add("М", "M");
            words.Add("Н", "N");
            words.Add("О", "O");
            words.Add("П", "P");
            words.Add("Р", "R");
            words.Add("С", "S");
            words.Add("Т", "T");
            words.Add("У", "U");
            words.Add("Ф", "F");
            words.Add("Х", "H");
            words.Add("Ц", "C");
            words.Add("Ч", "Ch");
            words.Add("Ш", "Sh");
            words.Add("Щ", "Sch");
            words.Add("Ъ", "J");
            words.Add("Ы", "I");
            words.Add("Ь", "J");
            words.Add("Э", "E");
            words.Add("Ю", "Yu");
            words.Add("Я", "Ya");
            words.Add(" ", "_");
            words.Add(":", "-");
            words.Add("<", "(");
            words.Add(">", ")");
            words.Add("[", "(");
            words.Add("]", ")");
            words.Add("{", "(");
            words.Add("}", ")");
            words.Add("«", "");
            words.Add("»", "");
            words.Add("\"", "");
            words.Add("#", "");
            words.Add("%", "");
            words.Add("&", "");
            words.Add("@", "");
            words.Add("$", "");
            words.Add("'", "");
            words.Add("*", "");
            words.Add(",", "");
            words.Add(";", "");
            words.Add("=", "");
            words.Add("+", "");
            words.Add("!", "");
            words.Add("?", "");
            words.Add("^", "");
            words.Add("`", "");
            words.Add("|", "");

            Regex re = new Regex("[-]{2,}");
            Regex Re = new Regex("[_]{2,}");
            Regex StartRe = new Regex("^[-|_]{1,}");
            Regex EndRe = new Regex("[-|_]${1,}");

            foreach (KeyValuePair<string, string> pair in words)
            {
                source = source.Replace(pair.Key, pair.Value);
            }
            source = re.Replace(source, "-");
            source = Re.Replace(source, "_");
            source = StartRe.Replace(source, "");
            source = EndRe.Replace(source, "");
            return source;
        }
    }
}