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
    }
}
