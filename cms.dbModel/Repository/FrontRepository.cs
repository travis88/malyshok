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
        public abstract Breadcrumbs[] getCatalogBreadCrumb(string Url);

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
        public abstract UsersModel getCustomer(string Id);
        public abstract bool createCustomer(UsersModel item);
        public abstract UsersModel updateCustomer(UsersModel item);
        public abstract UsersModel SetCustromerSocialNetwork(Guid userId, string type, string socialId);
        //public abstract bool deleteCustomer(Guid id);

        public abstract int FailedLogin(Guid id, string IP);
        public abstract void setRestorePassCode(Guid id, Guid RestoreCode);
        public abstract bool getCmsAccountCode(Guid RestoreCode);
        public abstract void changePasByCode(Guid id, string NewSalt, string NewHash);
        public abstract void changePassword(Guid UserId, string NewSalt, string NewHash);

        // Продукция
        public abstract CategoryModel[] getProdCatalogModule();
        public abstract CategoryTree getProdCatalog(string Path);
        public abstract ProductList getProdList(FilterParams filter);
        public abstract ProductModel getProdItem(Guid id, Guid Order);
        public abstract Catalog_list[] getCertificates(Guid ProdId);

        // Заказы
        public abstract bool CheckOrder(Guid OrderId);
        public abstract Guid getOrderId(Guid UserId);
        public abstract OrdersList getOrderList(Guid UserId, FilterParams filter);
        public abstract OrderModel getOrder(Guid OrderId);
        public abstract OrderDetails[] getOrderDetails(Guid OrderId);
        public abstract Guid CreateOrder();
        public abstract Guid CreateOrder(Guid UserId);
        public abstract bool transferOrder(Guid OrderId, Guid UserId);
        public abstract int sendOrder(OrderModel OrderInfo);
        public abstract void removeFromBasket(Guid OrderId, Guid ProdId);
        //public abstract OrderModel[] getOrders(Guid UserId);

        // Корзина
        public abstract bool addInBasket(Guid OrderId, Guid id, int Count);
        public abstract OrderModel getBasketInfo(Guid OrderId);
        public abstract ProductModel[] getBasketItems(Guid OrderId);
        
        public abstract Catalog_list[] getfiltrParams(string type);
    }
}