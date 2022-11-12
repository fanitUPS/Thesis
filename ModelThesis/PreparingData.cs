using System.Collections.Generic;
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
            var nomValueColumn = new Pd.PrimitiveDataFrameColumn<double>
                ("NomVoltage", this.VoltageSignals.Length);
            var nameColumn = new Pd.StringDataFrameColumn
                ("Name", this.VoltageSignals.Length);

            var result = new Pd.DataFrame
                (valueColumn, maxValueColumn, minValueColumn, nomValueColumn, nameColumn);

            for (int i = 0; i < VoltageSignals.Length; i++)
            {
                result[i, 0] = VoltageSignals[i].SignalValue;
                result[i, 1] = VoltageSignals[i].MaxVoltage;
                result[i, 2] = VoltageSignals[i].MinVoltage;
                result[i, 3] = VoltageSignals[i].NomVoltage;
                result[i, 4] = VoltageSignals[i].SignalName;
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

            var result = new Pd.DataFrame();

            switch (name)
            {
                case nameof(CurrentSignals):
                    var valueColumn = new Pd.PrimitiveDataFrameColumn<double>
                        ("Value", this.CurrentSignals.Length);
                    var maxValueColumn = new Pd.PrimitiveDataFrameColumn<double>
                        ("MaxValue", this.CurrentSignals.Length);
                    var nameColumn = new Pd.StringDataFrameColumn
                        ("Name", this.CurrentSignals.Length);

                    result = new Pd.DataFrame
                        (valueColumn, maxValueColumn, nameColumn);

                    for (int i = 0; i < CurrentSignals.Length; i++)
                    {
                        result[i, 0] = CurrentSignals[i].SignalValue;
                        result[i, 1] = CurrentSignals[i].MaxCurrent;
                        result[i, 2] = CurrentSignals[i].SignalName;
                    }
                    break;

                case nameof(PowerSignals):
                    var valueColumnP = new Pd.PrimitiveDataFrameColumn<double>
                        ("Value", this.PowerSignals.Length);
                    var maxValueColumnP = new Pd.PrimitiveDataFrameColumn<double>
                        ("MaxValue", this.PowerSignals.Length);
                    var nameColumnP = new Pd.StringDataFrameColumn
                        ("Name", this.PowerSignals.Length);

                    result = new Pd.DataFrame
                        (valueColumnP, maxValueColumnP, nameColumnP);

                    for (int i = 0; i < PowerSignals.Length; i++)
                    {
                        result[i, 0] = PowerSignals[i].SignalValue;
                        result[i, 1] = PowerSignals[i].MaxPower;
                        result[i, 2] = PowerSignals[i].SignalName;
                    }
                    break;
            };

            return result;
        }
    }
}
