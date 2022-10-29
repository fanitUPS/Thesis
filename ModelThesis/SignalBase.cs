using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private string _signalName;

        /// <summary>
        /// Название объекта
        /// </summary>
        public string SignalName { get; set; }

        /// <summary>
        /// Значение ТИ
        /// </summary>
        private float _signalValue;

        /// <summary>
        /// Значение ТИ
        /// </summary>
        public float SignalValue
        {
            get => _signalValue;
            set => _signalValue = CheckValue(value);
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="signalName">Название объекта</param>
        /// <param name="signalValue">Значение ТИ</param>
        protected SignalBase(string signalName, float signalValue)
        {
            SignalName = signalName;
            SignalValue = signalValue;
        }

        /// <summary>
        /// Проверка значения ТИ
        /// </summary>
        /// <param name="value">Значение ТИ</param>
        /// <returns>Проверенное значение ТИ</returns>
        protected float CheckValue(float value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Значение ТИ не может быть меньше нуля.");
            }

            return value;
        }
    }
}
