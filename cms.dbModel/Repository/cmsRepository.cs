using cms.dbModel.entity;
using System;
using System.Collections.Generic;

namespace cms.dbModel
{
    public abstract class abstract_cmsRepository
    {
        public abstract string getSiteId(string DomainUrl);
        public abstract Guid currentSiteId();

        public abstract SitesModel getSite(Guid Id);
        public abstract SitesModel getSite(string domain);

        // !!!! Далее для всех методов не нужно передавать  Guid user, string ip пользователя, изменившего запись
        //public abstract bool updateSiteInfo(SitesModel item, Guid user, string ip);
        public abstract bool updateSiteInfo(SitesModel item);

        // Работа с логами
        public abstract cmsLogModel[] getCmsUserLog(Guid UserId);
        public abstract cmsLogModel[] getCmsPageLog(Guid PageId);
        public abstract void insertLog(LogModel log); //старое Guid UserId, string IP, string Action, Guid PageId, string PageName, string Section, string Site);

        // CmsMenu
        public abstract bool check_cmsMenu(Guid id);
        public abstract bool check_cmsMenu(string alias);

        public abstract cmsMenuModel[] getCmsMenu(Guid user_id);
        public abstract cmsMenuItem[] getCmsMenuItems(string group_id, Guid user_id);
        public abstract cmsMenuItem getCmsMenuItem(Guid id);
        public abstract cmsMenuType[] getCmsMenuType();

        public abstract bool createCmsMenu(Guid id, cmsMenuItem Item);
        public abstract bool updateCmsMenu(Guid id, cmsMenuItem Item);
        public abstract bool deleteCmsMenu(Guid id);
        public abstract bool permit_cmsMenu(Guid id, int num);

        // Все сайты портала
        public abstract SitesList getSiteList(SiteFilter filtr);
        public abstract SitesShortModel[] getSiteListWithCheckedForUser(SiteFilter filtr);
        public abstract SitesShortModel[] getSiteListWithCheckedForBanner(SiteFilter filtr);

        public abstract bool check_Site(Guid id);
        public abstract bool insertSite(SitesModel ins);
        public abstract bool updateSite(Guid id, SitesModel ins);
        public abstract bool deleteSite(Guid id);
        public abstract Domain[] getSiteDomains(string SiteId);
        public abstract bool insertDomain(String SiteId, string NewDomain);
        public abstract bool deleteDomain(Guid id);
        public abstract string getSiteId(Guid ContentId);

        // Все пользователи портала
        public abstract UsersList getUsersList(FilterParams filtr);
        public abstract UsersModel getUser(Guid id);
        public abstract bool createUser(Guid id, UsersModel Item);
        public abstract bool createUserOnSite(Guid id, UsersModel Item);
        public abstract bool updateUser(Guid id, UsersModel Item);
        public abstract bool deleteUser(Guid id);

        // Для работы с пользователями
        public abstract bool check_user(Guid id);
        public abstract bool check_user(string email);
        public abstract void check_usergroup(Guid id, string group);
        public abstract void changePassword(Guid id, string Salt, string Hash);
        public abstract bool updateUserSiteLinks(ContentLinkModel link);
        //Все доступные группы на портале - справочник
        public abstract Catalog_list[] getUsersGroupList();
        public abstract Catalog_list[] getUsersGroupListAdmin();


        //Группа пользователей
        public abstract GroupModel getGroup(string alias);
        public abstract bool updateGroup(GroupModel group);
        public abstract bool deleteGroup(string alias);

        public abstract ResolutionsModel[] getGroupResolutions(string alias);
        public abstract bool updateGroupClaims(GroupClaims GroupClaim);
        public abstract bool updateUserClaims(UserClaims claim);

        // Материалы
        public abstract MaterialsGroup[] getAllMaterialGroups();
        public abstract MaterialsGroup[] getMaterialGroups(Guid materialId);

        public abstract MaterialsList getMaterialsList(MaterialFilter filtr);
        public abstract MaterialsModel getMaterial(Guid id);

        public abstract bool insertCmsMaterial(MaterialsModel material);
        public abstract bool updateCmsMaterial(MaterialsModel material);
        public abstract bool deleteCmsMaterial(Guid id);
        public abstract MaterialsGroup[] getMaterialsGroups();

        public abstract bool updateContentLink(ContentLinkModel model);
        
