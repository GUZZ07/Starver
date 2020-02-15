namespace Starvers
{
	public partial class StarverPlayer
	{
		public class PlayerData
		{
			public int Level { get; set; }
			public int Exp { get; set; }
			public int[] Skills { get; set; }
			public byte[] BLDatas { get; set; }
			public int MaxMP { get; set; }
		}
	}
}
