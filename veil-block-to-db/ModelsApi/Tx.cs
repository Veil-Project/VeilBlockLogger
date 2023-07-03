using System.Collections.Generic;

namespace VeilBlockToDB.ModelsApi
{
    public class Tx
    {
        public string txid { get; set; }
        public List<Vin> vin { get; set; }
        public List<Vout> vout { get; set; }
        public string blockhash { get; set; }
        public long blockheight { get; set; }
        public long confirmations { get; set; }
        public long time { get; set; }
        public string valueOut { get; set; }
        public string valueIn { get; set; }
        public string fees { get; set; }
        public string hex { get; set; }
    }
}
