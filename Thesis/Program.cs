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

        private const string _connectionStringToDb = "data source=STS20;initial catalog=ThesisDatabase;trusted_connection=true";

        private const string _serverAddress = "10.221.3.9";

        private const int _serverPort = 2404;

        private const int _coa = 77;

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
                var dataCurrent = dataBase.SelectData(nameof(DataBaseTables.Currents));
                var dataPower = dataBase.SelectData(nameof(DataBaseTables.Powers));
                var dataVoltage = dataBase.SelectData(nameof(DataBaseTables.Voltages));
                
                var voltageList = new List<SignalVoltage>();
                var currentList = new List<SignalCurrent>();
                var powerList = new List<SignalPower>();

                var time = DateTime.Now;

                foreach(var value in dataVoltage)
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

                
                var stopTime = DateTime.Now;
                Console.WriteLine((stopTime - starTime).TotalSeconds);
                Console.WriteLine(5 > (stopTime - starTime).TotalSeconds);
                Console.WriteLine(time);

                var send = new ModelResponse.DataResponse(_serverAddress, _serverPort, time, valueIndex, _coa, _ioa);
                send.SendPerformanceIndex();
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

