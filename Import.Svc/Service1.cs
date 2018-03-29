using Import.Core;
using Import.Core.Helpers;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace Import.Svc
{
    public partial class Service1 : ServiceBase
    {
        /// <summary>
        /// Разрешённость
        /// </summary>
        private bool enableIntegration = true;

        /// <summary>
        /// Поток
        /// </summary>
        private static Thread integrationWorker = null;

        /// <summary>
        /// Параметры из конфига
        /// </summary>
        private ReceiverParamsHelper helperParams;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Service1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Запуск 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                integrationWorker = new Thread(DoIntegration);
                integrationWorker.Start();
            }
            catch (Exception e)
            {
                SrvcLogger.Fatal("{work}", String.Format("глобальная ошибка {0}", e.ToString()));
            }
        }

        /// <summary>
        /// Остановка
        /// </summary>
        protected override void OnStop()
        {
            enableIntegration = false;
            integrationWorker.Abort();
        }

        /// <summary>
        /// Время ожидание запуска интеграции
        /// </summary>
        /// <param name="runTime"></param>
        /// <returns></returns>
        public static int MilisecondsToWait(TimeSpan runTime)
        {
            int result = 10000;
            var _currentTime = DateTime.Now.TimeOfDay;

            switch (TimeSpan.Compare(_currentTime, runTime))
            {
                case 1:
                    result = (int)(86400000 - _currentTime.TotalMilliseconds + runTime.TotalMilliseconds);
                    break;
                case 0:
                    result = 0;
                    break;
                case -1:
                    result = (int)(runTime.TotalMilliseconds - _currentTime.TotalMilliseconds);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Время ожидания запуска интеграции
        /// </summary>
        /// <param name="runTime"></param>
        /// <returns></returns>
        public int MilisecondsToWait(string runTime)
        {
            TimeSpan _runTime;
            if (TimeSpan.TryParse(runTime, out _runTime))
            {
                return MilisecondsToWait(_runTime);
            }

            throw new Exception("Ошибка определения времени выполнения");
        }

        /// <summary>
        /// Запуск интеграции
        /// </summary>
        /// <param name="data"></param>
        private void DoIntegration(object data)
        {
            SrvcLogger.Debug("{preparing}", "I work!");

            helperParams = new ReceiverParamsHelper();
            SrvcLogger.Debug("{preparing}", String.Format("время запуска интеграции {0}", helperParams.StartTime));
            SrvcLogger.Debug("{preparing}", String.Format("директория с файлами {0}", helperParams.DirName));
            
            while (enableIntegration)
            {
                DirectoryInfo info = new DirectoryInfo(helperParams.DirName);
                FileInfo[] files = info.GetFiles("*.xml")
                                       .Where(w => w.FullName.ToLower().Contains("cat") || w.FullName.Contains("prod"))
                                       .OrderByDescending(p => p.LastWriteTime)
                                       .Take(2)
                                       .ToArray();
                
                var executeWait = MilisecondsToWait(helperParams.StartTime);
                SrvcLogger.Debug("{preparing}", String.Format("импорт будет выполнен через: {0} {1}", executeWait / 1000 / 60, "мин"));
                Thread.Sleep(executeWait);
                SrvcLogger.Debug("{preparing}", "запуск ядра импорта");
                Importer.DoImport(files);
                Thread.Sleep(1000 * 60 * 2);
            }
        }
    }
}
