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

            ImageHelper helper = new ImageHelper(helperParams);
            helper.Execute();
        }
    }
}
