using Microsoft.Data.SqlClient;

using System.Data;

namespace Thunderbird.Domain.Interfaces {
    public interface IBaseRepository {
        string GetConnectionString();
        SqlConnection GetConnection();
        SqlCommand GetSqlCommand(SqlConnection sqlConnection, string sqlCommandName, bool isStoredProcedure = true);
        SqlParameter GetInParameter(string sqlParameterName, SqlDbType dbType, object value);
        SqlParameter GetOutParameter(string sqlParameterName, SqlDbType dbType);
        SqlParameter GetInputOutParameter(string sqlParameterName, SqlDbType dbType, object value);
    }
}
