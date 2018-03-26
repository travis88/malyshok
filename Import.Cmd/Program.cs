using Import.Core;
using Import.Core.Helpers;
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
            SrvcLogger.Debug("{preparing}", String.Format("время запуска интеграции {0}", helperParams.StartTime));
            SrvcLogger.Debug("{preparing}", String.Format("директория с файлами {0}", helperParams.DirName));

            DirectoryInfo info = new DirectoryInfo(helperParams.DirName);
            FileInfo[] files = info.GetFiles("*.xml")
                                   .Where(w => w.FullName.Contains("cat") || w.FullName.Contains("prod"))
                                   .OrderByDescending(p => p.LastWriteTime)
                                   .Take(2)
                                   .ToArray();

            Importer.DoImport(files);
        }
    }
}
