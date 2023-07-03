using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace VeilBlockToDB.ModelsDb
{
    [Table("WinningDenom", Schema = "bak")]
    public class WinningDenom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 XID { get; set; }
        public Int64 BlockID { get; set; }
        public DateTime BlockDate { get; set; }
        public string StakeDenom { get; set; }

    }
}
