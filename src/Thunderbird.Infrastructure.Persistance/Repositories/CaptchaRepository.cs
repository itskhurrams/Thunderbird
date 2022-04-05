using Microsoft.Data.SqlClient;
using System.Data;
using Thunderbird.Domain.Interfaces;
using Thunderbird.Infrastructure.Common;

namespace Thunderbird.Infrastructure.Persistance.Repositories {
    public class CaptchaRepository : ICaptchaRepository {
        private readonly IBaseRepository _baseRepository;
        public CaptchaRepository(IBaseRepository baseRepository) {
            _baseRepository = baseRepository;
        }
        #region SQL Procedures
        protected const string PROC_CAPTCH_INSERT = "[dbo].[Proc_Captch_Insert]";
        protected const string PROC_CAPTCH_VALIDATE = "[dbo].[Proc_Captch_Validate]";
        #endregion SQL Procedures
        #region Parameters
        protected const string ID = "Id";
        protected const string CAPTCHACODE = "CaptchaCode";
        #endregion Parameters
        public async Task<long> Insert(string CaptchCode) {
            using SqlConnection sqlConnection = _baseRepository.GetConnection();
            using SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, PROC_CAPTCH_INSERT);
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(CAPTCHACODE, SqlDbType.VarChar, CaptchCode));
            return Conversion.ToLong(await sqlCommand.ExecuteScalarAsync());
        }

        public async Task<bool> IsValid(long Id, string CaptchCode) {
           using SqlConnection sqlConnection = _baseRepository.GetConnection();
            using SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, PROC_CAPTCH_VALIDATE);
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(ID, SqlDbType.BigInt, Id));
            sqlCommand.Parameters.Add(_baseRepository.GetInParameter(CAPTCHACODE, SqlDbType.VarChar, CaptchCode));
            return Conversion.ToInt(await sqlCommand.ExecuteScalarAsync()) > 0;
        }
    }
}
