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
            FileInfo archive = di.GetFiles("*.zip")
                .Where(w => w.FullName.ToLower().Contains("image"))
                .OrderByDescending(p => p.LastWriteTime)
                .FirstOrDefault();

            ImageService helper = new ImageService(helperParams);
            if (archive != null)
            {
                helper.Execute(archive);
            }
            else
            {
                SrvcLogger.Info("{work}", $"директория: {helperParams.DirName} не найдена");
            }
        }
    }
}
