using Import.Core;
using Import.Core.Helpers;
using Import.Core.Services;
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
                SrvcLogger.Error("{work}", $"глобальная ошибка {e.ToString()}");
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
            string errorMessage = "ошибка определения времени выполнения";
            SrvcLogger.Error("{error}", errorMessage);
            throw new Exception(errorMessage);
        }

        /// <summary>
        /// Запуск интеграции
        /// </summary>
        /// <param name="data"></param>
        private void DoIntegration(object data)
        {
            SrvcLogger.Info("{preparing}", "I work!");
            ReceiverParamsHelper helperParams = new ReceiverParamsHelper();
            SrvcLogger.Info("{preparing}", $"время запуска интеграции {helperParams.StartTime}");
            SrvcLogger.Info("{preparing}", $"директория с файлами {helperParams.DirName}");
            
            while (enableIntegration)
            {
                DirectoryInfo info = new DirectoryInfo(helperParams.DirName);
                
                FileInfo[] files = { info.GetFiles("*.xml")
                                        .Where(w => w.FullName.ToLower()
                                        .Contains("cat"))
                                        .OrderByDescending(p => p.LastWriteTime)
                                        .FirstOrDefault(),

                                     info.GetFiles("*.xml")
                                        .Where(w => w.FullName.ToLower()
                                        .Contains("prod"))
                                        .OrderByDescending(p => p.LastWriteTime)
                                        .FirstOrDefault(),

                                     info.GetFiles("*.zip")
                                        .OrderByDescending(p => p.LastWriteTime)
                                        .FirstOrDefault() };
                
                int executeWait = MilisecondsToWait(helperParams.StartTime);
                SrvcLogger.Info("{preparing}", $"импорт будет выполнен через: {executeWait / 1000 / 60} мин");
                Thread.Sleep(executeWait);
                SrvcLogger.Info("{preparing}", "запуск ядра импорта");
                SrvcLogger.Info("{work}", $"директория: {helperParams.DirName}");
                Importer.DoImport(files);
                Thread.Sleep(1000 * 60 * 2);
            }
        }
    }
}
