using System.Collections.Generic;

namespace VeilBlockToDB.ModelsRpc.Request
{
    public class BlockHeaderRequest
    {
        public BlockHeaderRequest()
        {
            @params = new List<string>();
        }
        public string jsonrpc { get { return "1.0"; } }
        public string id { get { return "veillogger"; } }
        public string method { get { return "getblockheader"; } }
        public List<string> @params { get; set; }
    }
}
