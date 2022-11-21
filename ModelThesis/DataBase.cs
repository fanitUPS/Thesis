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
        public string ConnectionString { get; set; }

        public DataBase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public Dictionary<string, Guid[]> SelectData(string tableName)
        {
            var cnn = new SqlConnection(this.ConnectionString);
            cnn.Open();
            var sql = String.Format("select * from {0}", tableName);
            var command = new SqlCommand(sql, cnn);
            var dataReader = command.ExecuteReader();
            var result = new Dictionary<string, Guid[]>();
            while (dataReader.Read())
            {
                var tempList = new List<Guid>();
                for (int i = 2; i < dataReader.FieldCount; i++)
                {
                    tempList.Add(new Guid(dataReader.GetString(i)));
                }
                result[dataReader.GetString(1)] = tempList.ToArray();
            }
            cnn.Close();
            return result;
        }
    }
}
