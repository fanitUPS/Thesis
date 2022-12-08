using System;

namespace ModelThesis
{
    /// <summary>
    /// Класс хранения ТИ для тока
    /// </summary>
    public class SignalCurrent : SignalBase
    {
        /// <summary>
        /// АДТН ЛЭП
        /// </summary>
        private double _maxCurrent;

        /// <summary>
        /// АДТН ЛЭП
        /// </summary>
        public double MaxCurrent
        {
            get => _maxCurrent;
            set => _maxCurrent = CheckValue(value);
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="signalName">Название ЛЭП</param>
        /// <param name="signalValue">Значение тока</param>
        /// <param name="time">Метка времени</param>
        /// <param name="maxCurrent">АДТН ЛЭП</param>
        public SignalCurrent(string signalName, double signalValue, DateTime time,
            double maxCurrent) : base (signalName, signalValue, time)
        {
            MaxCurrent = maxCurrent;
        }
    }
}
