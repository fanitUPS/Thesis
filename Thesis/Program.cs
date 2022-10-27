using System;
using Ck = Monitel.Rtdb.Api;
using ModelThesis;

namespace ConsoleApp
{
    internal class Program
    {
        private const string _connectionStringToRtdb = "10.221.3.29:900";

        static void Main(string[] args)
        {
            var signalGuid1 = new Guid("65CCCE53-5914-49E7-9623-BB914962C46A");
            var signalGuid2 = new Guid("7443900b-9a2a-436d-a284-cc9400bb75dd");
            //good 10000002
            var uidsArray = new Guid[] { signalGuid1, signalGuid2 };

            var signalsData = new DataRequest(_connectionStringToRtdb);

            try
            {
                var rtdbValues = signalsData.GetSignals(uidsArray);
                var validationSignal = new Validation(rtdbValues);
                var validSignals = validationSignal.GetValidData();

                foreach (Ck.RtdbValue value in validSignals)
                {
                    Console.WriteLine("Value: " + value.Value.AnalogValue);
                    Console.WriteLine("Type: " + value.Type);
                    Console.WriteLine("Uid: " + value.Uid);
                    Console.WriteLine("QualityCodes: " + Convert.ToString(value.QualityCodes, 16));
                    Console.WriteLine("Time: " + value.Time);
                }
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

