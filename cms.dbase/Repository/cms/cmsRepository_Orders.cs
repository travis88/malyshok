using cms.dbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;

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
                var list = db.content_orderss.AsQueryable();

                if (!String.IsNullOrEmpty(filter.SearchText))
                {
                    list = list.Where(w => w.n_num.ToString().Equals(filter.SearchText));
                }
                if (filter.Date != null)
                {
                    list = list.Where(w => w.d_date >= filter.Date);
                }
                if (filter.DateEnd != null)
                {
                    list = list.Where(w => w.d_date <= filter.DateEnd);
                }
                if (!String.IsNullOrEmpty(filter.Category))
                {
                    list = list.Where(w => w.f_status.ToString().Equals(filter.Category));
                }

                int itemCount = list.Count();

                var data = list
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
                                },
                                User = new UsersModel
                                {
                                    Id = s.contentorderscontentusers.id,
                                    EMail = s.contentorderscontentusers.c_email
                                },
                                Total = s.contentorderdetailscontentorderss
                                            .Sum(d => d.m_price * d.n_count)
                            });

                if (list.Any())
                {
                    return new OrdersList
                    {
                        Orders = data.ToArray(),
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
                        UserComment = s.c_user_comment,
                        AdminComment = s.c_admin_comment,
                        Status = new OrderStatus
                        {
                            Title = s.contentorderscontentorderstatuses.c_title,
                            Id = s.contentorderscontentorderstatuses.id
                        },
                        User = new UsersModel
                        {
                            Id = s.contentorderscontentusers.id,
                            Name = s.contentorderscontentusers.c_name,
                            Patronymic = s.contentorderscontentusers.c_patronymic,
                            Surname = s.contentorderscontentusers.c_surname,
                            Phone = s.contentorderscontentusers.c_phone,
                            Address = s.contentorderscontentusers.c_address
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

        /// <summary>
        /// Возвращает список статусов
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<OrderStatus> getStatuses()
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_order_statusess
                    .OrderBy(o => o.id)
                    .Select(s => new OrderStatus
                    {
                        Id = s.id,
                        Title = s.c_title
                    }).ToArray();
            }
        }

        /// <summary>
        /// Обновляет инфу по заказу
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool updateOrder(OrderModel item)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_orderss
                    .Where(w => w.id.Equals(item.Id))
                    .Set(u => u.f_status, item.Status.Id)
                    .Set(u => u.c_admin_comment, item.AdminComment)
                    .Update() > 0;
            }
        }
    }
}
