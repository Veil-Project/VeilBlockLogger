using VeilBlockToDB.ModelsApi;

namespace VeilBlockToDB.ModelsRpc.Response
{
    public class BlockHeaderResponse : RpcResponseBase
    {
        public BlockHeader result { get; set; }
    }
}
