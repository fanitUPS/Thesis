using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelThesis
{
    public class UuidContainer
    {
        public string Name { get; private set; }

        public string Value { get; private set; }

        public string MaxValue { get; private set; }

        public string MinValue { get; private set; }

        public string NomValue { get; private set; }

        public UuidContainer(string name, string value, string maxValue, 
            string minValue, string nomValue)
        {
            Name = name;
            Value = value;
            MaxValue = maxValue;
            MinValue = minValue;
            NomValue = nomValue;
        }

        public UuidContainer(string name, string value, string maxValue)
        {
            Name = name;
            Value = value;
            MaxValue = maxValue;
        }
    }
}
