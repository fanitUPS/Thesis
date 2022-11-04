using System;
using Ck = Monitel.Rtdb.Api;
namespace ModelThesis
{
    /// <summary>
    /// Базовый класс для хранения данных ТИ
    /// </summary>
    public abstract class SignalBase
    {
        /// <summary>
        /// Название объекта
        /// </summary>
        public string SignalName { get; set; }

        /// <summary>
        /// Значение ТИ
        /// </summary>
        private double _signalValue;

        /// <summary>
        /// Значение ТИ
        /// </summary>
        public double SignalValue
        {
            get => _signalValue;
            set => _signalValue = CheckValue(value);
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="signalName">Название объекта</param>
        /// <param name="signalValue">Значение ТИ</param>
        protected SignalBase(string signalName, double signalValue)
        {
            SignalName = signalName;
            SignalValue = signalValue;
        }

        /// <summary>
        /// Проверка значения ТИ
        /// </summary>
        /// <param name="value">Значение ТИ</param>
        /// <returns>Проверенное значение ТИ</returns>
        protected double CheckValue(double value)
        {
            if (value < 0)
            {
                throw new ArgumentException
                    ("Значение ТИ не может быть меньше нуля.");
            }

            return value;
        }
    }
}
