using Import.Core;
using Import.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Import.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            ReceiverParamsHelper helperParams = new ReceiverParamsHelper();

            DirectoryInfo info = new DirectoryInfo(helperParams.DirName);
            FileInfo[] files = info.GetFiles("*.zip")
                                   .OrderByDescending(p => p.LastWriteTime)
                                   .Take(3)
                                   .ToArray();

            Importer.DoImport(files);

            //XmlSerializer writer = new XmlSerializer(typeof(OrdersXMLModel));
            //var path = $"{helperParams.DirName}order.xml";
            //using (FileStream file = File.Create(path))
            //{
            //    var order = OrdersCreator()[0];
            //    writer.Serialize(file, order);
            //}
        }

        private static OrdersXMLModel[] OrdersCreator()
        {
            List<OrdersXMLModel> list = new List<OrdersXMLModel>();
            for (int i = 0; i < 100; i++)
            {
                OrdersXMLModel order = new OrdersXMLModel
                {
                    Num = i,
                    Date = DateTime.Now,
                    UserName = $"username{i}",
                    Organization = $"organization{i}",
                    Email = $"email{i}",
                    Phone = $"phone{i}",
                    Address = $"address{i}",
                    Comment = $"comment{i}",
                    Details = new XMLOrderDetails[] 
                    {
                        new XMLOrderDetails
                        {
                            Code = i.ToString(),
                            Count = i
                        },
                        new XMLOrderDetails
                        {
                            Code = (i + 1).ToString(),
                            Count = i + 1
                        }
                    }
                };
                list.Add(order);
            }
            return list.ToArray();
        }
    }
}
