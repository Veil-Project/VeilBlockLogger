using System;
using System.Collections.Generic;

namespace VeilBlockToDB.ModelsApi
{
    public class Block
    {
        public string hash { get; set; }
        public long confirmations { get; set; }
        public long strippedsize { get; set; }
        public long size { get; set; }
        public long weight { get; set; }
        public long height { get; set; }
        public string proof_type { get; set; }
        public string proofofstakehash { get; set; }
        public long version { get; set; }
        public string versionHex { get; set; }
        public string merkleroot { get; set; }
        public long time { get; set; }
        public long mediantime { get; set; }
        public long nonce { get; set; }
        public ulong nonce64 { get; set; }
        public string mixhash { get; set; }
        public string bits { get; set; }
        public double difficulty { get; set; }
        public string chainwork { get; set; }
        public int nTx { get; set; }
        public long anon_index { get; set; }
        public string veil_data_hash { get; set; }
        public string previousblockhash { get; set; }
        public string nextblockhash { get; set; }
        public DateTime BlockDate { get { return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(time); } }
        public List<Tx> tx { get; set; }

        public bool IsPos { 
            get { 
                if(!string.IsNullOrWhiteSpace(proof_type) && proof_type == "Proof-of-Stake")
                {
                    return true;
                }
                return !string.IsNullOrWhiteSpace(versionHex) && versionHex == "30000000"; 
            } 
        }
    }
}
