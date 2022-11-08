﻿using System.Collections.Generic;
using System;
//using PandasNet;
using Pd = Microsoft.Data.Analysis;

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
        public Pd.DataFrame PreparingNodeData()
        {
            var valueColumn = new Pd.PrimitiveDataFrameColumn<double>
                ("ValueVoltage", this.VoltageSignals.Length);
            var maxValueColumn = new Pd.PrimitiveDataFrameColumn<double>
                ("MaxVoltage", this.VoltageSignals.Length);
            var minValueColumn = new Pd.PrimitiveDataFrameColumn<double>
                ("MinVoltage", this.VoltageSignals.Length);

            var result = new Pd.DataFrame
                (valueColumn, maxValueColumn, minValueColumn);

            for (int i = 0; i < VoltageSignals.Length; i++)
            {
                result[i, 0] = VoltageSignals[i].SignalValue;
                result[i, 1] = VoltageSignals[i].MaxVoltage;
                result[i, 2] = VoltageSignals[i].MinVoltage;
            }

            return result;
        }

        /// <summary>
        /// Преобразование данных тока и мощности
        /// </summary>
        /// <param name="name">Выбор преобразования тока или мощности</param>
        /// <returns>Датафрейм токов или мощностей</returns>
        public Pd.DataFrame PreparingBranchData(string name)
        {
            var list = new List<string>()
            {
                nameof(CurrentSignals),
                nameof(PowerSignals)
            };

            if (!list.Contains(name))
            {
                throw new ArgumentException
                    ("Параметра, переданного в PreparingBranchData не существует.");
            }

            var valueColumn = new Pd.PrimitiveDataFrameColumn<double>
                ("Value", this.VoltageSignals.Length);
            var maxValueColumn = new Pd.PrimitiveDataFrameColumn<double>
                ("MaxValue", this.VoltageSignals.Length);

            var result = new Pd.DataFrame
                (valueColumn, maxValueColumn);

            switch (name)
            {
                case nameof(CurrentSignals):
                    for (int i = 0; i < CurrentSignals.Length; i++)
                    {
                        result[i, 0] = CurrentSignals[i].SignalValue;
                        result[i, 1] = CurrentSignals[i].MaxCurrent;
                    }
                    break;
                case nameof(PowerSignals):
                    for (int i = 0; i < PowerSignals.Length; i++)
                    {
                        result[i, 0] = PowerSignals[i].SignalValue;
                        result[i, 1] = PowerSignals[i].MaxPower;
                    }
                    break;
            };

            return result;
        }

            ///// <summary>
            ///// Преобразование данных тока и мощности
            ///// </summary>
            ///// <returns>Датафрейм токов</returns>
            //public DataFrame PreparingBranchData(string name)
            //{
            //    var valueList = new List<double>();
            //    var maxValueList = new List<double>();

            //    switch (name)
            //    {
            //        case nameof(CurrentSignals):
            //            foreach (var signal in CurrentSignals)
            //            {
            //                valueList.Add(signal.SignalValue);
            //                maxValueList.Add(signal.MaxCurrent);
            //            }
            //            break;
            //        case nameof(PowerSignals):
            //            foreach (var signal in PowerSignals)
            //            {
            //                valueList.Add(signal.SignalValue);
            //                maxValueList.Add(signal.MaxPower);
            //            }
            //            break;
            //    };

            //    var seriesValue = new Series(valueList.ToArray());
            //    var seriesMaxValue = new Series(maxValueList.ToArray());

            //    var columnValue = new Column();
            //    columnValue.Name = "Value";
            //    var columnMaxValue = new Column();
            //    columnMaxValue.Name = "MaxValue";

            //    var columnName = new List<Column>()
            //    { columnValue,
            //      columnMaxValue
            //    };

            //    return new DataFrame(new List<Series>()
            //        { seriesValue, seriesMaxValue}, null, columnName);
            //}
        }
}
