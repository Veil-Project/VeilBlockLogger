using System.Collections.Generic;

namespace VeilBlockToDB.ModelsJson
{
    public class MultiSeriesLineChart
    {
        public MultiSeriesLineChart() {
            Series1 = new List<LineGraphDataPointDecimal>();
            Series2 = new List<LineGraphDataPointDecimal>();
            Series3 = new List<LineGraphDataPointDecimal>();
            Series4 = new List<LineGraphDataPointDecimal>();
        }

        public string LastBlockTime { get; set; }
        public List<LineGraphDataPointDecimal> Series1 {get;set; }
        public List<LineGraphDataPointDecimal> Series2 {get;set; }
        public List<LineGraphDataPointDecimal> Series3 {get;set; }
        public List<LineGraphDataPointDecimal> Series4 {get;set; }
    }
}
