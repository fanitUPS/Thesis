using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ModelThesis
{
    public class DataBase
    {
        private string _connectionString;

        public string ConnectionString { get; set; }

        public DataBase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string[] SelectData(DataBaseTables tableName)
        {
            var cnn = new SqlConnection(_connectionString);
            cnn.Open();
            var nameTable = nameof(tableName);
            var sql = String.Format("select * from {0}", nameTable);
            var command = new SqlCommand(sql, cnn);
            var dataReader = command.ExecuteReader();
            var result = new List<string>();
            while (dataReader.Read())
            {
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    result.Add(dataReader.GetString(i));
                }
            }
            cnn.Close();
            return result.ToArray();
        }
    }
}
