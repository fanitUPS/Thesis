using System;
using ModelThesis;
using System.Collections.Generic;
using lib60870;
using MalProv = Monitel.Mal.Providers;
using Mal = Monitel.Mal;
using MalProvMal = Monitel.Mal.Providers.Mal;

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

        private const string _connectionStringToCK11 = "10.221.3.29";

        private const string _nameOfModel = "ODB_EnergyMain";

        private const int _numberOfModel = 30;

        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        private const string _connectionStringToDb = 
            "data source=STS20;initial catalog=ThesisDatabase;trusted_connection=true";

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
                //var starTime = DateTime.Now;
                //Console.WriteLine(starTime);

                var dataBase = new DataBase(_connectionStringToDb);
                //var dataCurrent = dataBase.SelectUuid(nameof(DataBaseTables.Currents));
                //var dataPower = dataBase.SelectUuid(nameof(DataBaseTables.Powers));
                //var dataVoltage = dataBase.SelectUuid(nameof(DataBaseTables.Voltages));

                //var voltageList = new List<SignalVoltage>();
                //var currentList = new List<SignalCurrent>();
                //var powerList = new List<SignalPower>();

                //var time = DateTime.Now;

                //foreach (var value in dataVoltage)
                //{
                //    var tempData = new Verification(dataRequest.GetSignals(value.Value));
                //    var data = tempData.GetValidData();
                //    voltageList.Add(new SignalVoltage(value.Key,
                //        data[0].Value.AnalogValue,
                //        (DateTime)data[0].Time,
                //        data[1].Value.AnalogValue,
                //        data[2].Value.AnalogValue,
                //        data[3].Value.AnalogValue));
                //}

                //foreach (var value in dataCurrent)
                //{
                //    var tempData = new Verification(dataRequest.GetSignals(value.Value));
                //    var data = tempData.GetValidData();
                //    currentList.Add(new SignalCurrent(value.Key,
                //        data[0].Value.AnalogValue,
                //        (DateTime)data[0].Time,
                //        data[1].Value.AnalogValue));
                //}

                //foreach (var value in dataPower)
                //{
                //    var tempData = new Verification(dataRequest.GetSignals(value.Value));
                //    var data = tempData.GetValidData();
                //    powerList.Add(new SignalPower(value.Key,
                //        data[0].Value.AnalogValue,
                //        (DateTime)data[0].Time,
                //        data[1].Value.AnalogValue));
                //}

                //var preparingData = new PreparingData(
                //    powerList.ToArray(),
                //    voltageList.ToArray(),
                //    currentList.ToArray());

                //var voltages = preparingData.PreparingNodeData();
                //var currents = preparingData.PreparingBranchData(nameof(preparingData.CurrentSignals));
                //var powers = preparingData.PreparingBranchData(nameof(preparingData.PowerSignals));

                //var calc = new Calculation(powers, currents, voltages);


                //var index = calc.GetPerformanceIndex();
                //Console.WriteLine(index.Value);
                //Console.WriteLine(calc.CurrentIndex);
                //Console.WriteLine(calc.VoltagetIndex);
                //Console.WriteLine(calc.PowerIndex);

                //var stopTime = DateTime.Now;
                //Console.WriteLine((stopTime - starTime).TotalSeconds);
                //Console.WriteLine(5 > (stopTime - starTime).TotalSeconds);
                //Console.WriteLine("time of index: "+index.TimeStamp);

                //var send = new DataResponse(_serverAddress, _serverPort, index, _coa, _ioa);
                //send.SendPerformanceIndex();

                //Console.WriteLine("скорость приращения:" + dataBase.GetRateOfChange(index));
                //Console.WriteLine("Приращение: "+dataBase.GetIncrementOfIndex(index));

                //dataBase.InsertData(nameof(DataBaseTables.Calculations), index);
  
                var uuidBranchFolder = "2F9E9FFE-755E-4BA7-85A6-0BE43626FE8A";

                var model = new SynchronizeModel(_connectionStringToCK11, _nameOfModel, 55);
                
                var powerUuids = model.UpdatePowerUuid(uuidBranchFolder);

                dataBase.InsertUuids(powerUuids, nameof(DataBaseTables.Powers));
                
                Console.WriteLine("Finish");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.TargetSite);
                Console.ReadKey();
            }
        }
    }
}

