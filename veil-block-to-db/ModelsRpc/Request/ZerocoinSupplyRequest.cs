using System.Collections.Generic;

namespace VeilBlockToDB.ModelsRpc.Request
{
    public class ZerocoinSupplyRequest
    {
        public ZerocoinSupplyRequest()
        {
            @params = new List<long>();
        }
        public string jsonrpc { get { return "1.0"; } }
        public string id { get { return "veillogger"; } }
        public string method { get { return "getzerocoinsupply"; } }
        public List<long> @params { get; set; }
    }
}
