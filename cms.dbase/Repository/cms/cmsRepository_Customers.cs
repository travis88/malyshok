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
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Возвращает список пользователей
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override UsersList getCustomers(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_userss.AsQueryable();

                if (filter.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disable == filter.Disabled.Value);
                }

                foreach (string param in filter.SearchText.Split(' '))
                {
                    if (param != String.Empty)
                    {
                        query = query.Where(w => w.c_surname.Contains(param) 
                                            || w.c_name.Contains(param) 
                                            || w.c_patronymic.Contains(param) 
                                            || w.c_email.Contains(param));
                    }
                }

                query = query.OrderBy(o => new { o.c_surname, o.c_name });

                if (query.Any())
                {
                    int itemCount = query.Count();

                    var list = query
                        .Skip(filter.Size * (filter.Page - 1))
                        .Take(filter.Size)
                        .Select(s => new UsersModel
                        {
                            Id = s.id,
                            Surname = s.c_surname,
                            Name = s.c_name,
                            Patronymic = s.c_patronymic,
                            EMail = s.c_email,
                            Disabled = s.b_disable
                        }).ToArray();

                    return new UsersList
                    {
                        Data = list,
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
        /// Возвращает клиента
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override UsersModel getCustomer(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_userss
                    .Where(w => w.id.Equals(id))
                    .Select(s => new UsersModel
                    {
                        Id = s.id,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        Surname = s.c_surname,
                        Address = s.c_address,
                        Phone = s.c_phone,
                        EMail = s.c_email,
                        Disabled = s.b_disable
                    })
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Создаёт пользователя
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool createCustomer(UsersModel item)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_userss
                    .Value(v => v.id, item.Id)
                    .Value(v => v.c_name, item.Name)
                    .Value(v => v.c_surname, item.Surname)
                    .Value(v => v.c_patronymic, item.Patronymic)
                    .Value(v => v.c_address, item.Address)
                    .Value(v => v.c_phone, item.Phone)
                    .Value(v => v.c_email, item.EMail)
                    .Value(v => v.b_disable, item.Disabled)
                    .Insert() > 0;
            }
        }

        /// <summary>
        /// Обновляет пользователя
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool updateCustomer(UsersModel item)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_userss
                    .Where(w => w.id.Equals(item.Id))
                    .Set(s => s.c_name, item.Name)
                    .Set(s => s.c_surname, item.Surname)
                    .Set(s => s.c_patronymic, item.Patronymic)
                    .Set(s => s.c_address, item.Address)
                    .Set(s => s.c_phone, item.Phone)
                    .Set(s => s.c_email, item.EMail)
                    .Set(s => s.b_disable, item.Disabled)
                    .Update() > 0;
            }
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool deleteCustomer(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_userss
                    .Where(w => w.id.Equals(id))
                    .Delete() > 0;
            }
        }

        /// <summary>
        /// Проверяет существование пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool checkCustomerExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_userss
                    .Where(w => w.id.Equals(id))
                    .Any();
            }
        }

        /// <summary>
        /// Проверяет занятость email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public override bool checkCustomerExists(string email)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_userss
                    .Where(w => w.c_email.ToLower().Equals(email.ToLower()))
                    .Any();
            }
        }
    }
}
