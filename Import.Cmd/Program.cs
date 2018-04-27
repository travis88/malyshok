using Import.Core;
using Import.Core.Helpers;
using Import.Core.Services;
using System;
using System.IO;
using System.Linq;

namespace Import.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            ReceiverParamsHelper helperParams = new ReceiverParamsHelper();

            DirectoryInfo di = new DirectoryInfo(helperParams.DirName);
            FileInfo[] files = di.GetFiles("*.zip")
                                       .OrderByDescending(p => p.LastWriteTime)
                                       .Take(2)
                                       .ToArray();

            FileInfo[] filesToDrop = di.GetFiles("*.xml");
            DropFiles(filesToDrop);
            SrvcLogger.Info("{preparing}", "запуск ядра импорта");
            SrvcLogger.Info("{work}", $"директория: {helperParams.DirName}");
            if (files != null && files.Any(a => a != null))
            {
                string listFiles = "список найденных файлов: ";
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        listFiles += $"{file.Name}; ";
                    }
                }
                SrvcLogger.Info("{work}", $"{listFiles}");
                Importer.DoImport(files);

                foreach (var file in files)
                {
                    if (file != null && file.Exists)
                    {
                        try
                        {
                            SrvcLogger.Info("{work}", $"удаление файла: {file}");
                            file.Delete();
                        }
                        catch (Exception e)
                        {
                            SrvcLogger.Error("{error}", $"{e.ToString()}");
                        }
                    }
                }
            }
            else
            {
                SrvcLogger.Info("{work}", "файлов для импорта не найдено");
            }
        }

        /// <summary>
        /// Удаляет файлы с предыдущего импорта
        /// </summary>
        /// <param name="files"></param>
        private static void DropFiles(FileInfo[] files)
        {
            try
            {
                SrvcLogger.Info("{work}", "удаление файлов с предыдущего импорта");
                foreach (var file in files)
                {
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception e)
            {
                SrvcLogger.Error("{error}", e.ToString());
            }
        }
    }
}
