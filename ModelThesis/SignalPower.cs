using System;

namespace ModelThesis
{
    /// <summary>
    /// Класс хранения значений ТИ активной мощности
    /// </summary>
    public class SignalPower : SignalBase
    {
        /// <summary>
        /// МДП в КС
        /// </summary>
        private double _maxPower;

        /// <summary>
        /// МДП в КС
        /// </summary>
        public double MaxPower
        {
            get => _maxPower;
            set => _maxPower = CheckValue(value);
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="signalName">Название КС</param>
        /// <param name="signalValue">Значение мощности</param>
        /// <param name="time">Метка времени</param>
        /// <param name="maxPower">МДП в КС</param>
        public SignalPower(string signalName, double signalValue, DateTime time,
            double maxPower) : base(signalName, signalValue, time)
        {
            MaxPower = maxPower;
        }
    }
}
