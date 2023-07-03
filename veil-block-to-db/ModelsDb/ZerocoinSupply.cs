using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilBlockToDB.ModelsDb
{
    [Table("ZerocoinSupply", Schema = "bak")]
    public class ZerocoinSupply
    {
        private int _iSupplyDivisor = 100000000;
        private decimal _dAmount = 0m;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 XID { get; set; }
        public Int64 BlockID { get; set; }
        public string Denom { get; set; }
        public decimal Amount
        {
            get { return (_dAmount / _iSupplyDivisor); }
            set { _dAmount = value; }
        }
        public decimal PercentOfSupply { get; set; }
    }
}
