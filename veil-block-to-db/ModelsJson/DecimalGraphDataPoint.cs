using System.Runtime.Serialization;

namespace VeilBlockLoggerCleanup.ModelsJson
{
    [DataContract]
    public class DecimalGraphDataPoint
    {
        public DecimalGraphDataPoint(long x, decimal y, string label)
        {
            this.X = x;
            this.Y = y;
            this.label = label;
        }

        public DecimalGraphDataPoint()
        {
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "x")]
        public long X;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public decimal Y;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string label = "";
    }
}
