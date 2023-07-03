using System;
using System.Collections.Generic;
using System.Globalization;

namespace VeilBlockToDB.ModelsApi
{
    public class HistoricalBlock
    {
        private double _dDiff = 0;

        public int page { get; set; }
        public int totalPages { get; set; }
        public int itemsOnPage { get; set; }
        public string hash { get; set; }
        public string previousblockhash { get; set; }
        public string nextblockhash { get; set; }
        public long height { get; set; }
        public long confirmations { get; set; }
        public int size { get; set; }
        public int time { get; set; }
        public DateTime BlockDate { get { return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(time); } }
        public long moneysupply { get; set; }
        public int Type { get; set; }
        public List<ZerocoinSupply> zerocoinsupply { get; set; }
        public int version { get; set; }
        public string merkleroot { get; set; }
        public string nonce { get; set; }
        public string bits { get; set; }
        public string difficulty { get; set; }
        public double Diff { get { return double.Parse(difficulty, CultureInfo.InvariantCulture); } }
        public int txCount { get; set; }
        public List<Tx> txs { get; set; }
    }
}
