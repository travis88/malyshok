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
    /// Репозиторий для работы с фотоальбомами
    /// </summary>
    public partial class cmsRepository : abstract_cmsRepository
    {
        public override PhotoAlbumList getPhotoAlbum(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                if (!string.IsNullOrEmpty(filter.Domain))
                {
                    var query = db.content_photoalbums.Where(w => w.f_site == filter.Domain);
                    if (filter.SearchText != null) {
                        query = query.Where(w => (w.c_title.Contains(filter.SearchText)));
                    }
                    if (filter.Disabled != null) {
                        query = query.Where(w => w.c_disabled == filter.Disabled);
                    }
                    query = query.OrderByDescending(o => o.d_date);
                    int itemCount = query.Count();
                    if (query.Any())
                    {
                    var photoalbumsList = query
                                            .Skip(filter.Size * (filter.Page - 1))
                                            .Take(filter.Size)
                                            .Select(s => new PhotoAlbum
                                            {
                                            Id = s.id,
                                            Title = s.c_title,
                                            Date = s.d_date,
                                            Text = s.c_text,
                                            PreviewImage = new Photo() { Url = s.c_preview }
                                            });
                        return new PhotoAlbumList
                        {
                            Data = photoalbumsList.ToArray(),
                            Pager = new Pager
                            {
                                page = filter.Page,
                                size = filter.Size,
                                items_count = itemCount,
                                page_count = (itemCount % filter.Size > 0) ? (itemCount / filter.Size) + 1 : itemCount / filter.Size
                            }
                        };
                }
                }
                return null;
            }
        }
        public override PhotoAlbum getPhotoAlbumItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_photoalbums
                           .Where(w => w.id == id)
                           .Select(s => new PhotoAlbum {
                               Id=s.id,
                               Path=s.c_path,
                               Title=s.c_title,
                               Date=s.d_date,
                               PreviewImage = new Photo() { Url = s.c_preview },
                               Text=s.c_text
                           });
                if (query.Any())
                {
                    var data = query.Single();
                    data.Photos = db.content_photoss
                                   .Where(w => w.f_album == id)
                                   .OrderBy(o=>o.n_sort)
                                   .Select(s=>new PhotoModel() {
                                       PreviewImage=new Photo { Url=s.c_preview},
                                       Id=s.id,
                                       Title=s.c_title
                                   }).ToArray();
                    return data;
                }
                return null;
            }
        }
        
        public override bool insPhotoAlbum(Guid id, PhotoAlbum ins)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_photoalbum cdPhotoAlbum = db.content_photoalbums
                                                .Where(p => p.id == ins.Id)
                                                .SingleOrDefault();
                        if (cdPhotoAlbum != null)
                        {
                            throw new Exception("Запись с таким Id уже существует");
                        }
                        
                        cdPhotoAlbum = new content_photoalbum
                        {
                            id = ins.Id,
                            f_site=_domain,
                            c_path=ins.Path,
                            c_title = ins.Title,                            
                            c_text = ins.Text,
                            d_date = ins.Date,
                            c_preview = (ins.PreviewImage != null) ? ins.PreviewImage.Url : null                         
                        };

                        db.Insert(cdPhotoAlbum);

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.PhotoAlbums,
                            Action = LogAction.insert,
                            PageId = ins.Id,
                            PageName = ins.Title,
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
        public override bool updPhotoAlbum(Guid id, PhotoAlbum upd)
        {            
            try
            {
                using (var db = new CMSdb(_context))
                {
                    using (var tran = db.BeginTransaction())
                    {
                        content_photoalbum cdPhoto = db.content_photoalbums
                                                .Where(p => p.id == upd.Id)
                                                .SingleOrDefault();
                        if (cdPhoto == null)
                            throw new Exception("Запись с таким Id не найдена");

                        cdPhoto.c_title = upd.Title;                        
                        cdPhoto.c_text = upd.Text;
                        cdPhoto.d_date = upd.Date;
                        cdPhoto.c_preview =(upd.PreviewImage == null) ? cdPhoto.c_preview : upd.PreviewImage.Url;
                        db.Update(cdPhoto);                        

                        var log = new LogModel()
                        {
                            Site = _domain,
                            Section = LogSection.PhotoAlbums,
                            Action = LogAction.update,
                            PageId = upd.Id,
                            PageName = upd.Title,
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
        public override bool delPhotoAlbum(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_photoalbums
                           .Where(w => w.id == id);
                if (data.Any())
                {
                    data.Delete();
                    return true;
                }
                return false;
            }
        }


        public override bool insertPhotos(Guid AlbumId, PhotoModel[] insert)
        {
            using (var db = new CMSdb(_context))
            {
                //try
                //{
                    var queryMaxSort = db.content_photoss
                                         .Where(w => w.f_album==AlbumId)                        
                                         .Select(s => s.n_sort);
                    int maxSort = queryMaxSort.Any() ? queryMaxSort.Max() + 1 : 0;
                if (insert != null)
                {
                    if(insert.Length>0)
                    foreach (PhotoModel item in insert)
                    {
                        if (item != null)
                        {
                            maxSort++;
                            db.content_photoss
                              .Value(v => v.f_album, AlbumId)
                              .Value(v => v.c_title, item.Title)
                              .Value(v => v.d_date, item.Date)
                              .Value(v => v.c_preview, item.PreviewImage.Url)
                              .Value(v => v.c_photo, item.PhotoImage.Url)
                              .Value(v => v.n_sort, maxSort)
                              .Insert();
                        }                        
                    }
                }
                    
                    return true;
                //}
                //catch { return false; }
            }
        }
        public override bool sortingPhotos(Guid id, int num)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.content_photoss.Where(w => w.id == id).Select(s => new PhotoModel { AlbumId = s.f_album, Sort= s.n_sort }).First();
                var AlbumId = data.AlbumId;

                if (num > data.Sort)
                {
                    db.content_photoss.Where(w => w.f_album == AlbumId && w.n_sort > data.Sort && w.n_sort <= num)
                        .Set(p => p.n_sort, p => p.n_sort - 1)
                        .Update();
                }
                else
                {
                    db.content_photoss.Where(w => w.f_album == AlbumId && w.n_sort < data.Sort && w.n_sort >= num)
                        .Set(p => p.n_sort, p => p.n_sort + 1)
                        .Update();
                }
                db.content_photoss
                    .Where(w => w.id == id)
                    .Set(s => s.n_sort, num)
                    .Update();
            }
            return true;
        }

        public override PhotoModel getPhotoItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.content_photoss.Where(w => w.id == id);
                if (query.Any())
                {
                    return query.Select(s => new PhotoModel
                    {
                        Id = s.id,
                        PhotoImage = new Photo { Url = s.c_photo },
                        PreviewImage = new Photo { Url = s.c_preview }
                    }).SingleOrDefault();

                }
                return null;
        }
        }
        public override bool delPhotoItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {

                    var data = db.content_photoss.Where(w=>w.id==id);
                    if (data.Any())
                    {
                        Guid AlbumId = data.Single().f_album;
                        int ThisSort = data.Single().n_sort;
                        // удаление фотографии
                        data.Delete();
                        //корректировка порядка
                        db.content_photoss
                            .Where(w => (w.f_album == AlbumId && w.n_sort > ThisSort))
                            .Set(u => u.n_sort, u => u.n_sort - 1)
                            .Update();
                        tran.Commit();
                        return true;
                    }
                    return false;
                }                    

    }
}
    }
}
