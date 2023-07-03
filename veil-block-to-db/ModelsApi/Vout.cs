namespace VeilBlockToDB.ModelsApi
{
    public class Vout
    {
        public double value { get; set; }
        public string type { get; set; }
        public double valueSat { get; set; }
        public ScriptPubKey scriptPubKey { get; set; }
    }
}
