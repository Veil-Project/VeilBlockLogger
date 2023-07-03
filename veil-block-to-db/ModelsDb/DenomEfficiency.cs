using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace VeilBlockToDB.ModelsDb
{
    [Table("DenomEfficiency", Schema = "bak")]
    public class DenomEfficiency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 XID { get; set; }
        public long BlockID { get; set; }
        public DateTime BlockDate { get; set; }
        public decimal Supply10 { get; set; }
        public decimal Reward10 { get; set; }
        public decimal Efficiency10 { get; set; }
        public decimal Supply100 { get; set; }
        public decimal Reward100 { get; set; }
        public decimal Efficiency100 { get; set; }
        public decimal Supply1000 { get; set; }
        public decimal Reward1000 { get; set; }
        public decimal Efficiency1000 { get; set; }
        public decimal Supply10000 { get; set; }
        public decimal Reward10000 { get; set; }
        public decimal Efficiency10000 { get; set; }
    }
}
