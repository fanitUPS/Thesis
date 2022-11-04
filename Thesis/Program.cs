using System;
using Ck = Monitel.Rtdb.Api;
using ModelThesis;
using System.Collections.Generic;


namespace ConsoleApp
{
    internal class Program
    {
        private const string _connectionStringToRtdb = "10.221.3.29:900";

        static void Main(string[] args)
        {
            var signalGuid1 = new Guid("65CCCE53-5914-49E7-9623-BB914962C46A");
            var signalGuid2 = new Guid("7443900b-9a2a-436d-a284-cc9400bb75dd");
            
            var uidsArray = new Guid[] { signalGuid1};

            var dataRequest = new DataRequest(_connectionStringToRtdb);

            try
            {
                var dataValue = dataRequest.GetSignals(uidsArray);
                var validationValue = new Validation(dataValue);
                var validDataValue = validationValue.GetValidData();

                var dataMaxValue = dataRequest.GetSignals(uidsArray);
                var validationMaxValue = new Validation(dataMaxValue);
                var validDataMaxValue = validationMaxValue.GetValidData();

                var dataMinValue = dataRequest.GetSignals(uidsArray);
                var validationMinValue = new Validation(dataMinValue);
                var validDataMinValue = validationMinValue.GetValidData();

                var voltage = new SignalVoltage
                    ("someName",
                    validDataValue[0].Value.AnalogValue,
                    validDataMaxValue[0].Value.AnalogValue,
                    validDataMinValue[0].Value.AnalogValue);

                var power = new SignalPower
                    ("someName",
                    validDataValue[0].Value.AnalogValue,
                    validDataMaxValue[0].Value.AnalogValue);

                var current = new SignalCurrent
                    ("someName",
                    validDataValue[0].Value.AnalogValue,
                    validDataMaxValue[0].Value.AnalogValue);

                var signalVoltageArr = new SignalVoltage[] { voltage };
                var signalPower = new SignalPower[] { power };
                var signalCurrent = new SignalCurrent[] { current };

                var preparingData = new PreparingData
                    (signalPower,
                    signalVoltageArr,
                    signalCurrent);

                var voltageData = preparingData.PreparingNodeData();

                Console.WriteLine(voltageData[0]);
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

