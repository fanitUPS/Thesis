using Ck = Monitel.Rtdb.Api;
using System;
using System.Collections.Generic;

namespace ModelThesis
{
    /// <summary>
    /// Класс для валидации данных из СК-11
    /// </summary>
    public class Verification
    {
        /// <summary>
        /// Достоверный код качества ТИ
        /// </summary>
        private readonly string[] _goodQualitys = 
            new string[] 
            {
                "10000002",
                "10004000",
                "10008000",
                "10010000",
                "10020000",
                "10000020"
            };
        
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
        public Verification(Ck.RtdbValue[] inputData)
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

            var notValid = new Ck.RtdbValue();
            
            for (int i = 0; i < this.InputData.Length; i++)
            {
                if (Array.Exists(_goodQualitys, value => value ==
                    Convert.ToString(this.InputData[i].QualityCodes, 16)))
                {
                    validDataList.Add(this.InputData[i]);
                }
                else
                {
                    validDataList.Add(notValid);
                }
            }
            
            return validDataList.ToArray();
        }
    }
}
