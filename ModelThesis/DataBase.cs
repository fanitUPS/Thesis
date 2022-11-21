using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ModelThesis
{
    /// <summary>
    /// Класс работы с БД
    /// </summary>
    public class DataBase
    {
        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="connectionString">Строка подключения к БД</param>
        public DataBase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Метод запроса данных из БД
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
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
