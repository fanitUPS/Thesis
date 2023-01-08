using System;
using System.IO;
using lib60870;

namespace ModelThesis
{
    /// <summary>
    /// Класс отправки данных в СК-11
    /// </summary>
    public class DataResponse
    {
        /// <summary>
        /// IP адрес расчетного сервера
        /// </summary>
        public string ServerIpAddress { get; private set; }

        /// <summary>
        /// Порт устройства МЭК 104 в СК-11(по умолчанию 2404)
        /// </summary>
        public int ServerPort { get; private set; }

        /// <summary>
        /// Адрес slave в СК-11
        /// </summary>
        public int Coa { get; private set; }

        /// <summary>
        /// Адрес ТИ в СК-11
        /// </summary>
        public int Ioa { get; private set; }

        /// <summary>
        /// Метка времени
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Значение расчета
        /// </summary>
        public double Value { get; private set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="serverAddress">IP адрес расчетного сервера</param>
        /// <param name="serverPort">Порт устройства МЭК 104 в СК-11(по умолчанию 2404)</param>
        public DataResponse(string serverAddress, int serverPort)
        {
            ServerIpAddress = serverAddress;
            ServerPort = serverPort;
        }

        public Server CreateServer()
        {
            var server = new Server();
            server.DebugOutput = false;
            server.MaxQueueSize = 10;
            server.ServerMode = ServerMode.SINGLE_REDUNDANCY_GROUP;
            server.SetLocalAddress(this.ServerIpAddress); // IP-адрес сервера
            server.SetLocalPort(this.ServerPort);
            server.Start();

            return server;
        }
        
        /// <summary>
        /// Отправка результата в виде ТИ
        /// </summary>
        public void SendIndex(Server server, PerformanceIndex index, int coa, int ioa)
        {
            var quality = new QualityDescriptor();
            var newValue = (float)Math.Round(index.Value, 5);
            var newAsdu = new ASDU(server.GetConnectionParameters(), 
                CauseOfTransmission.PERIODIC, false, false, 1, coa, false);

            var CP56 = new CP56Time2a(index.TimeStamp);
            var io = new MeasuredValueShortWithCP56Time2a
                (ioa, newValue, quality, CP56);

            newAsdu.AddInformationObject(io);
            server.EnqueueASDU(newAsdu);
        }

        public async void WriteLocalIndex(string path, Calculation result)
        {
            File.WriteAllText(path, result.CurrentIndex.ToString());
            var text = result.PowerIndex.ToString() + result.VoltagetIndex.ToString();
            using (var file = new StreamWriter(path, append: true))
            {
                await file.WriteLineAsync(text);
                file.Close();
            }
        }
    }
}
