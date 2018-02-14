using System;
using System.Linq;
using cms.dbModel;
using cms.dbModel.entity;
using cms.dbase.models;
using LinqToDB;

namespace cms.dbase
{
    /// <summary>
    /// Репозиторий для работы с новостями
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {

        #region PortalUsers and SiteUsers
        public override bool check_user(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                bool result = false;

                int count = db.cms_userss.Where(w => w.id == id).Count();
                if (count > 0) result = true;

                return result;
            }
        }
        public override bool check_user(string email)
        {
            using (var db = new CMSdb(_context))
            {
                bool result = false;

                int count = db.cms_userss.Where(w => w.c_email == email).Count();
                if (count > 0) result = true;

                return result;
            }
        }
        public override void check_usergroup(Guid id, string group)
        {
            using (var db = new CMSdb(_context))
            {
                string groupName = db.cms_userss.Where(w => w.id == id).First().f_group;
                string logTitle = db.cms_userss.Where(w => w.id == id).Select(s => s.c_surname + " " + s.c_name).First().ToString();

                if (group != groupName)
                {
                    // Удаляем все права пользователя
                    db.cms_resolutionss.
                        Where(w => w.c_user_id == id).
                        Delete();
                    // Назначение прав по шаблону группы
                    ResolutionsModel[] GroupResolution = db.cms_resolutions_templatess.
                        Where(w => w.f_user_group == group).
                        Select(s => new ResolutionsModel
                        {
                            MenuId = s.f_menu_id,
                            Read = s.b_read,
                            Write = s.b_write,
                            Change = s.b_change,
                            Delete = s.b_delete
                        }).ToArray();

                    foreach (ResolutionsModel m in GroupResolution)
                    {
                        db.cms_resolutionss
                            .Value(v => v.c_user_id, id)
                            .Value(v => v.c_menu_id, m.MenuId)
                            .Value(v => v.b_read, m.Read)
                            .Value(v => v.b_write, m.Write)
                            .Value(v => v.b_change, m.Change)
                            .Value(v => v.b_delete, m.Delete)
                            .Insert();
                    }
                    // логирование
                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.Users,
                        Action = LogAction.change_resolutions,
                        PageId = id,
                        PageName = logTitle,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

                }
            }
        }

        public override UsersList getUsersList(FilterParams filtr)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.cms_userss.AsQueryable();

                if (!string.IsNullOrEmpty(filtr.Domain))
                {

                    query = query.Where(s => s.fklinkusertosites.Any(t => t.f_site == filtr.Domain))
                                 .Where(s => (s.f_group.ToLower()!= "developer" && s.f_group.ToLower() != "administrator"));
                }