        // Карта сайта
        public abstract SiteMapList getSiteMapList(string site, FilterParams filtr);
        public abstract SiteMapModel getSiteMapItem(Guid id);
        public abstract string[] getSiteMapGroupMenu(Guid id);
        public abstract int getCountSiblings(Guid id);
        public abstract bool checkSiteMap(Guid id);
        public abstract bool ckeckSiteMapAlias(string alias, string ParentId, Guid ThisGuid);
        public abstract bool existSiteMap(string path, string alias, Guid id);
        public abstract bool createSiteMapItem(Guid id, SiteMapModel item);
        public abstract bool updateSiteMapItem(Guid id, SiteMapModel item);
        public abstract bool deleteSiteMapItem(Guid id);
        public abstract bool deleteSiteMapItemCascad(Guid id);

        public abstract SiteMapMenu[] getSiteMapFrontSectionList(string domain);
        public abstract SiteMapMenu getSiteMapMenu(Guid id);
        public abstract Catalog_list[] getSiteMapMenuTypes();
        public abstract bool createOrUpdateSiteMapMenu(SiteMapMenu item);
        public abstract bool deleteSiteMapMenu(Guid id);

        public abstract SiteMapModel[] getSiteMapChildrens(Guid parent);
        public abstract BreadCrumbSiteMap[] getSiteMapBreadCrumbs(Guid? id);
        public abstract BreadCrumbSiteMap getSiteMapBreadCrumbItem(Guid id);
        public abstract bool permit_SiteMap(Guid id, int permit, string menuSort);

        // Баннеры
        public abstract BannersSectionModel[] getSections();
        public abstract BannersSectionModel getSectionItem(Guid id);
        public abstract int getCountBanners(Guid section);
        public abstract BannersListModel getBanners(Guid section, FilterParams filter);
        public abstract BannersModel getBannerItem(Guid id);
        public abstract bool checkBannerExist(Guid id);
        public abstract bool createBanner(Guid id, BannersModel item);
        public abstract bool updateBanner(Guid id, BannersModel item);
        public abstract bool deleteBanner(Guid id);
        public abstract bool permit_Banners(Guid id, int permit);

        //Разделы сайта
        public abstract SiteSectionList getSiteSectionList(FilterParams filtr);
        public abstract SiteSectionModel getSiteSectionItem(Guid id);
        public abstract bool deleteSiteSection(Guid id);
        public abstract bool updateSiteSection(SiteSectionModel upd);
        public abstract bool insertSiteSection(SiteSectionModel sitesection);
        
        //Документы
        public abstract DocumentsModel[] getDocuments(Guid id);
        public abstract bool insDocuments(DocumentsModel insert);
        public abstract DocumentsModel getDocumentsPath(Guid id);
        public abstract bool deleteSiteMapDocuments(Guid id);
        public abstract bool permit_Documents(Guid id, int num);

        //фотоальбом
        public abstract PhotoAlbumList getPhotoAlbum(FilterParams filter);
        public abstract PhotoAlbum getPhotoAlbumItem(Guid id);
        public abstract bool insPhotoAlbum(Guid id, PhotoAlbum ins);
        public abstract bool updPhotoAlbum(Guid id, PhotoAlbum upd);
        public abstract bool delPhotoAlbum(Guid id);
        public abstract bool insertPhotos(Guid AlbumId, PhotoModel[] insert);
        public abstract bool sortingPhotos(Guid id, int num);
        public abstract PhotoModel getPhotoItem(Guid id);
        public abstract bool delPhotoItem(Guid id);

        // категории товаров
        public abstract CategoryModel[] getAllCategories();
        public abstract CategoryModel[] getCategories(Guid? parent);
        public abstract CategoryModel getCategory(Guid id);
        public abstract BreadCrumbCategory getBreadCrumbCategory(Guid id);
        public abstract BreadCrumbCategory[] getCategoryBreadCrumbs(Guid? id);
        public abstract bool categoryExists(Guid id);
        public abstract bool createCategory(CategoryModel item);
        public abstract bool updateCategory(CategoryModel item);
        public abstract bool deleteCategory(Guid id);

        // продукция
        public abstract ProductList getProducts(FilterParams filter);
        public abstract ProductModel getProduct(Guid id);
        public abstract bool updateProduct(ProductModel item);
        public abstract bool insertProduct(ProductModel item);
        public abstract bool deleteProduct(Guid id);
    }
}