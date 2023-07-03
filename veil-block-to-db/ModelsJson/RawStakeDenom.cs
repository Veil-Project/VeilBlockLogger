using System;

namespace VeilBlockToDB.ModelsJson
{
    public class RawStakeDenom
    {
        public long BlockID { get; set; }
        public DateTime BlockDate { get; set; }
        public string WinningDenom { get; set; }
        public decimal Supply10 { get; set; }
        public decimal Reward10 { get; set; }
        public decimal Efficiency10 { get; set; }
        public int Winning10s { get; set; }
        public decimal Supply100 { get; set; }
        public decimal Reward100 { get; set; }
        public decimal Efficiency100 { get; set; }
        public int Winning100s { get; set; }
        public decimal Supply1000 { get; set; }
        public decimal Reward1000 { get; set; }
        public decimal Efficiency1000 { get; set; }
        public int Winning1000s { get; set; }
        public decimal Supply10000 { get; set; }
        public decimal Reward10000 { get; set; }
        public decimal Efficiency10000 { get; set; }
        public int Winning10000s { get; set; }
    }
}
