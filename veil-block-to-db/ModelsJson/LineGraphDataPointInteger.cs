using System.Runtime.Serialization;

namespace VeilBlockToDB.ModelsJson
{
    [DataContract]
    public class LineGraphDataPointInteger
    {
        public LineGraphDataPointInteger(long x, int y, string label)
        {
            this.X = x;
            this.Y = y;
            this.label = label;
        }

        public LineGraphDataPointInteger()
        {
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "x")]
        public long X;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public int Y;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string label = "";
    }
}
