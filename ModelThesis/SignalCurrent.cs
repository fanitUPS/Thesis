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
        private float _maxCurrent;

        /// <summary>
        /// АДТН ЛЭП
        /// </summary>
        public float MaxCurrent
        {
            get => _maxCurrent;
            set => _maxCurrent = CheckValue(value);
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="signalName">Название ЛЭП</param>
        /// <param name="signalValue">Значение тока</param>
        /// <param name="maxCurrent">АДТН ЛЭП</param>
        public SignalCurrent(string signalName,float signalValue,
            float maxCurrent) : base (signalName, signalValue)
        {
            MaxCurrent = maxCurrent;
        }
    }
}
