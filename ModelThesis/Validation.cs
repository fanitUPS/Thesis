using Ck = Monitel.Rtdb.Api;
using System;
using System.Collections.Generic;

namespace ModelThesis
{
    /// <summary>
    /// Класс для валидации данных из СК-11
    /// </summary>
    public class Validation
    {
        /// <summary>
        /// Достоверный код качества ТИ
        /// </summary>
        private const string _goodQuality = "10000002";

        /// <summary>
        /// Входные данные
        /// </summary>
        private Ck.RtdbValue[] _inputData;

        /// <summary>
        /// Входные данные
        /// </summary>
        public Ck.RtdbValue[] InputData
        {
            get => _inputData;
            set
            {
                if (value.Length == 0)
                {
                    throw new ArgumentException
                        ("Массив не содержит данных.");
                }

                _inputData = value;
            }
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="inputData">Входные данные для проверки</param>
        public Validation(Ck.RtdbValue[] inputData)
        {
            InputData = inputData;
        }

        /// <summary>
        /// Осуществление проверки данных
        /// </summary>
        /// <returns>Массив достоверных значений</returns>
        public Ck.RtdbValue[] GetValidData()
        {
            var validDataList = new List<Ck.RtdbValue>();

            for (int i = 0; i < this.InputData.Length; i++)
            {
                if (Convert.ToString
                    (this.InputData[i].QualityCodes, 16) == _goodQuality)
                {
                    validDataList.Add(this.InputData[i]);
                }
            }

            return validDataList.ToArray();
        }
    }
}
