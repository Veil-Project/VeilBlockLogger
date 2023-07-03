using System;
using System.Data.Entity;
using System.Linq;
using VeilBlockToDB.ModelsDb;

namespace VeilBlockToDB
{
    public class VeilContext : DbContext, IDisposable
    {
        public DbSet<BlockData> BlockData { set; get; }
        public DbSet<BlockDataExtended> BlockDataExtended { set; get; }
        public DbSet<ZerocoinSupply> ZerocoinSupply { set; get; }
        public DbSet<WinningDenom> WinningDenom { set; get; }
        public DbSet<DenomEfficiency>DenomEfficiency { set; get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }

        public BlockData GetBlockData(long blockID)
        {
           return this.BlockData.FirstOrDefault(w => w.BlockID == blockID);
        }
        public ZerocoinSupply GetZerocoinSupply(long blockID)
        {
           return this.ZerocoinSupply.FirstOrDefault(w => w.BlockID == blockID);
        }
        public WinningDenom GetWinningDenom(long blockID,string denom)
        {
           return this.WinningDenom.FirstOrDefault(w => w.BlockID == blockID && w.StakeDenom == denom);
        }
    }
}
