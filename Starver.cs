using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Starver
{
	#region Using Alias
	using Assembly = System.Reflection.Assembly;
	#endregion
	[ApiVersion(2, 1)]
	public class Starver : TerrariaPlugin
	{
		#region Fields
		public const string ConfigPath = "tshock//StarverConfig.json";
		#endregion
		#region Properties
		public static Starver Instance { get; private set; }

		public StarverConfig Config { get; private set; }
		public PlayerDataManager PlayerDatas { get; private set; }
		#region Plugin Infos
		public override string Name => nameof(Starver);
		public override string Description => nameof(Starver);
		public override string Author => "1413";
		#endregion
		#endregion
		#region Load & Unload
		public Starver(Main game) : base(game)
		{

		}
		public override void Initialize()
		{
			Instance = this;

			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{
				var path = Path.Combine("Nugets", args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll");
				if (File.Exists(path))
				{
					return Assembly.LoadFrom(path);
				}
				return null;
			};

			Config = StarverConfig.Read(ConfigPath);
			PlayerDatas = new PlayerDataManager(StorageType.MySql);
			PlayerDatas.SaveData(new PlayerData(-44) { Exp = 1, Level = 20 });
		}
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		#endregion
	}
}
