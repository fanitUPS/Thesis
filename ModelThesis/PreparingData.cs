using System;
using System.Collections.Generic;
using ModelThesis;
using PandasNet;

namespace ModelThesis
{
    public class PreparingData
    {
        public SignalPower[] PowerSignals { get; set; }

        public SignalCurrent[] CurrentSignals { get; set; }

        public SignalVoltage[] VoltageSignals { get; set; }

        public PreparingData(SignalPower[] signalPower,
            SignalVoltage[] signalVoltage, SignalCurrent[] signalCurrent)
        {
            PowerSignals = signalPower;
            VoltageSignals = signalVoltage;
            CurrentSignals = signalCurrent;
        }

        public DataFrame PreparingNodeData()
        {
            var valueList = new List<double>();
            var maxValueList = new List<double>();
            var minValueList = new List<double>();

            foreach (var signal in VoltageSignals)
            {
                valueList.Add(signal.SignalValue);
                maxValueList.Add(signal.MaxVoltage);
                minValueList.Add(signal.MinVoltage);
            }

            var seriesValue = new Series(valueList.ToArray());
            var seriesMaxValue = new Series(maxValueList.ToArray());
            var seriesMinValue = new Series(minValueList.ToArray());

            return new DataFrame(new List<Series>() 
                { seriesValue, seriesMaxValue, seriesMinValue });
        }

        public DataFrame PreparingBranchCurrentData()
        {
            var valueList = new List<double>();
            var maxValueList = new List<double>();

            foreach (var signal in CurrentSignals)
            {
                valueList.Add(signal.SignalValue);
                maxValueList.Add(signal.MaxCurrent);
            }

            var seriesValue = new Series(valueList.ToArray());
            var seriesMaxValue = new Series(maxValueList.ToArray());

            return new DataFrame(new List<Series>()
                { seriesValue, seriesMaxValue});
        }

        public DataFrame PreparingBranchPowerData()
        {
            var valueList = new List<double>();
            var maxValueList = new List<double>();

            foreach (var signal in PowerSignals)
            {
                valueList.Add(signal.SignalValue);
                maxValueList.Add(signal.MaxPower);
            }

            var seriesValue = new Series(valueList.ToArray());
            var seriesMaxValue = new Series(maxValueList.ToArray());

            return new DataFrame(new List<Series>()
                { seriesValue, seriesMaxValue});
        }
    }
}
