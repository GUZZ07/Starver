using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace Starver
{
	public class PlayerData
	{
		public int UserID { get; set; }
		public int Level { get; set; }
		public int Exp { get; set; }
		public PlayerData(int userID)
		{
			UserID = userID;
			Level = 1;
			Exp = 0;
		}

		public string Serialize()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		public static PlayerData Deserialize(string text)
		{
			return JsonConvert.DeserializeObject<PlayerData>(text);
		}
	}
	public class PlayerDataManager
	{
		private string folder;

		public PlayerDataManager()
		{
			folder = Path.Combine(Starver.Instance.Config.SavePath, "PlayerDatas");
		}

		public PlayerData GetData(int userID)
		{
			var path = GetPath(userID);
			if (!File.Exists(path))
			{
				PlayerData data = new PlayerData(userID);
				File.WriteAllText(path, data.Serialize());
				return data;
			}
			return PlayerData.Deserialize(File.ReadAllText(path));
		}
		public void SaveData(PlayerData data)
		{
			var path = GetPath(data.UserID);
			File.WriteAllText(path, data.Serialize());
		}

		private string GetPath(int userID)
		{
			return Path.Combine(folder, "data-" + userID + ".json");
		}
	}
}
