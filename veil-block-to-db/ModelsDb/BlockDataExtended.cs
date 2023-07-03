using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilBlockToDB.ModelsDb
{
    [Table("BlockDataExtended", Schema = "bak")]
    public class BlockDataExtended
    {
        private int _iSupplyDivisor = 100000000;
        private decimal _dMoneySupply = 0;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long XID { get; set; }
        public long BlockID { get; set; }
        public int PosBlocks24Hr { get; set; }
        public decimal PosBlocks24HrPercent { get; set; }
        public int PowBlocks24Hr { get; set; }
        public decimal PowBlocks24HrPercent { get; set; }
        public int X16rtBlocks24Hr { get; set; }
        public decimal X16rtBlocks24HrPercent { get; set; }
        public int RandomXBlocks24Hr { get; set; }
        public decimal RandomXBlocks24HrPercent { get; set; }
        public int ProgPowBlocks24Hr { get; set; }
        public decimal ProgPowBlocks24HrPercent { get; set; }
        public int ShaBlocks24Hr { get; set; }
        public decimal ShaBlocks24HrPercent { get; set; }
    }
}