using Newtonsoft.Json;

namespace Blockexplorer.BlockProvider.Rpc.Client
{
    public class TransportRpcModel<T>
    {
        [JsonProperty("result")]
        public T Result { get; set; }
    }
}