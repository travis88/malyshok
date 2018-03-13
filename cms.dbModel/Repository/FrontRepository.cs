using cms.dbModel.entity;
using System;
using System.Collections.Generic;

namespace cms.dbModel
{
    public abstract class abstract_FrontRepository
    {
        public abstract string getView(string siteSection); //string siteId,
        public abstract SitesModel getSiteInfo(); //string domain
        public abstract SiteMapModel[] getMenu(string section);

        // Карта сайта
        public abstract SiteMapModel getSiteMap(string path, string alias); //, string domain
        public abstract SiteMapModel getSiteMap(string frontSection);
        //public abstract string[] getSiteMapSiblings(string path);
        //public abstract List<SiteMapModel> getSiteMapSiblingElements(string path);
        public abstract SiteMapModel[] getSiteMapChild(Guid ParentId);
        public abstract Breadcrumbs[] getBreadCrumb(string Url); //, string domain

        //Banners
        public abstract BannersModel[] getBanners(); //string domain
        public abstract BannersModel getBanner(Guid id);

        //Materials
        public abstract List<MaterialFrontModule> getMaterialsModule(); //string domain
        public abstract MaterialsList getMaterialsList(FilterParams filtr);
        public abstract MaterialsModel getMaterialsItem(string year, string month, string day, string alias); //, string domain
        //public abstract MaterialsGroup[] getMaterialsGroup();
        
        //Attached Documents
        public abstract DocumentsModel[] getAttachDocuments(Guid id);

        //фотоальбом
        public abstract PhotoModel[] getPhotoList(Guid id);

        // Пользователи
        public abstract bool CheckCustomerMail(string Mail);
        public abstract string ConfirmMail(Guid Code);
        public abstract UsersModel getCustomer(Guid id);
        public abstract bool createCustomer(UsersModel item);
        //public abstract bool updateCustomer(UsersModel item);
        //public abstract bool deleteCustomer(Guid id);

        // Продукция
        public abstract CategoryModel[] getProdCatalogModule();
        public abstract ProductList getProdList(FilterParams filter);

        // Заказы
        public abstract bool CheckOrder(Guid OrderId);
        public abstract Guid? CreateOrder();
        //public abstract OrderModel[] getOrders(Guid UserId);

        // Корзина
        public abstract bool addInBasket(Guid OrderId, Guid id, int Count);
        public abstract OrderModel getBasketInfo(Guid OrderId);
        //public abstract OrderModel getBasket(Guid OrderId);
    }
}