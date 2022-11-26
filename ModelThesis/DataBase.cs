using System;
using System.Collections.Generic;
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
        public Dictionary<string, Guid[]> SelectUuid(string tableName)
        {
            var result = new Dictionary<string, Guid[]>();
            using (var cnn = new SqlConnection(this.ConnectionString))
            {
                var sql = $"select * from {tableName}";
                var command = new SqlCommand(sql, cnn);
                command.Connection.Open();
                var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    var tempList = new List<Guid>();
                    for (int i = 2; i < dataReader.FieldCount; i++)
                    {
                        tempList.Add(new Guid(dataReader.GetString(i)));
                    }
                    result[dataReader.GetString(1)] = tempList.ToArray();
                }
            }
            
            return result;
        }

        public void InsertData(string tableName, string data)
        {
            using (var cnn = new SqlConnection(this.ConnectionString))
            {
                string sql = $"INSERT INTO {tableName} VALUES ({data})";
                SqlCommand command = new SqlCommand(sql, cnn);
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Dispose();
            }
        }

        public string PreparingInsertCalcResult(double value, string time)
        {
            var lastId = "0";
            var stringValue = "";
            using (var cnn = new SqlConnection(this.ConnectionString))
            { 
                var sql = $"SELECT TOP 1 * FROM {nameof(DataBaseTables.Calculations)} ORDER BY ID DESC";
                var command = new SqlCommand(sql, cnn);
                command.Connection.Open();
                var dataReader = command.ExecuteReader();

                stringValue = value.ToString().Replace(",", ".");

                while (dataReader.Read())
                {
                    lastId = dataReader.GetValue(0).ToString();
                }
            }
            
            var id = Convert.ToInt32(lastId) + 1;

            return $"{id}, {stringValue}, '{time}'";
        }

        private PerformanceIndex GetLastPerformanceIndex()
        {
            var id = "0";
            var value = "0";
            var timeStamp = "0";

            using (var cnn = new SqlConnection(this.ConnectionString))
            {
                var sql = $"SELECT TOP 1 * FROM {nameof(DataBaseTables.Calculations)} ORDER BY ID DESC";
                var command = new SqlCommand(sql, cnn);
                command.Connection.Open();
                var dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    id = dataReader.GetValue(0).ToString();
                    value = dataReader.GetValue(1).ToString();
                    timeStamp = dataReader.GetValue(2).ToString();
                }
            }

            return new PerformanceIndex
                (Convert.ToInt32(id), Convert.ToDouble(value), Convert.ToDateTime(timeStamp));
        }
        
        public double GetIncrementOfIndex(double value)
        {
            var preValue = this.GetLastPerformanceIndex();

            return preValue.Value - value;
        }

        public double GetRateOfChange(double value, DateTime time)
        {
            var increment = this.GetIncrementOfIndex(value);
            var timeDiff = time.Subtract(this.GetLastPerformanceIndex().TimeStamp);
            if (timeDiff == TimeSpan.Zero)
            {
                throw new ArgumentException("Одинаковове время двух последних расчетов");
            }
            return increment / timeDiff.TotalSeconds;
        }
    }
}
