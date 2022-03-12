using Microsoft.Data.SqlClient;

using System.Data;

using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Interfaces;
using Thunderbird.Infrastructure.Common;
namespace Thunderbird.Infrastructure.Persistance.Repositories {
    public class TerritoryRepository : ITerritoryRepository {
        private readonly IBaseRepository _baseRepository;
        public TerritoryRepository(IBaseRepository baseRepository) {
            _baseRepository = baseRepository;
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
            Division division = new() {
                DivisionId = (reader[DIVISIONID] != DBNull.Value) ? Conversion.ToByte(reader[DIVISIONID]) : byte.MinValue,
                ProvinceId = (reader[PROVINCEID] != DBNull.Value) ? Conversion.ToByte(reader[PROVINCEID]) : byte.MinValue,
                DivisionName = (reader[DIVISIONNAME] != DBNull.Value) ? Conversion.ToString(reader[DIVISIONNAME]) : string.Empty,
                IsActive = (reader[ISACTIVE] != DBNull.Value) ? Conversion.ToBool(reader[ISACTIVE]) : false,
                CreatedBy = (reader[CREATEDBY] != DBNull.Value) ? Conversion.ToInt(reader[CREATEDBY]) : 0,
                CreatedDate = (reader[CREATEDDATE] != DBNull.Value) ? Conversion.ToDateTime(reader[CREATEDDATE]) : DateTime.MinValue,
                UpdateBy = (reader[UPDATEDBY] != DBNull.Value) ? Conversion.ToInt(reader[UPDATEDBY]) : 0,
                UpdateDate = (reader[UPDATEDDATE] != DBNull.Value) ? Conversion.ToDateTime(reader[UPDATEDDATE]) : DateTime.MinValue
            };
            return division;
        }
        public async Task<IList<Division>> GetDivisions(bool? isActive = null) {
            try {
                IList<Division> divisionList = new List<Division>();
                using (SqlConnection sqlConnection = _baseRepository.GetConnection()) {
                    using (SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, PROC_DIVISION_GETALL)) {
                        sqlCommand.Parameters.Add(_baseRepository.GetInParameter(ISACTIVE, SqlDbType.Int, isActive == null ? DBNull.Value : isActive));
                        using var reader = await sqlCommand.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                            divisionList.Add(Mapper(reader));
                    }
                    return divisionList;
                }
            }
            finally { }
        }
        #endregion Functions
    }
}
