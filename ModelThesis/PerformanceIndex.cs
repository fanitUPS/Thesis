using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelThesis
{
    public class PerformanceIndex
    {
        public int Id { get; set; }

        public double Value { get; set; }

        public DateTime TimeStamp { get; set; }

        public PerformanceIndex(int id, double value, DateTime timeStamp)
        {
            Id = id;
            Value = value;
            TimeStamp = timeStamp;
        }
    }
}
