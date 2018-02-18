using cms.dbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cms.dbModel.entity;
using cms.dbase.models;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с заказами
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Возвращает список заказов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override OrdersList getOrders(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                int itemCount = db.content_orderss.Count();

                var list = db.content_orderss
                    .OrderByDescending(o => o.n_num)
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new OrderModel
                    {
                        Id = s.id,
                        Num = s.n_num,
                        Date = s.d_date,
                        Status = new OrderStatus
                        {
                            Title = s.contentorderscontentorderstatuses.c_title
                        }
                    });

                if (list.Any())
                {
                    return new OrdersList
                    {
                        Orders = list.ToArray(),
                        Pager = new Pager
                        {
                            page = filter.Page,
                            size = filter.Size,
                            items_count = itemCount,
                            page_count = (itemCount % filter.Size > 0)
                                            ? (itemCount / filter.Size) + 1
                                            : itemCount / filter.Size
                        }
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// Возвращает заказ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override OrderModel getOrder(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_orderss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new OrderModel
                    {
                        Id = s.id,
                        Num = s.n_num,
                        Date = s.d_date,
                        Status = new OrderStatus
                        {
                            Title = s.contentorderscontentorderstatuses.c_title
                        },
                        User = new UsersModel
                        {
                            Id = s.contentorderscontentusers.id,
                            Name = s.contentorderscontentusers.c_name,
                            Patronymic = s.contentorderscontentusers.c_patronymic,
                            Surname = s.contentorderscontentusers.c_surname
                        },
                        Details = s.contentorderdetailscontentorderss
                                    .Select(d => new OrderDetails
                                    {
                                        Product = new ProductModel
                                        {
                                            Id = d.contentorderdetailscontentproducts.id,
                                            Title = d.contentorderdetailscontentproducts.c_title
                                        },
                                        Date = d.d_date,
                                        Price = d.m_price,
                                        Count = d.n_count
                                    }).ToArray()
                    }).SingleOrDefault();
            }
        }
    }
}
