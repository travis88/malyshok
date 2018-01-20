using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;
using System.Web;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с новостями
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        #region private methods of class

        // Получение групп к которым относится новость
        private MaterialsGroup[] db_getMaterialGroups(CMSdb db, Guid materialId)
        {
            var query = db.content_materials_groups_links.AsQueryable();

            if (materialId != Guid.Empty)
            {
                query = query.Where(w => w.f_material == materialId);
            }

            var data = query
                 .Select(s => new MaterialsGroup
                 {
                     Id = s.f_group,
                     Title = s.fkcontentmaterialsgroups.c_title
                 });

            if (!data.Any())
                return null;

            return data.ToArray();
        }

        //Данная функция должна вызываться в рамках транзакции
        private void db_updateMaterialGroups(CMSdb db, Guid materialId, Guid[] groups)
        {
            //Удаляем привязанные группы
            db.content_materials_groups_links
                    .Where(w => w.f_material == materialId)
                    .Delete();

            // привязываем новые группы
            if (groups != null)
            {
                foreach (var group in groups)
                {
                    if (group != Guid.Empty)
                    {
                        var materialGroup = new content_materials_groups_link
                        {
                            id = Guid.NewGuid(),
                            f_material = materialId,
                            f_group = group
                        };
                        db.Insert(materialGroup);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Получение групп к которым относится новость
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public override MaterialsGroup[] getMaterialGroups(Guid materialId)
        {
            using (var db = new CMSdb(_context))
            {
                return db_getMaterialGroups(db, materialId);
            }
        }

        public override MaterialsGroup[] getAllMaterialGroups()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_materials_groupss
                 .Select(s => new MaterialsGroup
                 {
                     Id = s.id,
                     Title = s.c_title
                 });

                if (!data.Any())
                    return null;

                return data.ToArray();
            }
        }

        /// <summary>
        /// Получим список новостей
        /// </summary>
        /// <param name="filtr">Фильтр</param>
        /// <returns></returns>
        public override MaterialsList getMaterialsList(MaterialFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                // кол-во эл-тов
                int itemCount = db.content_materialss.Count();

                // список новостей
                var list = db.content_materialss
                    .OrderByDescending(o => o.d_date)
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new MaterialsModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Date = s.d_date,
                        PreviewImage = new Photo
                        {
                            Url = s.c_preview
                        },
                        Disabled = s.b_disabled,
                        Text = s.c_text
                    });

                if (list.Any())
                {
                    return new MaterialsList
                    {
                        Data = list.ToArray(),
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
                else { return null; }
            }
        }

        /// <summary>
        /// Получим единичную запись новости
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public override MaterialsModel getMaterial(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                //Проверка на домен???

                var data = db.content_materialss
                    .Where(w => w.id == id)
                    .Select(s => new MaterialsModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Alias = s.c_alias,
                        PreviewImage = new Photo()
                        {
                            Url = s.c_preview
                        },
                        Text = s.c_text,
                        Url = s.c_url,
                        UrlName = s.c_url_name,
                        Date = s.d_date,
                        Keyw = s.c_keyw,
                        Desc = s.c_desc,
                        Disabled = s.b_disabled,
                        Important = s.b_important,
                        GroupsId = s.fkcontentmaterialsgroupslinkmaterials
                                    .Select(g =>
                                    g.f_group).ToArray(),
                        Groups = s.fkcontentmaterialsgroupslinkmaterials
                                    .Select(g => new MaterialsGroup()
                                    {
                                        Id = g.f_group,
                                        Title = g.fkcontentmaterialsgroups.c_title
                                    }).ToArray()
                    });

                if (data.Any())
                    return data.First();
                else
                    return null;
            }
        }

        /// <summary>
        /// Добавляем запись
        /// </summary>
        /// <param name="material">Новость</param>
        /// <returns></returns>
        public override bool insertCmsMaterial(MaterialsModel material)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_materials cdMaterial = db.content_materialss
                                                .Where(p => p.id == material.Id)
                                                .SingleOrDefault();
                        if (cdMaterial != null)
                        {
                            throw new Exception("Запись с таким Id уже существует");
                        }

                        cdMaterial = new content_materials
                        {
                            id = material.Id,
                            c_title = material.Title,
                            c_alias = material.Alias,
                            c_text = material.Text,
                            d_date = material.Date,
                            c_preview = (material.PreviewImage != null) ? material.PreviewImage.Url : null,
                            c_url = material.Url,
                            c_url_name = material.UrlName,
                            c_desc = material.Desc,
                            c_keyw = material.Keyw,
                            b_important = material.Important,
                            b_disabled = material.Disabled,
                            n_day = material.Date.Day,
                            n_month = material.Date.Month,
                            n_year = material.Date.Year
                        };

                        db.Insert(cdMaterial);

                        db_updateMaterialGroups(db, material.Id, material.GroupsId);

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Materials,
                            Action = LogAction.insert,
                            PageId = material.Id,
                            PageName = material.Title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Обновляем запись
        /// </summary>
        /// <param name="material">Новость</param>
        /// <returns></returns>
        public override bool updateCmsMaterial(MaterialsModel material)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_materials cdMaterial = db.content_materialss
                                                .Where(p => p.id == material.Id)
                                                .SingleOrDefault();
                        if (cdMaterial == null)
                            throw new Exception("Запись с таким Id не найдена");

                        cdMaterial.c_title = material.Title;
                        cdMaterial.c_alias = material.Alias;
                        cdMaterial.c_text = material.Text;
                        cdMaterial.d_date = material.Date;

                        if (material.PreviewImage != null)
                        {
                            cdMaterial.c_preview = material.PreviewImage.Url;
                        }
                        else
                        {
                            cdMaterial.c_preview = null;
                        }

                        cdMaterial.c_url = material.Url;
                        cdMaterial.c_url_name = material.UrlName;
                        cdMaterial.c_desc = material.Desc;
                        cdMaterial.c_keyw = material.Keyw;
                        cdMaterial.b_important = material.Important;
                        cdMaterial.b_disabled = material.Disabled;
                        cdMaterial.n_day = material.Date.Day;
                        cdMaterial.n_month = material.Date.Month;
                        cdMaterial.n_year = material.Date.Year;

                        db.Update(cdMaterial);
                        db_updateMaterialGroups(db, material.Id, material.GroupsId);

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Materials,
                            Action = LogAction.update,
                            PageId = material.Id,
                            PageName = material.Title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Удаляем новость
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns></returns>
        public override bool deleteCmsMaterial(Guid id)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        var data = db.content_materialss
                                               .Where(p => p.id == id);
                        if (!data.Any())
                        {
                            throw new Exception("Запись с таким Id не найдена");
                        }

                        var cdMaterial = data.SingleOrDefault();

                        //Delete news_group_links
                        var q1 = db.content_materials_groups_links
                             .Where(s => s.f_material == id)
                             .Delete();
                        
                        db.Delete(cdMaterial);

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Materials,
                            Action = LogAction.delete,
                            PageId = cdMaterial.id,
                            PageName = cdMaterial.c_title,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //write to log ex
                var message = String.Format("cmsRepository: deleteCmsMaterial; id={0}", id);
                OnDislyEvent(new DislyEventArgs(LogLevelEnum.Error, message, ex));

                return false;
            }
        }

        /// <summary>
        /// Получаем список групп для новостей
        /// </summary>
        /// <returns></returns>
        public override MaterialsGroup[] getMaterialsGroups()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_materials_groupss
                    .OrderBy(o => o.n_sort)
                    .Select(s => new MaterialsGroup
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Sort = s.n_sort
                    });

                if (!data.Any()) return null;
                else return data.ToArray();
            }
        }
    }
}