                if (filtr.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disabled == filtr.Disabled.Value);
                }
                if (filtr.Group != String.Empty)
                {
                    query = query.Where(w => w.f_group == filtr.Group);
                }
                foreach (string param in filtr.SearchText.Split(' '))
                {
                    if (param != String.Empty)
                    {
                        query = query.Where(w => w.c_surname.Contains(param) || w.c_name.Contains(param) || w.c_patronymic.Contains(param) || w.c_email.Contains(param));
                    }
                }

                query = query.OrderBy(o => new { o.c_surname, o.c_name });

                if (query.Any())
                {
                    int ItemCount = query.Count();

                    var List = query
                        .Skip(filtr.Size * (filtr.Page - 1))
                        .Take(filtr.Size)
                        .Select(s => new UsersModel
                        {
                            Id = s.id,
                            Surname = s.c_surname,
                            Name = s.c_name,
                            EMail = s.c_email,
                            Group = s.f_group,
                            GroupName = s.fkusersgroup.c_title,
                            Disabled = s.b_disabled,
                            Lvl = s.fkusersgroup.n_level
                        });

                    UsersModel[] usersInfo = List.ToArray();

                    return new UsersList
                    {
                        Data = usersInfo,
                        Pager = new Pager
                        {
                            page = filtr.Page,
                            size = filtr.Size,
                            items_count = ItemCount,
                            page_count = (ItemCount % filtr.Size > 0) ? (ItemCount / filtr.Size) + 1 : ItemCount / filtr.Size
                        }
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// Пользователь по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override UsersModel getUser(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_userss.
                    Where(w => w.id == id).
                    Select(s => new UsersModel
                    {
                        Id = s.id,
                        EMail = s.c_email,
                        Group = s.f_group,
                        Post = s.c_post,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        Birthday = s.d_birthday,
                        Sex = s.b_sex,
                        Photo = s.c_photo,
                        Address = s.c_adres,
                        Phone = s.c_phone,
                        Mobile = s.c_mobile,
                        Contacts = s.c_contacts,
                        Disabled = s.b_disabled
                    });


                if (!data.Any()) { return null; }
                else { return data.First(); }
            }
        }
        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Item"></param>
        /// <param name="UserId"></param>
        /// <param name="IP"></param>
        /// <returns></returns>
        public override bool createUser(Guid id, UsersModel Item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cms_userss.Where(w => w.id == id);
                    if (!data.Any())
                    {
                        db.cms_userss
                        .Value(p => p.id, id)
                        .Value(p => p.c_surname, Item.Surname)
                        .Value(p => p.c_name, Item.Name)
                        .Value(p => p.c_patronymic, Item.Patronymic)
                        .Value(p => p.f_group, Item.Group)
                        .Value(p => p.b_sex, Item.Sex)
                        .Value(p => p.d_birthday, Item.Birthday)
                        .Value(p => p.c_post, Item.Post)
                        .Value(p => p.c_adres, Item.Address)
                        .Value(p => p.c_phone, Item.Phone)
                        .Value(p => p.c_mobile, Item.Mobile)
                        .Value(p => p.c_email, Item.EMail)
                        .Value(p => p.c_salt, Item.Salt)
                        .Value(p => p.c_hash, Item.Hash)
                        .Value(p => p.c_contacts, Item.Contacts)
                        .Value(p => p.b_disabled, Item.Disabled)
                        .Insert();

                        // логирование
                        //insertLog(UserId, IP, "insert", id, String.Empty, "Users", Item.Surname + " " + Item.Name);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Users,
                            Action = LogAction.insert,
                            PageId = id,
                            PageName = Item.Surname + " " + Item.Name,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);
                        
                        //цепление к сайтам если создается администратор портала или разработчик
                        if(Item.Group== "administrator" || Item.Group.ToLower() == "developer")
                        {
                            string[] allsitesdomain = db.cms_sitess.Select(s => s.c_alias).ToArray();
                            foreach (var singldomain in allsitesdomain)
                            {
                                db.cms_user_site_links
                                  .Value(v => v.f_user, id)
                                  .Value(v => v.f_site, singldomain)
                                  .Insert();
                            }
                        }
                        

                        // Назначение прав по шаблону группы
                        ResolutionsModel[] GroupResolution = db.cms_resolutions_templatess.
                            Where(w => w.f_user_group == Item.Group).
                            Select(s => new ResolutionsModel
                            {
                                MenuId = s.f_menu_id,
                                Read = s.b_read,
                                Write = s.b_write,
                                Change = s.b_change,
                                Delete = s.b_delete
                            }).ToArray();

                        foreach (ResolutionsModel m in GroupResolution)
                        {
                            db.cms_resolutionss
                                .Value(v => v.c_user_id, id)
                                .Value(v => v.c_menu_id, m.MenuId)
                                .Value(v => v.b_read, m.Read)
                                .Value(v => v.b_write, m.Write)
                                .Value(v => v.b_change, m.Change)
                                .Value(v => v.b_delete, m.Delete)
                                .Insert();
                        }
                        // логирование
                        //insertLog(UserId, IP, "change_resolutions", id, String.Empty, "Users", Item.Surname + " " + Item.Name);
                        log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Users,
                            Action = LogAction.change_resolutions,
                            PageId = id,
                            PageName = Item.Surname + " " + Item.Name,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// создатет пользователя и цепляет его к сайту
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Item"></param>
        /// <returns></returns>
        public override bool createUserOnSite(Guid id, UsersModel Item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cms_userss.Where(w => w.id == id);
                    if (!data.Any())
                    {
                        db.cms_userss
                        .Value(p => p.id, id)
                        .Value(p => p.c_surname, Item.Surname)
                        .Value(p => p.c_name, Item.Name)
                        .Value(p => p.c_patronymic, Item.Patronymic)
                        .Value(p => p.f_group, Item.Group)
                        .Value(p => p.b_sex, Item.Sex)
                        .Value(p => p.d_birthday, Item.Birthday)
                        .Value(p => p.c_post, Item.Post)
                        .Value(p => p.c_adres, Item.Address)
                        .Value(p => p.c_phone, Item.Phone)
                        .Value(p => p.c_mobile, Item.Mobile)
                        .Value(p => p.c_email, Item.EMail)
                        .Value(p => p.c_salt, Item.Salt)
                        .Value(p => p.c_hash, Item.Hash)
                        .Value(p => p.c_contacts, Item.Contacts)
                        .Value(p => p.b_disabled, Item.Disabled)
                        .Insert();


                        //прицепление пользователя к текущему сайту
                        db.cms_user_site_links
                          .Value(v => v.f_user, id)
                          .Value(v => v.f_site, _domain)
                          .Insert();




                        // логирование
                        //insertLog(UserId, IP, "insert", id, String.Empty, "Users", Item.Surname + " " + Item.Name);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Users,
                            Action = LogAction.insert,
                            PageId = id,
                            PageName = Item.Surname + " " + Item.Name,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        // Назначение прав по шаблону группы
                        ResolutionsModel[] GroupResolution = db.cms_resolutions_templatess.
                            Where(w => w.f_user_group == Item.Group).
                            Select(s => new ResolutionsModel
                            {
                                MenuId = s.f_menu_id,
                                Read = s.b_read,
                                Write = s.b_write,
                                Change = s.b_change,
                                Delete = s.b_delete
                            }).ToArray();

                        foreach (ResolutionsModel m in GroupResolution)
                        {
                            db.cms_resolutionss
                                .Value(v => v.c_user_id, id)
                                .Value(v => v.c_menu_id, m.MenuId)
                                .Value(v => v.b_read, m.Read)
                                .Value(v => v.b_write, m.Write)
                                .Value(v => v.b_change, m.Change)
                                .Value(v => v.b_delete, m.Delete)
                                .Insert();
                        }
                        // логирование
                        //insertLog(UserId, IP, "change_resolutions", id, String.Empty, "Users", Item.Surname + " " + Item.Name);
                        log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Users,
                            Action = LogAction.change_resolutions,
                            PageId = id,
                            PageName = Item.Surname + " " + Item.Name,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Item"></param>
        /// <param name="UserId"></param>
        /// <param name="IP"></param>
        /// <returns></returns>
        public override bool updateUser(Guid id, UsersModel Item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    check_usergroup(id, Item.Group);

                    var data = db.cms_userss.Where(w => w.id == id);

                    if (data != null)
                    {
                        data.Where(w => w.id == id)
                        .Set(p => p.c_surname, Item.Surname)
                        .Set(p => p.c_name, Item.Name)
                        .Set(p => p.c_patronymic, Item.Patronymic)
                        .Set(p => p.f_group, Item.Group)
                        .Set(p => p.b_sex, Item.Sex)
                        .Set(p => p.d_birthday, Item.Birthday)
                        .Set(p => p.c_post, Item.Post)
                        .Set(p => p.c_adres, Item.Address)
                        .Set(p => p.c_phone, Item.Phone)
                        .Set(p => p.c_mobile, Item.Mobile)
                        .Set(p => p.c_email, Item.EMail)
                        .Set(p => p.c_contacts, Item.Contacts)
                        .Set(p => p.b_disabled, Item.Disabled)
                        .Update();

                        // логирование
                        //insertLog(UserId, IP, "update", id, String.Empty, "Users", Item.Surname + " " + Item.Name);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Users,
                            Action = LogAction.update,
                            PageId = id,
                            PageName = string.Format("{0} {1}", Item.Surname, Item.Name),
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }
        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <param name="UserId"></param>
        /// <param name="IP"></param>
        /// <returns></returns>
        public override bool deleteUser(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var user = db.cms_userss.Where(w => w.id == id);
                    if (user.Any())
                    {
                        string userName = user.Select(s => string.Format("{0} {1}", s.c_surname, s.c_name))
                            .SingleOrDefault();

                        user.Delete();

                        // логирование
                        //insertLog(UserId, IP, "delete", id, String.Empty, "Users", logTitle);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Users,
                            Action = LogAction.delete,
                            PageId = id,
                            PageName = userName,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }
        /// <summary>
        /// Изменение пароля
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Salt"></param>
        /// <param name="Hash"></param>
        /// <param name="UserId"></param>
        /// <param name="IP"></param>
        public override void changePassword(Guid id, string Salt, string Hash)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var user = db.cms_userss.Where(w => w.id == id);
                    if (user.Any())
                    {
                        string userName = user.Select(s => string.Format("{0} {1}", s.c_surname, s.c_name))
                            .SingleOrDefault();

                        user.Set(p => p.c_salt, Salt)
                            .Set(p => p.c_hash, Hash)
                            .Update();

                        // логирование
                        //insertLog(UserId, IP, "change_pass", id, String.Empty, "Users", logTitle);
                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Users,
                            Action = LogAction.change_pass,
                            PageId = id,
                            PageName = userName,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);

                        tran.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Прикрепление пользователям доступных сайтов
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool updateUserSiteLinks(ContentLinkModel data)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        var getSite = db.cms_sitess.Where(s => s.id == data.LinkId);
                        var alias = getSite.SingleOrDefault().c_alias;

                        //Если существует
                        var linkExist = db.cms_user_site_links
                            .Where(l => l.f_user == data.ObjctId)
                            .Where(l => l.f_site == alias);
                             
                        if (linkExist.Any())
                        {
                            if(!data.Checked)
                            {
                                linkExist.Delete();
                            }
                        }
                        else
                        {
                            if (data.Checked)
                            {
                                var cdUserSiteLink = new cms_user_site_link()
                                {
                                    id = Guid.NewGuid(),
                                    f_user = data.ObjctId,
                                    f_site = alias
                                };
                                db.Insert(cdUserSiteLink);
                            }
                        }

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.Users,
                            Action = LogAction.change_resolutions,
                            PageId = data.ObjctId,
                            PageName = alias,
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
        /// Каталог, список всех доступных на портале групп
        /// </summary>
        /// <returns></returns>
        public override Catalog_list[] getUsersGroupList()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_users_groups.
                    Select(s => new Catalog_list
                    {
                        text = s.c_title,
                        value = s.c_alias
                    });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        /// <summary>
        /// список групп за исключением администратора портала и разработчиков
        /// </summary>
        /// <returns></returns>
        public override Catalog_list[] getUsersGroupListAdmin()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_users_groups
                            .Where(w=>(w.c_alias!= "administrator" && w.c_alias.ToLower()!= "developer"))
                            .Select(s => new Catalog_list
                            {
                                text = s.c_title,
                                value = s.c_alias
                            });

                if (!data.Any()) { return null; }
                else { return data.ToArray(); }
            }
        }
        /// <summary>
        /// Группа
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public override GroupModel getGroup(string alias)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_users_groups
                    .Where(w => w.c_alias == alias)
                    .Select(s => new GroupModel
                    {
                        id = s.id,
                        GroupName = s.c_title,
                        Alias = s.c_alias,
                        GroupResolutions = getGroupResolutions(s.c_alias)
                    });

                if (!data.Any())
                    return null;
                else
                    return data.FirstOrDefault();
            }
        }
        /// <summary>
        /// Получение прав для группы
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public override ResolutionsModel[] getGroupResolutions(string alias)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cms_sv_resolutions_templatess.
                    Where(w => w.f_user_group == alias).
                    OrderBy(o => new { o.f_group, o.n_permit }).
                    Select(s => new ResolutionsModel()
                    {
                        Title = s.c_title,
                        MenuId = s.id,
                        Read = s.b_read,
                        Write = s.b_write,
                        Change = s.b_change,
                        Delete = s.b_delete
                    });

                if (!data.Any())
                    return null;
                else
                    return data.ToArray();
            }
        }
        /// <summary>
        /// Изменение группы, только название группе меняем
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public override bool updateGroup(GroupModel group)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var getGroup = db.cms_users_groups.
                        Where(g => g.c_alias.ToLower() == group.Alias.ToLower());

                    if (getGroup.Any())
                    {
                        var cdGroup = getGroup.SingleOrDefault();
                        
                        cdGroup.c_title = group.GroupName;
                        db.Update(cdGroup);

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.UserGroup,
                            Action = LogAction.update,
                            PageId = cdGroup.id,
                            PageName = group.GroupName,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);
                    }
                    else
                    {
                        var cdGroup = new cms_users_group()
                        {
                            id = Guid.NewGuid(),
                            c_alias = group.Alias,
                            c_title = group.GroupName
                        };
                        db.Insert(cdGroup);

                        //insert claims
                        var templates = db.cms_menus.Select(p => p.id);
                        if (templates != null)
                        {
                            foreach (var template in templates.ToArray())
                            {
                                var claims = new cms_resolutions_templates()
                                {
                                    f_menu_id = template,
                                    f_user_group = group.Alias,
                                    b_read = false,
                                    b_write = false,
                                    b_change = false,
                                    b_delete = false,
                                };
                                db.Insert(claims);
                            }
                        }

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.UserGroup,
                            Action = LogAction.insert,
                            PageId = cdGroup.id,
                            PageName = group.GroupName,
                            UserId = _currentUserId,
                            IP = _ip,
                        };
                        insertLog(log);
                    }

                    tran.Commit();
                    return true;
                }
            }
        }
        /// <summary>
        /// Изменение прав для группы
        /// При этом необходимо всем пользователям этой группы поменять в другой таблице (раньше можно было отдельным пользователям давать отдельные права)
        /// </summary>
        /// <param name="groupClaim"></param>
        /// <returns></returns>
        public override bool updateGroupClaims(GroupClaims groupClaim)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    cms_resolutions_templates cdGroupResolution = null;

                    if (string.IsNullOrEmpty(groupClaim.GroupAlias) || groupClaim.ContentId == Guid.Empty)
                        throw new Exception("updateGroupClaims: alias or contentId Is Null Or Empty");


                    var query = db.cms_resolutions_templatess
                        .Where(t => t.f_menu_id == groupClaim.ContentId)
                        .Where(t => t.f_user_group == groupClaim.GroupAlias);

                    if (!query.Any())
                        cdGroupResolution = new cms_resolutions_templates()
                        {
                            f_user_group = groupClaim.GroupAlias,
                            f_menu_id = groupClaim.ContentId
                        };
                    else
                        cdGroupResolution = query.SingleOrDefault();

                    switch (groupClaim.Claim)
                    {
                        case ClaimType.read:
                            cdGroupResolution.b_read = groupClaim.Checked;
                            break;
                        case ClaimType.write:
                            cdGroupResolution.b_write = groupClaim.Checked;
                            break;
                        case ClaimType.change:
                            cdGroupResolution.b_change = groupClaim.Checked;
                            break;
                        case ClaimType.delete:
                            cdGroupResolution.b_delete = groupClaim.Checked;
                            break;
                        default:
                            return false;
                    }

                    if (!query.Any())
                        db.Insert(cdGroupResolution);
                    else
                        db.Update(cdGroupResolution);


                    //Права пользователей группы
                    var groupUsers = db.cms_userss
                                        .Where(p => p.f_group != null)
                                        .Where(p => p.f_group == groupClaim.GroupAlias);
                    if(groupUsers.Any())
                    {
                        foreach (var user in groupUsers.ToArray())
                        {
                            var userClaims = new UserClaims()
                            {
                                UserId = user.id,
                                ContentId = groupClaim.ContentId,
                                Claim = groupClaim.Claim,
                                Checked = groupClaim.Checked
                            };
                            updateUserClaims(userClaims);
                        }
                    }

                    tran.Commit();
                    return true;
                }
            }
        }

        public override bool updateUserClaims(UserClaims claim)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    cms_resolutions cdUserClaim = null;

                    if (claim.UserId == Guid.Empty || claim.ContentId == Guid.Empty)
                        throw new Exception("updateUserClaims: user or contentId Is Null Or Empty");

                    var query = db.cms_resolutionss
                                .Where(u => u.c_user_id == claim.UserId)
                                .Where(u => u.c_menu_id == claim.ContentId);

                    if (!query.Any())
                    {
                        cdUserClaim = new cms_resolutions()
                        {
                            c_user_id = claim.UserId,
                            c_menu_id = claim.ContentId
                        };
                    }
                    else
                    {
                        cdUserClaim = query.SingleOrDefault();
                    }

                    switch (claim.Claim)
                    {
                        case ClaimType.read:
                            cdUserClaim.b_read = claim.Checked;
                            break;
                        case ClaimType.write:
                            cdUserClaim.b_write = claim.Checked;
                            break;
                        case ClaimType.change:
                            cdUserClaim.b_change = claim.Checked;
                            break;
                        case ClaimType.delete:
                            cdUserClaim.b_delete = claim.Checked;
                            break;
                        }

                    if (!query.Any())
                        db.Insert(cdUserClaim);
                    else
                        db.Update(cdUserClaim);

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public override bool deleteGroup(string alias)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var getGroup = db.cms_users_groups.
                        Where(g => g.c_alias.ToLower() == alias.ToLower());

                    if (!getGroup.Any())
                        return false;

                    var cdGroup = getGroup.SingleOrDefault();
                    var groupId = cdGroup.id;
                    var groupName = cdGroup.c_title;

                    db.cms_resolutions_templatess
                        .Where(g => g.f_user_group == alias)
                        .Delete();
                    getGroup.Delete();

                    var log = new LogModel()
                    {
                        Site = _domain,
                        Section = LogSection.UserGroup,
                        Action = LogAction.delete,
                        PageId = groupId,
                        PageName = groupName,
                        UserId = _currentUserId,
                        IP = _ip,
                    };
                    insertLog(log);

                    tran.Commit();
                    return true;
                }
            }
        }

        #endregion

    }
}
