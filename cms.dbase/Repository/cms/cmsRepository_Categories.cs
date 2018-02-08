using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;
using System.Collections.Generic;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с категориями товаров
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        /// <summary>
        /// Возвращает список категорий
        /// </summary>
        /// <returns></returns>
        public override CategoryModel[] getCategories(Guid? parent = null)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_categoriess
                    .Where(w => w.uui_parent.Equals(parent))
                    .OrderBy(o => o.c_title)
                    .Select(s => new CategoryModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Parent = s.uui_parent,
                        CountChildren = db.content_categoriess
                                            .Where(w => w.uui_parent.Equals(s.id))
                                            .Count()
                    }).ToArray();
            }
        }

        /// <summary>
        /// Возвращает запись категории продукции
        /// </summary>
        /// <param name="id">идентификатор</param>
        /// <returns></returns>
        public override CategoryModel getCategory(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_categoriess
                    .Where(w => w.id.Equals(id))
                    .Select(s => new CategoryModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Alias = s.c_alias,
                        Parent = s.uui_parent,
                        CountChildren = db.content_categoriess
                                            .Where(w => w.uui_parent.Equals(s.id))
                                            .Count()
                    }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Возвращает единичную хлебную крошку
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override BreadCrumbCategory getBreadCrumbCategory(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_categoriess
                    .Where(w => w.id.Equals(id))
                    .Select(s => new BreadCrumbCategory
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Parent = s.uui_parent
                    }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Возвращает список хлебных крошек для категорий
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override BreadCrumbCategory[] getCategoryBreadCrumbs(Guid? id)
        {
            var list = new List<BreadCrumbCategory>();

            if (!id.Equals(null))
            {
                var item = getBreadCrumbCategory((Guid)id);
                while (item != null)
                {
                    list.Add(item);
                    if (!item.Parent.Equals(null))
                    {
                        item = getBreadCrumbCategory((Guid)item.Parent);
                    }
                    else { item = null; }
                }
            }

            list.Reverse();

            return list.ToArray();
        }

        /// <summary>
        /// Существует ли категория
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool categoryExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_categoriess
                    .Where(w => w.id.Equals(id))
                    .Any();
            }
        }

        /// <summary>
        /// Создаёт новую категорию
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool createCategory(CategoryModel item)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_categoriess
                    .Value(v => v.id, item.Id)
                    .Value(v => v.c_title, item.Title)
                    .Value(v => v.c_alias, item.Alias)
                    .Value(v => v.uui_parent, item.Parent)
                    .Insert() > 0;
            }
        }

        /// <summary>
        /// Обновляет категорию
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool updateCategory(CategoryModel item)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_categoriess
                    .Where(w => w.id.Equals(item.Id))
                    .Set(u => u.c_title, item.Title)
                    .Set(u => u.c_alias, item.Alias)
                    .Update() > 0;
            }
        }

        /// <summary>
        /// Удаляем категорию
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool deleteCategory(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.content_categoriess
                    .Where(w => w.id.Equals(id))
                    .Delete() > 0;
            }
        }
    }
}
