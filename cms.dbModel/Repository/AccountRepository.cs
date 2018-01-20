using cms.dbModel.entity;
using System;

namespace cms.dbModel
{
    public abstract class abstract_AccountRepository
    {
        public abstract AccountModel getCmsAccount(string Email);
        public abstract AccountModel getCmsAccount(Guid Id);

        public abstract bool getCmsAccountCode(Guid Code);

        public abstract DomainList[] getUserDomains(Guid Id);

        public abstract ResolutionsModel getCmsUserResolutioInfo(Guid _userId, string _pageUrl);

        public abstract void changePasswordUser(Guid id, string NewSalt, string NewHash, string IP);
        public abstract void changePasByCode(Guid Code, string NewSalt, string NewHash, string IP);

        public abstract void SuccessLogin(Guid id, string IP);
        public abstract int FailedLogin(Guid id, string IP);
        public abstract void setRestorePassCode(Guid id, Guid Code, string IP);
        
        public abstract void insertLog(Guid UserId, string IP, string Action, Guid PageId, string PageName, string Section, string Site);
    }
}