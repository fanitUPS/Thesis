using System;
using lib60870;


namespace ModelResponse
{
    public class DataResponse
    {
        public string ServerIpAddress { get; set; }

        public int ServerPort { get; set; }

        public int Coa { get; set; }

        public int Ioa { get; set; }

        public DateTime DateTime { get; set; }

        public double Value { get; set; }

        public DataResponse(string serverAddress, int serverPort, DateTime time, double value, int coa, int ioa)
        {
            ServerIpAddress = serverAddress;
            ServerPort = serverPort;
            DateTime = time;
            Value = value;
            Coa = coa;
            Ioa = ioa;
        }

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
            var newValue = (float)this.Value;
            var newAsdu = new ASDU(server.GetConnectionParameters(), CauseOfTransmission.PERIODIC, false, false, 1, this.Coa, false);
            var CP56 = new CP56Time2a(this.DateTime);
            var io = new MeasuredValueShortWithCP56Time2a(this.Ioa, newValue, quality, CP56);
            newAsdu.AddInformationObject(io);
            server.EnqueueASDU(newAsdu);
        }
    }
}
