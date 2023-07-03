using VeilBlockToDB.ModelsApi;

namespace VeilBlockToDB.ModelsRpc.Response
{
    public class BlockResponse : RpcResponseBase
    {
        public Block result { get; set; }
    } 
}
