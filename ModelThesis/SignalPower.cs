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
        private float _maxPower;

        /// <summary>
        /// МДП в КС
        /// </summary>
        public float MaxPower
        {
            get => _maxPower;
            set => _maxPower = CheckValue(value);
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="signalName">Название КС</param>
        /// <param name="signalValue">Значение мощности</param>
        /// <param name="maxPower">МДП в КС</param>
        public SignalPower(string signalName, float signalValue,
            float maxPower) : base(signalName, signalValue)
        {
            MaxPower = maxPower;
        }
    }
}
