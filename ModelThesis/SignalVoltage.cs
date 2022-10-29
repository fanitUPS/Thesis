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
        private float _maxVoltage;

        /// <summary>
        /// Нижняя граница напряжения объекта
        /// </summary>
        private float _minVoltage;

        /// <summary>
        /// Верхняя граница напряжения объекта
        /// </summary>
        public float MaxVoltage
        {
            get => _maxVoltage;
            set => _maxVoltage = CheckValue(value);
        }

        /// <summary>
        /// Нижняя граница напряжения объекта
        /// </summary>
        public float MinVoltage
        {
            get => _minVoltage;
            set => _minVoltage = CheckValue(value);
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="signalName">Название объекта</param>
        /// <param name="signalValue">Значение напряжения</param>
        /// <param name="maxVoltage">Верхняя граница напряжения объекта</param>
        /// <param name="minVoltage">Нижняя граница напряжения объекта</param>
        public SignalVoltage(string signalName, float signalValue,
            float maxVoltage, float minVoltage)
            : base(signalName, signalValue)
        {
            MaxVoltage = maxVoltage;
            MinVoltage = minVoltage;
        }
    }
}
