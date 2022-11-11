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

        public Pd.DataFrame GetVoltageIndex()
        {
            var upperClm = new Pd.PrimitiveDataFrameColumn<double>("upper");
            var lowerClm = new Pd.PrimitiveDataFrameColumn<double>("lower");

            var tempDataframe = new Pd.DataFrame
                (upperClm, lowerClm);

            var result = _voltageSignals.Join(tempDataframe);

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var vu = Convert.ToDouble(result["MaxVoltage"][i]);
                var vl = Convert.ToDouble(result["MinVoltage"][i]);
                var vras = Convert.ToDouble(result["ValueVoltage"][i]);
                var uhom = Convert.ToDouble(result["NomVoltage"][i]);

                if (uhom == 0)
                {
                    throw new ArgumentException
                        ("Номинальное напряжение равно 0.");
                }

                var fu = vu * 0.95d;
                var fl = vl * 1.05d;

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

                var gu = (vu - fu) / uhom;
                var gl = (fl - vl) / uhom;

                if (gu == 0 || gl == 0)
                {
                    throw new ArgumentException
                        ("Коэффициенты Gu или Gl равны 0.");
                }

                var upper = Math.Pow(du / gu, 4d);
                var lower = Math.Pow(dl / gl, 4d);
                result[i, 4] = Math.Round(upper, 5);
                result[i, 5] = Math.Round(lower, 5);
            }
            return result;
        }

        public Pd.DataFrame GetPowerIndex()
        {
            var powerCalcClm = new Pd.PrimitiveDataFrameColumn<double>("powerCalc");

            var tempDataframe = new Pd.DataFrame
                (powerCalcClm);

            var result = _powerSignals.Join(tempDataframe);

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var p = Math.Abs(Convert.ToDouble(result["Value"][i]));
                var mpf = Convert.ToDouble(result["MaxValue"][i]);
                const double baseP = 1000d;
                var preLim = mpf * 0.5;

                var dp = 0d;
                if (p > preLim)
                {
                    dp = (p - preLim) / baseP;
                }

                var gp = (mpf - preLim) / baseP;

                var powerCalc = Math.Pow(dp / gp, 4d);
                result[i, 2] = Math.Round(powerCalc, 5);

            }
            return result;
        }

        public Pd.DataFrame GetCurrentIndex()
        {
            var currentCalcClm = new Pd.PrimitiveDataFrameColumn<double>("currentCalc");

            var tempDataframe = new Pd.DataFrame
                (currentCalcClm);

            var result = _currentSignals.Join(tempDataframe);

            for (int i = 0; i < result.Rows.Count; i++)
            {
                var curr = Math.Abs(Convert.ToDouble(result["Value"][i]));
                var max_curr = Convert.ToDouble(result["MaxValue"][i]);
                const double baseI = 1000d;
                var preLim = max_curr * 0.5;

                var di = 0d;
                if (curr > preLim)
                {
                    di = (curr - preLim) / baseI;
                }

                var gi = (max_curr - preLim) / baseI;

                var currentCalc = Math.Pow(di / gi, 4d);
                result[i, 2] = Math.Round(currentCalc, 5);

            }
            return result;
        }

        public double GetPerformanceIndex()
        {
            var voltage = this.GetVoltageIndex();
            var power = this.GetPowerIndex();
            var current = this.GetCurrentIndex();

            var calcUpper = Convert.ToDouble(voltage["upper"].Sum());
            var calcLower = Convert.ToDouble(voltage["lower"].Sum());
            var calcPower = Convert.ToDouble(power["powerCalc"].Sum());
            var calcCurrent = Convert.ToDouble(current["currentCalc"].Sum());
            var result = calcUpper + calcLower + calcPower + calcCurrent;

            return Math.Round(Math.Pow(result, 0.25d), 5);
        }
    }
}
