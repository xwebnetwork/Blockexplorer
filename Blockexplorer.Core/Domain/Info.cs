namespace Blockexplorer.Core.Domain
{
	/// <summary>
	/// Models return values from getinfo.
	/// </summary>
    public class Info
    {
		public decimal MoneySupply { get; set; }
	    public string  Version { get; set; }
		public int Blocks { get; set; }
	    public int Connections { get; set; }
		public string  Errors { get; set; }
	}
}
