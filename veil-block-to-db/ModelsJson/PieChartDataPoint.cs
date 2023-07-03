using System.Runtime.Serialization;

namespace VeilBlockToDB.ModelsJson
{
    [DataContract]
    public class PieChartDataPoint
    {
        public PieChartDataPoint(decimal y, string label)
        {
            this.Y = y;
            this.label = label;
        }

        public PieChartDataPoint()
        {
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public decimal Y;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string label = "";
    }
}
