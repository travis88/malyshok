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

            ImageService helper = new ImageService(helperParams);
            helper.Execute();
        }
    }
}
