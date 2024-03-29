﻿using System;

namespace ModelThesis
{
    /// <summary>
    /// Класс хранения ТИ напряжения
    /// </summary>
    public class SignalVoltage : SignalBase
    {
        /// <summary>
        /// Верхняя граница напряжения объекта
        /// </summary>
        private double _maxVoltage;

        /// <summary>
        /// Нижняя граница напряжения объекта
        /// </summary>
        private double _minVoltage;
        
        /// <summary>
        /// Номинальное напряжение
        /// </summary>
        private double _nomVoltage;

        /// <summary>
        /// Верхняя граница напряжения объекта
        /// </summary>
        public double MaxVoltage
        {
            get => _maxVoltage;
            set => _maxVoltage = CheckValue(value);
        }

        /// <summary>
        /// Нижняя граница напряжения объекта
        /// </summary>
        public double MinVoltage
        {
            get => _minVoltage;
            set => _minVoltage = CheckValue(value);
        }

        /// <summary>
        /// Номинальное напряжение
        /// </summary>
        public double NomVoltage
        {
            get => _nomVoltage;
            set => _nomVoltage = CheckValue(value);
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="signalName">Название объекта</param>
        /// <param name="signalValue">Значение напряжения</param>
        /// <param name="time">Метка времени</param>
        /// <param name="maxVoltage">Верхняя граница напряжения объекта</param>
        /// <param name="minVoltage">Нижняя граница напряжения объекта</param>
        public SignalVoltage(string signalName, double signalValue, DateTime time,
            double maxVoltage, double minVoltage, double nomVoltage)
            : base(signalName, signalValue, time)
        {
            MaxVoltage = maxVoltage;
            MinVoltage = minVoltage;
            NomVoltage = nomVoltage;
        }
    }
}
