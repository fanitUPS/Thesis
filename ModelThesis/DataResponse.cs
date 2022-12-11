using System;
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
        /// <param name="index">Показатель тяжести</param>
        /// <param name="coa">Адрес slave в СК-11</param>
        /// <param name="ioa">Адрес ТИ в СК-11</param>
        public DataResponse(string serverAddress, 
            int serverPort, PerformanceIndex index, int coa, int ioa)
        {
            ServerIpAddress = serverAddress;
            ServerPort = serverPort;
            DateTime = index.TimeStamp;
            Value = index.Value;
            Coa = coa;
            Ioa = ioa;
        }

        /// <summary>
        /// Отправка результата в виде ТИ
        /// </summary>
        public void SendPerformanceIndex()
        {
            var server = new Server();
            server.DebugOutput = false;
            server.MaxQueueSize = 10;
            server.ServerMode = ServerMode.SINGLE_REDUNDANCY_GROUP;
            server.SetLocalAddress(this.ServerIpAddress); // IP-адрес сервера
            server.SetLocalPort(this.ServerPort);
            server.Start();

            var quality = new QualityDescriptor();
            var newValue = (float)Math.Round(this.Value, 5);
            var newAsdu = new ASDU(server.GetConnectionParameters(), 
                CauseOfTransmission.PERIODIC, false, false, 1, this.Coa, false);

            var CP56 = new CP56Time2a(this.DateTime);
            var io = new MeasuredValueShortWithCP56Time2a
                (this.Ioa, newValue, quality, CP56);

            newAsdu.AddInformationObject(io);
            server.EnqueueASDU(newAsdu);
        }
    }
}
