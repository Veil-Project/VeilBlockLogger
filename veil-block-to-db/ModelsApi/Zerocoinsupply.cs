using System;

namespace VeilBlockToDB.ModelsApi
{
    public class ZerocoinSupply
    {
        public string denom { get; set; }
        public Int64 amount { get; set; }
        public decimal amount_formatted { get; set; }
        public decimal percent { get; set; }
    }

}
