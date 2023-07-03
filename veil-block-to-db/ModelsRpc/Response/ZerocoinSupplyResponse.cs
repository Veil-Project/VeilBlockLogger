using System.Collections.Generic;
using VeilBlockToDB.ModelsApi;

namespace VeilBlockToDB.ModelsRpc.Response
{
    public class ZerocoinSupplyResponse : RpcResponseBase
    {
        public List<ZerocoinSupply> result { get; set; }
    }   
}
