using System;
using ModelThesis;
using System.Collections.Generic;

namespace ConsoleApp
{
    /// <summary>
    /// Разрабатываемое ПО сервис для внешней Системы СК-11. Данный View создан для тестирования бизнес-логики
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Строка подключения к БДРВ, для запроса данных
        /// </summary>
        private const string _connectionStringToRtdb = "10.221.3.29:900";

        /// <summary>
        /// Строка подключения к СК-11
        /// </summary>
        private const string _connectionStringToCK11 = "10.221.3.29";

        /// <summary>
        /// Имя БД, откуда запрашивается информация
        /// </summary>
        private const string _nameOfModel = "ODB_EnergyMain";

        /// <summary>
        /// Номер ИМ
        /// </summary>
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

        private const string _pathLocalResult = 
            "C:\\Users\\stdAdmin.PL01N\\Desktop\\resultCalc\\LocalResult.txt";

        static void Main(string[] args)
        {
            //Создаем экземпляр класса, для запроса данных
            var dataRequest = new DataRequest(_connectionStringToRtdb);

            var send = new DataResponse(_serverAddress, _serverPort);
            var server = send.CreateServer();
            var dataBase = new DataBase(_connectionStringToDb);
            var time = DateTime.Now;
            try
            {
                string[] config = System.IO.File.ReadAllLines
                    (@"C:\Users\stdAdmin.PL01N\Desktop\config\config.txt");

                for (int i = 0; i < config.Length; i++)
                {
                    int index = config[i].IndexOf("=");
                    var x = "";
                    if (index >= 0)
                    {
                        x = config[i].Substring(index + 1);
                    }
                    else
                    {
                        throw new ArgumentException
                            ($"Ошибка в {i + 1} строке конфигурцационного файла.");
                    }

                    switch (i)
                    {
                        case 0:
                            var connectionStringToRtdb = x;
                            break;
                        case 1:
                            var connectionStringToCK11 = x;
                            break;
                        case 2:
                            var nameOfModel = x;
                            break;
                        case 3:
                            var numberOfModel = int.Parse(x);
                            break;
                        case 4:
                            var connectionStringToDb = x;
                            break;
                        case 5:
                            var serverAddress = x;
                            break;
                        case 6:
                            var serverPort = int.Parse(x);
                            break;
                        case 7:
                            var coa = int.Parse(x);
                            break;
                        case 8:
                            var ioa = int.Parse(x);
                            break;
                        case 9:
                            var pathLocalResult = x;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.TargetSite);
                dataBase.InsertLog(e.Message, time);
                Console.ReadKey();
            }

            while (true)
            {
                try
                {
                    Console.WriteLine("Начало работы.");


                    var model = new SynchronizeModel(_connectionStringToCK11, _nameOfModel, 270);

                    var uuidBranchFolder = "2F9E9FFE-755E-4BA7-85A6-0BE43626FE8A";
                    var uuidSubstations = "AFEBCA7A-7607-4B5F-BED7-0FD5F387CB94";
                    var uuidLines = "36C6AC72-18DE-4C99-8682-5BDB7E6F1668";

                    //var powerUuids = model.UpdatePowerUuid(uuidBranchFolder);
                    //var voltageUuids = model.UpdateVoltageUuid(uuidSubstations);
                    //var currentUuids = model.UpdateCurrentUuid(uuidLines);
                    //Console.WriteLine("Синхронизация с ИМ.");
                    //dataBase.InsertUuids(powerUuids, nameof(DataBaseTables.Powers));
                    //dataBase.InsertUuids(voltageUuids, nameof(DataBaseTables.Voltages));
                    //dataBase.InsertUuids(currentUuids, nameof(DataBaseTables.Currents));

                    var starTime = DateTime.Now;
                    Console.WriteLine("Запрос uuid из БД Системы");
                    var dataCurrent = dataBase.SelectUuid(nameof(DataBaseTables.Currents));
                    var dataPower = dataBase.SelectUuid(nameof(DataBaseTables.Powers));
                    var dataVoltage = dataBase.SelectUuid(nameof(DataBaseTables.Voltages));

                    var voltageList = new List<SignalVoltage>();
                    var currentList = new List<SignalCurrent>();
                    var powerList = new List<SignalPower>();

                    time = DateTime.Now;
                    Console.WriteLine("Запрос данных из БДРВ");
                    foreach (var value in dataVoltage)
                    {

                        var tempData = new Verification(dataRequest.GetSignals(value.Value));

                        var data = tempData.GetValidData();
                        var nomVoltage = 0d;

                        if (data[1].Value.AnalogValue > 500d)
                        {
                            nomVoltage = 500d;
                        }
                        else
                        {
                            nomVoltage = 220d;
                        }

                        voltageList.Add(new SignalVoltage(value.Key,
                            data[0].Value.AnalogValue,
                           (DateTime)data[0].Time,
                            data[1].Value.AnalogValue,
                            data[2].Value.AnalogValue,
                            nomVoltage));
                    }

                    Console.WriteLine("Фильтрация данных по коду качества.");
                    foreach (var value in dataCurrent)
                    {
                        var tempData = new Verification(dataRequest.GetSignals(value.Value));
                        var data = tempData.GetValidData();
                        currentList.Add(new SignalCurrent(value.Key,
                            data[0].Value.AnalogValue,
                            (DateTime)data[0].Time,
                            data[1].Value.AnalogValue));
                    }

                    foreach (var value in dataPower)
                    {
                        var tempData = new Verification(dataRequest.GetSignals(value.Value));
                        var data = tempData.GetValidData();
                        powerList.Add(new SignalPower(value.Key,
                            data[0].Value.AnalogValue,
                            (DateTime)data[0].Time,
                            data[1].Value.AnalogValue));
                    }

                    Console.WriteLine("Подготовка данных к расчету.");
                    var preparingData = new PreparingData(
                        powerList.ToArray(),
                        voltageList.ToArray(),
                        currentList.ToArray());

                    var voltages = preparingData.PreparingNodeData();
                    var currents = preparingData.PreparingBranchData(nameof(preparingData.CurrentSignals));
                    var powers = preparingData.PreparingBranchData(nameof(preparingData.PowerSignals));

                    var calc = new Calculation(powers, currents, voltages);

                    Console.WriteLine("Начало расчета");
                    var index = calc.GetPerformanceIndex();
                    Console.WriteLine("Результаты расчета расчета");
                    Console.WriteLine(index.Value);

                    calc.CurrentIndex.Columns.Remove("Time");
                    calc.VoltagetIndex.Columns.Remove("Time");
                    calc.PowerIndex.Columns.Remove("Time");
                    Console.WriteLine(calc.CurrentIndex.ToString());
                    Console.WriteLine(calc.VoltagetIndex);
                    Console.WriteLine(calc.PowerIndex);

                    var stopTime = DateTime.Now;
                    Console.WriteLine("Время расчета:");
                    Console.WriteLine((stopTime - starTime).TotalSeconds);


                    Console.WriteLine("Передача результата в СК-11");

                    send.SendIndex(server, index, _coa, _ioa, true);

                    var rateOfChange = new PerformanceIndex(0, dataBase.GetRateOfChange(index), time);
                    var increment = new PerformanceIndex(2, dataBase.GetIncrementOfIndex(index), time);

                    send.SendIndex(server, rateOfChange, _coa, 1001, true);
                    send.SendIndex(server, increment, _coa, 1000, true);

                    Console.WriteLine("Cкорость приращения:" + rateOfChange.Value);
                    Console.WriteLine("Приращение: " + increment.Value);
                    //throw new ArgumentException("test");
                    send.WriteLocalIndex(_pathLocalResult, calc);

                    //dataBase.InsertPerformanceIndex(nameof(DataBaseTables.Calculations), index);

                    Console.WriteLine("Finish");
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.TargetSite);
                    dataBase.InsertLog(e.Message, time);
                    send.SendIndex(server, new PerformanceIndex(0, 0, time), _coa, _ioa, false);
                    Console.WriteLine("finish with error");
                    Console.ReadKey();
                }
            }
        }
    }
}

