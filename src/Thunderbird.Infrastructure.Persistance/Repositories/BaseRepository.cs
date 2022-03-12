using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using System.Data;

using Thunderbird.Domain.Interfaces;

namespace Thunderbird.Infrastructure.Persistance.Repositories {
    public class BaseRepository : IBaseRepository {
        private string ConnectionString { get; set; }
        private readonly IConfiguration _configuration;
        public BaseRepository(IConfiguration configuration) {
            _configuration = configuration;
            ConnectionString = _configuration["Data:ConnectionString"];
        }
        public string GetConnectionString() {
            return ConnectionString;
        }
        public SqlConnection GetConnection() {
            SqlConnection sqlConnection = new(ConnectionString);
            sqlConnection.Open();
            return sqlConnection;
        }
        public SqlCommand GetSqlCommand(SqlConnection sqlConnection, string sqlCommandName, bool isStoredProcedure) {
            SqlCommand sqlCommand = new(sqlCommandName, sqlConnection) {
                CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text,
                CommandTimeout = Common.Conversion.ToInt(_configuration["Data:DefaultCommandTimeOutDurationSeconds"])
            };
            return sqlCommand;
        }
        public SqlParameter GetInParameter(string sqlParameterName, SqlDbType dbType, object value) {
            SqlParameter sqlParameter = new(sqlParameterName, dbType) { Value = value };
            return sqlParameter;
        }
        public SqlParameter GetOutParameter(string sqlParameterName, SqlDbType dbType) {
            SqlParameter sqlParameter = new(sqlParameterName, dbType) { Direction = ParameterDirection.Output };
            return sqlParameter;
        }
        public SqlParameter GetInputOutParameter(string sqlParameterName, SqlDbType dbType, object value) {
            SqlParameter SqlParameter = new(sqlParameterName, dbType) { Value = value,Direction = ParameterDirection.InputOutput};
            return SqlParameter;
        }
    }
}
