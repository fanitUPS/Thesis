﻿using System;
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
        /// Строка подключения к СК-11, для запроса данных
        /// </summary>
        private const string _connectionStringToRtdb = "10.221.3.29:900";

        static void Main(string[] args)
        {
            //Гуиды созданных в системе телеизмерений
            var signalGuid1 = new Guid("65CCCE53-5914-49E7-9623-BB914962C46A");
            var signalGuid2 = new Guid("7443900b-9a2a-436d-a284-cc9400bb75dd");
            //Запрос данных осуществляется через массив гуидов
            var uidsArray = new Guid[] { signalGuid1, signalGuid2 };
            //Создаем экземпляр класса, для запроса данных
            var dataRequest = new DataRequest(_connectionStringToRtdb);

            try
            {
                //Запрос данных, ответ приходит в виде массива данных
                var dataValue = dataRequest.GetSignals(uidsArray);
                var validationValue = new Validation(dataValue);
                var validDataValue = validationValue.GetValidData();
                //Запрос данных, ответ приходит в виде массива данных
                var dataMaxValue = dataRequest.GetSignals(uidsArray);
                var validationMaxValue = new Validation(dataMaxValue);
                var validDataMaxValue = validationMaxValue.GetValidData();
                //Запрос данных, ответ приходит в виде массива данных
                var dataMinValue = dataRequest.GetSignals(uidsArray);
                var validationMinValue = new Validation(dataMinValue);
                var validDataMinValue = validationMinValue.GetValidData();
                //Парсим данные, полученные из Системы
                var voltage = new SignalVoltage
                    ("someName",
                    validDataValue[0].Value.AnalogValue,
                    validDataMaxValue[0].Value.AnalogValue,
                    validDataMinValue[0].Value.AnalogValue);

                var voltage2 = new SignalVoltage
                   ("someName",
                   validDataValue[1].Value.AnalogValue,
                   validDataMaxValue[1].Value.AnalogValue,
                   validDataMinValue[1].Value.AnalogValue);

                var power = new SignalPower
                    ("someName",
                    validDataValue[0].Value.AnalogValue,
                    validDataMaxValue[0].Value.AnalogValue);

                var current = new SignalCurrent
                    ("someName",
                    validDataValue[0].Value.AnalogValue,
                    validDataMaxValue[0].Value.AnalogValue);
                //Создадим массивы данных, для их преобразования
                var signalVoltageArr = new SignalVoltage[] { voltage, voltage2 };
                var signalPower = new SignalPower[] { power };
                var signalCurrent = new SignalCurrent[] { current };
                //Преобразуем данные для расчета
                var preparingData = new PreparingData
                    (signalPower,
                    signalVoltageArr,
                    signalCurrent);

                Console.WriteLine(nameof(preparingData.CurrentSignals));

                var currentData = preparingData.PreparingBranchData(nameof(preparingData.CurrentSignals));
                Console.WriteLine(currentData);

                var powerData = preparingData.PreparingBranchData(nameof(preparingData.PowerSignals));
                Console.WriteLine(powerData);
                var voltageData = preparingData.PreparingNodeData();

                Console.WriteLine(voltageData);

                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }

            //foreach (Ck.RtdbValue value in validSignals)
            //{
            //    Console.WriteLine("Value: " + value.Value.AnalogValue);
            //    Console.WriteLine("Type: " + value.Type);
            //    Console.WriteLine("Uid: " + value.Uid);
            //    Console.WriteLine("QualityCodes: " + Convert.ToString(value.QualityCodes, 16));
            //    Console.WriteLine("Time: " + value.Time);
            //}
            //Console.ReadKey();
        }
    }
}
