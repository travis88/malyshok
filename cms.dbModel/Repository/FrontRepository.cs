using cms.dbModel.entity;
using System;
using System.Collections.Generic;

namespace cms.dbModel
{
    public abstract class abstract_FrontRepository
    {
        public abstract string getSiteId(string domainUrl);
        public abstract string getView(string siteSection); //string siteId,
        public abstract SitesModel getSiteInfo(); //string domain
        public abstract string getDomainSite();
        public abstract UsersModel[] getSiteAdmins();
        public abstract SiteMapModel[] getSiteMapList(); //string domain
        public abstract SiteMapModel[] getSiteMapListShort(string path); //string domain
        public abstract string[] getSiteMapGroupMenu(Guid id);

        //Redirect from old portal methods
        public abstract SitesModel getSiteInfoByOldId(int id);
        public abstract SiteMapModel getSiteMapByOldId(int id);
        public abstract MaterialsModel getMaterialsByOldId(int id);

        // Карта сайта
        public abstract SiteMapModel getSiteMap(string path, string alias); //, string domain
        public abstract SiteMapModel getSiteMap(string frontSection);
        public abstract string[] getSiteMapSiblings(string path);
        public abstract List<SiteMapModel> getSiteMapSiblingElements(string path);
        public abstract SiteMapModel[] getSiteMapChild(Guid ParentId);
        public abstract List<Breadcrumbs> getBreadCrumbCollection(string Url); //, string domain

        //Banners
        public abstract BannersModel[] getBanners(); //string domain
        public abstract BannersModel getBanner(Guid id);

        //Materials
        public abstract List<MaterialFrontModule> getMaterialsModule(); //string domain
        public abstract MaterialsList getMaterialsList(FilterParams filtr);
        public abstract MaterialsModel getMaterialsItem(string year, string month, string day, string alias); //, string domain
        public abstract MaterialsGroup[] getMaterialsGroup();
        
        //Attached Documents
        public abstract DocumentsModel[] getAttachDocuments(Guid id);

        //фотоальбом
        public abstract PhotoModel[] getPhotoList(Guid id);

        // Продукция
        public abstract CategoryModel[] getProdCatalogModule();
    }
}