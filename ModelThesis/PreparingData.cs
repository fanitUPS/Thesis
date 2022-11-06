using System.Collections.Generic;
using PandasNet;

namespace ModelThesis
{
    /// <summary>
    /// Класс подготовки данных к расчету
    /// </summary>
    public class PreparingData
    {
        /// <summary>
        /// Массив ТИ активной мощности
        /// </summary>
        public SignalPower[] PowerSignals { get; set; }

        /// <summary>
        /// Массив ТИ тока
        /// </summary>
        public SignalCurrent[] CurrentSignals { get; set; }

        /// <summary>
        /// Массив ТИ напряжения
        /// </summary>
        public SignalVoltage[] VoltageSignals { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="signalPower">Массив ТИ активной мощности</param>
        /// <param name="signalVoltage">Массив ТИ напряжения</param>
        /// <param name="signalCurrent">Массив ТИ тока</param>
        public PreparingData(SignalPower[] signalPower,
            SignalVoltage[] signalVoltage, SignalCurrent[] signalCurrent)
        {
            PowerSignals = signalPower;
            VoltageSignals = signalVoltage;
            CurrentSignals = signalCurrent;
        }

        /// <summary>
        /// Преобразование данных напряжения
        /// </summary>
        /// <returns>Датафрейм напряжений</returns>
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

            var columnValue = new Column();
            columnValue.Name = "Value";
            var columnMaxValue = new Column();
            columnMaxValue.Name = "MaxValue";
            var columnMinValue = new Column();
            columnMinValue.Name = "MinValue";

            var columnName = new List<Column>()
            { columnValue,
              columnMaxValue,
              columnMinValue
            };
            
            return new DataFrame(new List<Series>() 
                { seriesValue, seriesMaxValue, seriesMinValue }, null, columnName);
        }

        /// <summary>
        /// Преобразование данных тока и мощности
        /// </summary>
        /// <returns>Датафрейм токов</returns>
        public DataFrame PreparingBranchData(string name)
        {
            var valueList = new List<double>();
            var maxValueList = new List<double>();

            switch (name)
            {
                case nameof(CurrentSignals):
                    foreach (var signal in CurrentSignals)
                    {
                        valueList.Add(signal.SignalValue);
                        maxValueList.Add(signal.MaxCurrent);
                    }
                    break;
                case nameof(PowerSignals):
                    foreach (var signal in PowerSignals)
                    {
                        valueList.Add(signal.SignalValue);
                        maxValueList.Add(signal.MaxPower);
                    }
                    break;
            };

            var seriesValue = new Series(valueList.ToArray());
            var seriesMaxValue = new Series(maxValueList.ToArray());

            var columnValue = new Column();
            columnValue.Name = "Value";
            var columnMaxValue = new Column();
            columnMaxValue.Name = "MaxValue";

            var columnName = new List<Column>()
            { columnValue,
              columnMaxValue
            };

            return new DataFrame(new List<Series>()
                { seriesValue, seriesMaxValue}, null, columnName);
        }
    }
}
