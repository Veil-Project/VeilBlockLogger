namespace VeilBlockToDB.ModelsApi
{    
    public class BlockHeader
    {
        public string hash { get; set; }
        public int confirmations { get; set; }
        public int height { get; set; }
        public long moneysupply { get; set; }
        public int version { get; set; }
        public string versionHex { get; set; }
        public string merkleroot { get; set; }
        public int time { get; set; }
        public int mediantime { get; set; }
        public int nonce { get; set; }
        public string bits { get; set; }
        public double difficulty { get; set; }
        public string chainwork { get; set; }
        public int nTx { get; set; }
        public string previousblockhash { get; set; }
        public string nextblockhash { get; set; }
    }

}
