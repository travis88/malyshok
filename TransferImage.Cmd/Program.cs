using Import.Core.Services;
using System.IO;

namespace TransferImage.Cmd
{
    class Program
    {
        /// <summary>
        /// Параметры для переноса
        /// </summary>
        private static TransferParams transferParams = new TransferParams();

        /// <summary>
        /// Репозиторий
        /// </summary>
        private static Repository repository = new Repository();

        /// <summary>
        /// Точка входа
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            SrvcLogger.Error("{info}", $"перенос изображений из {transferParams.From}");
            var products = repository.GetProducts();
            SrvcLogger.Error("{info}", "перенос изображений завершён");
        }

        /// <summary>
        /// Возвращает список изображений из директории
        /// </summary>
        /// <returns></returns>
        private static FileInfo[] GetImages()
        {
            DirectoryInfo di = new DirectoryInfo(transferParams.From);
            if (di != null)
            {
                var files = di.GetFiles();
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}
