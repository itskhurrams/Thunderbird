using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Interfaces;

using System.Data;
using System.Data.Common;
using Thunderbird.Infrastructure.Common;

namespace Thunderbird.Infrastructure.Persistance.Repositories {
    public class UserRepository : IUserRepository {
        public UserRepository() {
        }

        #region SQL Procedures
        protected const string PROC_USER_LOGIN = "[dbo].[Proc_User_Login]";
        #endregion SQL Procedures

        #region Parameters
        protected const string USERID = "user_id";
        protected const string LOGINNAME = "login_name";
        protected const string FIRSTNAME = "first_name";
        protected const string LASTNAME = "last_name";
        protected const string LOGINPASSWORD = "login_password";
        protected const string ISACTIVE = "is_active";
        protected const string CREATEDBY = "created_by";
        protected const string CREATEDDATE = "created_date";
        protected const string UPDATEDBY = "updated_by";
        protected const string UPDATEDDATE = "updated_date";
        #endregion Parameters

        #region Functions
        private static User Mapper(IDataReader reader) {
            var userAccount = new User();
            if (reader[USERID] != null && reader[USERID] != DBNull.Value) {
                userAccount.UserId = Conversion.ToInt(reader[USERID]);
            }
            if (reader[LOGINNAME] != null && reader[LOGINNAME] != DBNull.Value) {
                userAccount.LoginName = Conversion.ToString(reader[LOGINNAME]);
            }
            if (reader[FIRSTNAME] != null && reader[FIRSTNAME] != DBNull.Value) {
                userAccount.FirstName = Conversion.ToString(reader[FIRSTNAME]);
            }
            if (reader[LASTNAME] != null && reader[LASTNAME] != DBNull.Value) {
                userAccount.LastName = Conversion.ToString(reader[LASTNAME]);
            }
            if (reader[ISACTIVE] != null && reader[ISACTIVE] != DBNull.Value) {
                userAccount.IsActive = Conversion.ToBool(reader[ISACTIVE]);
            }
            if (reader[CREATEDBY] != null && reader[CREATEDBY] != DBNull.Value) {
                userAccount.CreatedBy = Conversion.ToInt(reader[CREATEDBY]);
            }
            if (reader[CREATEDDATE] != null && reader[CREATEDDATE] != DBNull.Value) {
                userAccount.CreatedDate = Conversion.ToDateTime(reader[CREATEDDATE]);
            }
            if (reader[UPDATEDBY] != null && reader[UPDATEDBY] != DBNull.Value) {
                userAccount.UpdateBy = Conversion.ToInt(reader[UPDATEDBY]);
            }
            if (reader[UPDATEDDATE] != null && reader[UPDATEDDATE] != DBNull.Value) {
                userAccount.UpdateDate = Conversion.ToDateTime(reader[UPDATEDDATE]);
            }
            return userAccount;
        }
        public Task<User> Login(string loginName, string loginPassword) {
            try {
                User userAccount = new();
                //using (DbCommand dbCommand = _sqlDatabase.GetStoredProcCommand(PROC_USER_LOGIN)) {
                //    _sqlDatabase.AddInParameter(dbCommand, LOGINNAME, DbType.String, loginName);
                //    _sqlDatabase.AddInParameter(dbCommand, LOGINPASSWORD, DbType.String, loginPassword);
                //    using var reader = _sqlDatabase.ExecuteReader(dbCommand);
                //    if (reader.Read())
                //        userAccount = Mapper(reader);
                //}
                return Task.FromResult(userAccount);
            }
            finally { }
        }
        #endregion Functions
    }
}
