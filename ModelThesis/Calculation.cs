using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PandasNet;

namespace ModelThesis
{
    public class Calculation
    {
        private DataFrame _powerSignals;

        private DataFrame _currentSignals;

        private DataFrame _voltageSignals;

        public DataFrame PowerSignals
        {
            get => _powerSignals;
            set => EmptyCheck(value);
        }

        public DataFrame CurrentSignals
        {
            get => _currentSignals;
            set => EmptyCheck(value);
        }

        public DataFrame VoltageSignals
        {
            get => _voltageSignals;
            set => EmptyCheck(value);
        }

        public Calculation(DataFrame powerSignals,
            DataFrame currentSignals, DataFrame voltageSignals)
        {
            PowerSignals = powerSignals;
            CurrentSignals = currentSignals;
            VoltageSignals = voltageSignals;
        }

        private DataFrame EmptyCheck(DataFrame data)
        {
            if (data.shape[0] == 0)
            {
                throw new ArgumentException("Пустой датафрейм недопустим.");
            }

            return data;
        }

        //private double GetPerformanceIndex()
        //{
        //    var nodeVoltage = new
        //}
    }
}
