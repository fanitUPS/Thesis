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

        private const string _timePattern = "yyyy-MM-dd HH:mm:ss";

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

        /// <summary>
        /// Запись результата расчета в БД
        /// </summary>
        /// <param name="tableName">Имя таблицы, куда записывается результат</param>
        /// <param name="data">Данные для записи</param>
        public void InsertPerformanceIndex(string tableName, PerformanceIndex index)
        {
            var data = this.PreparingInsertCalcResult(index.Value, index.TimeStamp.ToString(_timePattern));
            using (var cnn = new SqlConnection(this.ConnectionString))
            {
                string sql = $"INSERT INTO {tableName} VALUES ({data})";
                SqlCommand command = new SqlCommand(sql, cnn);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Подготовка данных для записи
        /// </summary>
        /// <param name="value">Значение показателя</param>
        /// <param name="time">Метка времени</param>
        /// <returns>Подготовленные данные</returns>
        private string PreparingInsertCalcResult(double value, string time)
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

        /// <summary>
        /// Получение последнего показателя тяжести
        /// </summary>
        /// <returns>Показатель тяжести</returns>
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

        /// <summary>
        /// Расчет приращения показателя тяжести
        /// </summary>
        /// <param name="index">Показатель тяжести</param>
        /// <returns>Приращение показателя тяжести</returns>
        public double GetIncrementOfIndex(PerformanceIndex index)
        {
            var preValue = this.GetLastPerformanceIndex();

            return Math.Round(preValue.Value - index.Value, 5);
        }

        /// <summary>
        /// Расчет скорости изменения показателя тяжести
        /// </summary>
        /// <param name="index">Показатель тяжести</param>
        /// <returns>Скорость изменения показателя тяжести</returns>
        /// <exception cref="ArgumentException">Исключение</exception>
        public double GetRateOfChange(PerformanceIndex index)
        {
            var increment = this.GetIncrementOfIndex(index);
            var timeDiff = index.TimeStamp.Subtract(this.GetLastPerformanceIndex().TimeStamp);
            if (timeDiff == TimeSpan.Zero)
            {
                throw new ArgumentException("Одинаковое время двух последних расчетов");
            }
            return Math.Round(increment / timeDiff.TotalSeconds, 5) * 100;
        }

        private string GetLastId(string tableName)
        {
            var id = "0";
            var listUuidFromDb = new List<UuidContainer>();

            using (var cnn = new SqlConnection(this.ConnectionString))
            {
                var sqlSelectId = $"SELECT TOP 1 * FROM {tableName} ORDER BY ID DESC";
                var commandSelectId = new SqlCommand(sqlSelectId, cnn);
                commandSelectId.Connection.Open();
                var dataReader = commandSelectId.ExecuteReader();
                while (dataReader.Read())
                {
                    id = dataReader.GetValue(0).ToString();
                }
            }
            return id;
        }

        public List<UuidContainer> GetUuidsFromDb(string tableName)
        {
            var listUuidFromDb = new List<UuidContainer>();

            using (var cnn = new SqlConnection(this.ConnectionString))
            {
                var sqlSelect = $"SELECT * FROM {tableName}";
                var commandSelect = new SqlCommand(sqlSelect, cnn);
                commandSelect.Connection.Open();
                var dataReader = commandSelect.ExecuteReader();

                while (dataReader.Read())
                {
                    var nameUuid = dataReader.GetValue(1).ToString();
                    var factUuid = dataReader.GetValue(2).ToString();
                    var maxUuid = dataReader.GetValue(3).ToString();

                    if (tableName == nameof(DataBaseTables.Voltages))
                    {
                        var minUuid = dataReader.GetValue(4).ToString();
                        var nomUuid = dataReader.GetValue(5).ToString();
                        listUuidFromDb.Add(new UuidContainer(nameUuid, factUuid, maxUuid, minUuid, nomUuid));
                    }
                    else
                    {
                        listUuidFromDb.Add(new UuidContainer(nameUuid, factUuid, maxUuid));
                    }
                }
            }
            return listUuidFromDb;
        }

        public void InsertUuids(List<UuidContainer> listUuidFromModel, string tableName)
        {
            var listUuidFromDb = GetUuidsFromDb(tableName);

            foreach (var valueModel in listUuidFromModel)
            {
                foreach (var valueDb in listUuidFromDb)
                {
                    if (valueDb.Value != valueModel.Value || valueDb.MaxValue != valueModel.MaxValue ||
                        string.IsNullOrEmpty(valueModel.MinValue) && string.IsNullOrEmpty(valueModel.NomValue))
                    {
                        var id = GetLastId(tableName);
                        using (var cnn = new SqlConnection(this.ConnectionString))
                        {
                            var newId = Convert.ToInt32(id) + 1;
                            var sql = $"INSERT INTO {tableName} VALUES ({newId}, " +
                                $"'{valueModel.Name}', '{valueModel.Value}', '{valueModel.MaxValue}')";
                            var command = new SqlCommand(sql, cnn);
                            command.Connection.Open();
                            command.ExecuteNonQuery();
                        }
                        break;
                    }
                    else
                    {
                        var id = GetLastId(tableName);
                        using (var cnn = new SqlConnection(this.ConnectionString))
                        {
                            var newId = Convert.ToInt32(id) + 1;
                            var sql = $"INSERT INTO {tableName} VALUES " +
                                $"({newId}, '{valueModel.Name}', " +
                                $"'{valueModel.Value}', '{valueModel.MaxValue}'" +
                                $", '{valueModel.MinValue}', '{valueModel.NomValue}')";

                            var command = new SqlCommand(sql, cnn);
                            command.Connection.Open();
                            command.ExecuteNonQuery();
                        }
                        break;
                    }
                }
            }
        }
    }
}
