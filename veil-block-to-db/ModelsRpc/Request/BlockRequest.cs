using System.Collections.Generic;

namespace VeilBlockToDB.ModelsRpc.Request
{
    public class BlockRequest
    {
        public BlockRequest()
        {
            @params = new List<object>();
        }
        public string jsonrpc { get { return "1.0"; } }
        public string id { get { return "veillogger"; } }
        public string method { get { return "getblock"; } }
        public List<object> @params { get; set; }
    }
}
