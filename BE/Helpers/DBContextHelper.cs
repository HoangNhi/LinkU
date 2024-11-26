using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Reflection;
using ENTITIES.DbContent;
using Microsoft.EntityFrameworkCore;

namespace BE.Helpers
{
    public static class DBContextHelper
    {
        public static IEnumerable<TModel> ExcuteStoredProcedure<TModel>(this LINKUContext _context, string nameofStored, SqlParameter[]? parameters) where TModel : new()
        {
            if (_context.Database.GetDbConnection().State != ConnectionState.Open)
            {
                _context.Database.OpenConnection();
            }

            using DbCommand dbCommand = _context.Database.GetDbConnection().CreateCommand();
            dbCommand.CommandTimeout = 180;
            dbCommand.CommandType = CommandType.StoredProcedure;
            dbCommand.CommandText = nameofStored;
            if (parameters != null)
            {
                dbCommand.Parameters.AddRange(parameters);
            }

            DataTable dataTable = new DataTable();
            using (DbDataReader reader = dbCommand.ExecuteReader())
            {
                dataTable.Load(reader);
            }

            dbCommand.Parameters.Clear();

            List<TModel> resultList = new List<TModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                TModel instance = new TModel();
                foreach (PropertyInfo prop in typeof(TModel).GetProperties())
                {
                    if (dataTable.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                    {
                        prop.SetValue(instance, row[prop.Name]);
                    }
                }
                resultList.Add(instance);
            }
            return resultList;
        }
    }
}
