using System;
using System.Text.RegularExpressions;
using Ck = Monitel.Rtdb.Api;

namespace ModelThesis
{
    /// <summary>
    /// Класс для запроса данных из БДРВ СК-11
    /// </summary>
    public class DataRequest
    {
        /// <summary>
        /// IP адрес сервиса БДРВ
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// IP адрес сервиса БДРВ
        /// </summary>
        public string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = CheckConnectionString(value);
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="connectionString">IP адрес сервиса БДРВ</param>
        public DataRequest(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Проверка введеного IP адреса
        /// </summary>
        /// <param name="connectionString">IP адрес</param>
        /// <returns>Корректный IP адрес</returns>
        private string CheckConnectionString(string connectionString)
        {
            var pattern = @"([0-9]{1,3}[\.]){3}[0-9]{1,3}:[0-9]{1,5}";

            if (!Regex.IsMatch(connectionString, pattern))
            {
                throw new ArgumentException
                        ("Неправильный адрес сервера СК-11.");
            }

            return connectionString;
        }

        /// <summary>
        /// Запрос данных из БДРВ
        /// </summary>
        /// <param name="uidsArray">Массив uids нужных ТИ</param>
        /// <returns>Массив ТИ</returns>
        public Ck.RtdbValue[] GetSignals(Guid[] uidsArray)
        {
            using (Ck.IRtdbProvider provider = Ck.RtdbProvider.CreateProvider())
            {
                Ck.IRtdbProxy proxy;
                proxy = provider.Connect(this.ConnectionString);

                var request =
                    new Ck.Requests.ValuesSliceReadRequest(uidsArray, null);
    
                using (var tracker = proxy.SendRequest(request))
                {
                    var response = tracker.WaitResponse();
                    return response.Values;
                }
            }
        }
    }
}
