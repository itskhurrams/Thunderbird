using Microsoft.Data.SqlClient;

using System.Data;

using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Interfaces;
using Thunderbird.Infrastructure.Common;

namespace Thunderbird.Infrastructure.Persistance.Repositories {
    public class UserRepository : IUserRepository {
        private readonly IBaseRepository _baseRepository;
        public UserRepository(IBaseRepository baseRepository) {
            _baseRepository = baseRepository;
        }
        #region SQL Procedures
        protected const string PROC_USER_GET_BY_LOGIN_NAME = "[dbo].[Proc_User_GetByLoginName]";
        protected const string PROC_USER_UPDATE_PASSWORD = "[dbo].[Proc_User_UpdatePassword]";
        protected const string PROC_USER_REGISTER = "[dbo].[Proc_User_Register]";
        #endregion SQL Procedures

        #region Parameters
        protected const string USERID = "user_id";
        protected const string LOGINNAME = "login_name";
        protected const string FIRSTNAME = "first_name";
        protected const string LASTNAME = "last_name";
        protected const string LOGINPASSWORD = "login_password";
        protected const string EMAIL = "email";
        protected const string PHONENUMBER = "phone_number";
        protected const string ISACTIVE = "is_active";
        protected const string CREATEDBY = "created_by";
        protected const string CREATEDDATE = "created_date";
        protected const string UPDATEDBY = "updated_by";
        protected const string UPDATEDDATE = "updated_date";
        #endregion Parameters

        #region Functions
        private static User Mapper(IDataReader reader) {
            User userAccount = new() {
                UserId = (reader[USERID] != DBNull.Value) ? Conversion.ToInt(reader[USERID]) : 0,
                LoginName = (reader[LOGINNAME] != DBNull.Value) ? Conversion.ToString(reader[LOGINNAME]) : string.Empty,
                LoginPassword = (reader[LOGINPASSWORD] != DBNull.Value) ? Conversion.ToString(reader[LOGINPASSWORD]) : string.Empty,
                FirstName = (reader[FIRSTNAME] != DBNull.Value) ? Conversion.ToString(reader[FIRSTNAME]) : string.Empty,
                LastName = (reader[LASTNAME] != DBNull.Value) ? Conversion.ToString(reader[LASTNAME]) : string.Empty,
                Email = (reader[EMAIL] != DBNull.Value) ? Conversion.ToString(reader[EMAIL]) : string.Empty,
                PhoneNumber = (reader[PHONENUMBER] != DBNull.Value) ? Conversion.ToString(reader[PHONENUMBER]) : string.Empty,
                IsActive = (reader[ISACTIVE] != DBNull.Value) ? Conversion.ToBool(reader[ISACTIVE]) : false,
                CreatedBy = (reader[CREATEDBY] != DBNull.Value) ? Conversion.ToInt(reader[CREATEDBY]) : 0,
                CreatedDate = (reader[CREATEDDATE] != DBNull.Value) ? Conversion.ToDateTime(reader[CREATEDDATE]) : DateTime.MinValue,
                UpdateBy = (reader[UPDATEDBY] != DBNull.Value) ? Conversion.ToInt(reader[UPDATEDBY]) : 0,
                UpdateDate = (reader[UPDATEDDATE] != DBNull.Value) ? Conversion.ToDateTime(reader[UPDATEDDATE]) : DateTime.MinValue
            };
            return userAccount;
        }

        public async Task<User?> GetByLoginName(string loginName) {
            using SqlConnection sqlConnection = _baseRepository.GetConnection();
            using SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, PROC_USER_GET_BY_LOGIN_NAME);
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(LOGINNAME, SqlDbType.NVarChar, loginName));
            using var reader = await sqlCommand.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Mapper(reader) : null;
        }

        public async Task UpdatePassword(long userId, string hashedPassword) {
            using SqlConnection sqlConnection = _baseRepository.GetConnection();
            using SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, PROC_USER_UPDATE_PASSWORD);
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(USERID, SqlDbType.BigInt, userId));
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(LOGINPASSWORD, SqlDbType.NVarChar, hashedPassword));
            await sqlCommand.ExecuteNonQueryAsync();
        }

        public async Task<long> Register(string loginName, string hashedPassword, string firstName, string lastName, string email, string phoneNumber) {
            using SqlConnection sqlConnection = _baseRepository.GetConnection();
            using SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, PROC_USER_REGISTER);
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(LOGINNAME, SqlDbType.NVarChar, loginName));
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(LOGINPASSWORD, SqlDbType.NVarChar, hashedPassword));
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(FIRSTNAME, SqlDbType.NVarChar, firstName));
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(LASTNAME, SqlDbType.NVarChar, lastName));
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(EMAIL, SqlDbType.NVarChar, email));
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(PHONENUMBER, SqlDbType.NVarChar, phoneNumber));
            return Conversion.ToLong(await sqlCommand.ExecuteScalarAsync());
        }
        #endregion Functions
    }
}
