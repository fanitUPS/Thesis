using System;

namespace ModelThesis
{
    /// <summary>
    /// Класс хранения показателя тяжести
    /// </summary>
    public class PerformanceIndex
    {
        /// <summary>
        /// ИД показателя
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Зачение показателя
        /// </summary>
        public double Value { get; private set; }

        /// <summary>
        /// Метка времени
        /// </summary>
        public DateTime TimeStamp { get; private set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="id">ИД показателя</param>
        /// <param name="value">Зачение показателя</param>
        /// <param name="timeStamp">Метка времени</param>
        public PerformanceIndex(int id, double value, DateTime timeStamp)
        {
            Id = id;
            Value = value;
            TimeStamp = timeStamp;
        }
    }
}
