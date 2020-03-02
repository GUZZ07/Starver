using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using Newtonsoft.Json;
using Starvers.TaskSystem;

namespace Starvers
{
	public class StarverConfig
	{
		#region Properties
		public static StarverConfig Config { get; internal set; }
		public static DateTime LastSave { get; set; } = DateTime.Now;
		[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
		public TaskDifficulty TaskLevel { get; set; } = TaskDifficulty.Hard;
		public int DefaultLevel { get; set; } = 1;
		public int SaveInterval { get; set; } = 60;
		public int TaskNow { get; set; }
		public int TaskLock { get; set; } = StarverTaskManager.MainLineCount;
		public int LevelNeed { get; set; }
		public bool EnableStrongerNPC { get; set; } = true;
		public bool EnableTask { get; set; } = true;
		public bool EnableAura { get; set; } = true;
		public bool EnableNPC { get; set; } = true;
		public bool EnableBoss { get; set; } = true;
		public bool CanAfterMoon { get; set; }
		public bool EnableTestMode { get; set; }
		public bool EvilWorld { get; set; }
		public bool EnableSelfCollide { get; set; }
		public bool TaskNeedNoItem { get; set; }
		public string MySQLDBName { get; set; } = TShock.Config.MySqlDbName;
		public string MySQLUserName { get; set; } = TShock.Config.MySqlUsername;
		public string MySQLPassword { get; set; } = TShock.Config.MySqlPassword;
		public string MySQLHost { get; set; } = TShock.Config.MySqlHost;
		public SaveModes SaveMode { get; set; } = TShock.Config.StorageType.ToLower() == "mysql" ? SaveModes.MySQL : SaveModes.Json;
		#endregion
		#region Ctor & dtor
		private StarverConfig()
		{
			
		}
		~StarverConfig()
		{
			Write();
		}
		#endregion
		#region Read & Write
		internal static StarverConfig Read()
		{
			StarverConfig config;
			if (File.Exists(ConfigPath))
			{
				config = JsonConvert.DeserializeObject<StarverConfig>(File.ReadAllText(ConfigPath));
			}
			else
			{
				config = new StarverConfig();
				config.Write();
			}
			return config;
		}
		internal void Write()
		{
			File.WriteAllText(ConfigPath,JsonConvert.SerializeObject(this, Formatting.Indented));
		}
		internal static string ConfigPath =  "tshock//StarverConfig.json";
		#endregion
	}
}
