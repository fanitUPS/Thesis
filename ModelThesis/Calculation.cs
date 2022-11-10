using System;
using System.Collections.Generic;
using Pd = Microsoft.Data.Analysis;

namespace ModelThesis
{
    public class Calculation
    {
        private Pd.DataFrame _powerSignals;

        private Pd.DataFrame _currentSignals;

        private Pd.DataFrame _voltageSignals;

        public Pd.DataFrame PowerSignals
        {
            get => _powerSignals;
            set => _powerSignals = EmptyCheck(value);
        }

        public Pd.DataFrame CurrentSignals
        {
            get => _currentSignals;
            set => _currentSignals = EmptyCheck(value);
        }

        public Pd.DataFrame VoltageSignals
        {
            get => _voltageSignals;
            set => _voltageSignals = EmptyCheck(value);
        }

        public Calculation(Pd.DataFrame powerSignals,
            Pd.DataFrame currentSignals, Pd.DataFrame voltageSignals)
        {
            PowerSignals = powerSignals;
            CurrentSignals = currentSignals;
            VoltageSignals = voltageSignals;
        }

        private Pd.DataFrame EmptyCheck(Pd.DataFrame data)
        {
            if (data.Rows.Count == 0)
            {
                throw new ArgumentException("Пустой датафрейм недопустим.");
            }

            return data;
        }

        public Pd.DataFrame GetPerformanceIndex()
        {
            var fuClm = new Pd.PrimitiveDataFrameColumn<double>("Fu");
            var flClm = new Pd.PrimitiveDataFrameColumn<double>("Fl");
            var duClm = new Pd.PrimitiveDataFrameColumn<double>("Du");
            var dlClm = new Pd.PrimitiveDataFrameColumn<double>("Dl");
            var guClm = new Pd.PrimitiveDataFrameColumn<double>("Gu");
            var glClm = new Pd.PrimitiveDataFrameColumn<double>("Gl");
            var upperClm = new Pd.PrimitiveDataFrameColumn<double>("upper");
            var lowerClm = new Pd.PrimitiveDataFrameColumn<double>("lower");

            var tempDataframe = new Pd.DataFrame
                (fuClm, flClm, duClm, dlClm, guClm, glClm, upperClm, lowerClm);

            var result = _voltageSignals.Join(tempDataframe);

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var vu = double.Parse(result["MaxVoltage"][i].ToString());
                var vl = double.Parse(result["MinVoltage"][i].ToString());
                var vras = double.Parse(result["ValueVoltage"][i].ToString());
                var uhom = double.Parse(result["NomVoltage"][i].ToString());

                result[i, 4] = vu * 0.95d;
                result[i, 5] = vl * 1.05d;

                var fu = double.Parse(result["Fu"][i].ToString());
                var fl = double.Parse(result["Fl"][i].ToString());

                var du = 0d;
                if (vras > fu)
                {
                    du = (vras - fu) / uhom;
                }

                var dl = 0d;
                if (vras < fl)
                {
                    dl = (-vras + fl) / uhom;
                }

                result[i, 6] = du;
                result[i, 7] = dl;

                var gu = (vu - fu) / uhom;
                var gl = (fl - vl) / uhom;

                result[i, 8] = gu;
                result[i, 9] = gl;

                var upper = Math.Pow(du / gu, 4d);
                var lower = Math.Pow(dl/gl, 4d);
                result[i, 10] = Math.Round(upper, 5);
                result[i, 11] = Math.Round(lower, 5);
            }



            return result;

        }
    }
}
