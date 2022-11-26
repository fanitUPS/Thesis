using System;
using ModelThesis;
using System.Collections.Generic;
using lib60870;

namespace ConsoleApp
{
    /// <summary>
    /// Разрабатываемое ПО сервис для внешней Системы СК-11. Данный View создан для тестирования бизнес-логики
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Строка подключения к СК-11, для запроса данных
        /// </summary>
        private const string _connectionStringToRtdb = "10.221.3.29:900";

        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        private const string _connectionStringToDb = "data source=STS20;initial catalog=ThesisDatabase;trusted_connection=true";

        /// <summary>
        /// Адрес сервера передачи ТИ
        /// </summary>
        private const string _serverAddress = "10.221.3.9";

        /// <summary>
        /// Порт для приняти ТИ
        /// </summary>
        private const int _serverPort = 2404;

        /// <summary>
        /// Общий адрес ТИ
        /// </summary>
        private const int _coa = 77;

        /// <summary>
        /// Адрес объекта информации
        /// </summary>
        private const int _ioa = 999;

        static void Main(string[] args)
        {
            var server = new Server();
            //Создаем экземпляр класса, для запроса данных
            var dataRequest = new DataRequest(_connectionStringToRtdb);
            
            try
            {
                var starTime = DateTime.Now;
                Console.WriteLine(starTime);

                var dataBase = new DataBase(_connectionStringToDb);
                var dataCurrent = dataBase.SelectUuid(nameof(DataBaseTables.Currents));
                var dataPower = dataBase.SelectUuid(nameof(DataBaseTables.Powers));
                var dataVoltage = dataBase.SelectUuid(nameof(DataBaseTables.Voltages));

                var voltageList = new List<SignalVoltage>();
                var currentList = new List<SignalCurrent>();
                var powerList = new List<SignalPower>();

                var time = DateTime.Now;

                foreach (var value in dataVoltage)
                {
                    var tempData = new Verification(dataRequest.GetSignals(value.Value));
                    var data = tempData.GetValidData();
                    voltageList.Add(new SignalVoltage(value.Key,
                        data[0].Value.AnalogValue,
                        data[1].Value.AnalogValue,
                        data[2].Value.AnalogValue,
                        data[3].Value.AnalogValue));

                    if (data[0].Time < time)
                    {
                        time = Convert.ToDateTime(data[0].Time);
                    }
                }

                foreach (var value in dataCurrent)
                {
                    var tempData = new Verification(dataRequest.GetSignals(value.Value));
                    var data = tempData.GetValidData();
                    currentList.Add(new SignalCurrent(value.Key,
                        data[0].Value.AnalogValue,
                        data[1].Value.AnalogValue));

                    if (data[0].Time < time)
                    {
                        time = Convert.ToDateTime(data[0].Time);
                    }
                }

                foreach (var value in dataPower)
                {
                    var tempData = new Verification(dataRequest.GetSignals(value.Value));
                    var data = tempData.GetValidData();
                    powerList.Add(new SignalPower(value.Key,
                        data[0].Value.AnalogValue,
                        data[1].Value.AnalogValue));

                    if (data[0].Time < time)
                    {
                        time = Convert.ToDateTime(data[0].Time);
                    }
                }

                var preparingData = new PreparingData(
                    powerList.ToArray(),
                    voltageList.ToArray(),
                    currentList.ToArray());

                var voltages = preparingData.PreparingNodeData();
                var currents = preparingData.PreparingBranchData(nameof(preparingData.CurrentSignals));
                var powers = preparingData.PreparingBranchData(nameof(preparingData.PowerSignals));

                var calc = new Calculation(powers, currents, voltages);

                var valueIndex = calc.GetPerformanceIndex();
                Console.WriteLine(valueIndex);
                Console.WriteLine(calc.CurrentIndex);
                Console.WriteLine(calc.VoltagetIndex);
                Console.WriteLine(calc.PowerIndex);

                var index = new PerformanceIndex(0, valueIndex, time);

                var stopTime = DateTime.Now;
                Console.WriteLine((stopTime - starTime).TotalSeconds);
                Console.WriteLine(5 > (stopTime - starTime).TotalSeconds);
                Console.WriteLine(index.TimeStamp);

                var send = new DataResponse(_serverAddress, _serverPort, index, _coa, _ioa);
                send.SendPerformanceIndex();

                Console.WriteLine(dataBase.GetRateOfChange(index));
                Console.WriteLine(dataBase.GetIncrementOfIndex(index));

                dataBase.InsertData(nameof(DataBaseTables.Calculations), index);

                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }
    }
}

