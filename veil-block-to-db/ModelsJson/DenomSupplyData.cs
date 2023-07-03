using System;

namespace VeilBlockToDB.ModelsJson
{
    public class DenomSupplyData
    {
        public long MaxBlock { get; set; }
        public DateTime MaxBlockDate { get; set; }
        public long AvgZerocoinSupply { get; set; }
        public long AvgAmount10s { get; set; }
        public long AvgAmount100s { get; set; }
        public long AvgAmount1000s { get; set; }
        public long AvgAmount10000s { get; set; }
        public decimal Percent10s { get; set; }
        public decimal Percent100s { get; set; }
        public decimal Percent1000s { get; set; }
        public decimal Percent10000s { get; set; }
    }
}
