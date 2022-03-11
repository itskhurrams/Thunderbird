using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Interfaces;

using System.Data;
using System.Data.Common;
using Thunderbird.Infrastructure.Common;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Thunderbird.Infrastructure.Persistance {
    public class TerritoryRepository : ITerritoryRepository {
        private readonly IConfiguration _configuration;
        public TerritoryRepository(IConfiguration configuration) {
            _configuration = configuration;
        }

        #region SQL Procedures
        protected const string PROC_DIVISION_GETALL = "[dbo].[Proc_Division_GetAll]";
        #endregion SQL Procedures

        #region Parameters
        protected const string DIVISIONID = "division_id";
        protected const string PROVINCEID = "province_id";
        protected const string DIVISIONNAME = "division_name";
        protected const string ISACTIVE = "is_active";
        protected const string CREATEDBY = "created_by";
        protected const string CREATEDDATE = "created_date";
        protected const string UPDATEDBY = "updated_by";
        protected const string UPDATEDDATE = "updated_date";
        #endregion Parameters

        #region Functions
        private static Division Mapper(IDataReader reader) {
            var division = new Division();
            if (reader[DIVISIONID] != null && reader[DIVISIONID] != DBNull.Value) {
                division.DivisionId = Conversion.ToByte(reader[DIVISIONID]);
            }
            if (reader[PROVINCEID] != null && reader[PROVINCEID] != DBNull.Value) {
                division.ProvinceId = Conversion.ToByte(reader[PROVINCEID]);
            }
            if (reader[DIVISIONNAME] != null && reader[DIVISIONNAME] != DBNull.Value) {
                division.DivisionName = Conversion.ToString(reader[DIVISIONNAME]);
            }
            if (reader[ISACTIVE] != null && reader[ISACTIVE] != DBNull.Value) {
                division.IsActive = Conversion.ToBool(reader[ISACTIVE]);
            }
            if (reader[CREATEDBY] != null && reader[CREATEDBY] != DBNull.Value) {
                division.CreatedBy = Conversion.ToInt(reader[CREATEDBY]);
            }
            if (reader[CREATEDDATE] != null && reader[CREATEDDATE] != DBNull.Value) {
                division.CreatedDate = Conversion.ToDateTime(reader[CREATEDDATE]);
            }
            if (reader[UPDATEDBY] != null && reader[UPDATEDBY] != DBNull.Value) {
                division.UpdateBy = Conversion.ToInt(reader[UPDATEDBY]);
            }
            if (reader[UPDATEDDATE] != null && reader[UPDATEDDATE] != DBNull.Value) {
                division.UpdateDate = Conversion.ToDateTime(reader[UPDATEDDATE]);
            }
            return division;
        }
        public async Task<IList<Division>> GetDivisions(bool? isActive = null) {

            IList<Division> divisionList = new List<Division>();
            using SqlConnection sqlConnection = new(_configuration["Data:ConnectionString"]);
            sqlConnection.OpenAsync().Wait();
            using SqlCommand sqlCommand = new(PROC_DIVISION_GETALL, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new(ISACTIVE, isActive == null ? DBNull.Value : isActive));
            using var reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                divisionList.Add(Mapper(reader));
            return divisionList;
        }
        #endregion Functions
    }
}
