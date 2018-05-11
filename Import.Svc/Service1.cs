using Import.Core;
using Import.Core.Helpers;
using Import.Core.Services;
using System;
using System.Collections.Generic;
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
        private int MilisecondsToWait(TimeSpan runTime)
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
        private int[] MilisecondsToWait(string[] runTimes)
        {
            List<int> startTimeList = new List<int>();
            foreach (string runTime in runTimes)
            {
                if (TimeSpan.TryParse(runTime, out TimeSpan _runTime))
                {
                    startTimeList.Add(MilisecondsToWait(_runTime));
                }
            }
            if (startTimeList != null && startTimeList.Count() > 0)
            {
                return startTimeList.ToArray();
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
            string times = String.Join(";", helperParams.StartTime);
            SrvcLogger.Info("{preparing}", $"время запуска интеграции {times}");
            SrvcLogger.Info("{preparing}", $"директория с файлами {helperParams.DirName}");

            while (enableIntegration)
            {
                int[] executeWaitArray = MilisecondsToWait(helperParams.StartTime);
                int executeWait = executeWaitArray.Min();
                int hoursWait = executeWait / 1000 / 60 / 60;
                int minutesWait = (executeWait - (hoursWait * 60 * 60 * 1000)) / 1000 / 60;
                int secWait = (executeWait - (hoursWait * 60 * 60 * 1000) - (minutesWait * 60 * 1000)) / 1000;
                SrvcLogger.Info("{preparing}", $"импорт будет выполнен через: " +
                                $"{hoursWait} час. {minutesWait} мин. {secWait} сек.");
                Thread.Sleep(executeWait);

                DirectoryInfo info = new DirectoryInfo(helperParams.DirName);
                FileInfo[] files = info.GetFiles("*.zip")
                                       .OrderByDescending(p => p.LastWriteTime)
                                       .Take(3)
                                       .ToArray();

                //FileInfo[] filesToDrop = info.GetFiles("*.xml");
                //DropFiles(filesToDrop);

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

        /// <summary>
        /// Удаляет файлы с предыдущего импорта
        /// </summary>
        /// <param name="files"></param>
        private void DropFiles(FileInfo[] files)
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
