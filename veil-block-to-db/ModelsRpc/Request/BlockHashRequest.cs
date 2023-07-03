using System.Collections.Generic;

namespace VeilBlockToDB.ModelsRpc.Request
{
    public class BlockHashRequest
    {
        public BlockHashRequest()
        {
            @params = new List<long>();
        }
        public string jsonrpc { get { return "1.0"; } }
        public string id { get { return "veillogger"; } }
        public string method { get { return "getblockhash"; } }
        public List<long> @params { get; set; }
    }
}
