using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Infrastructure;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace Starvers
{
	[Table("Starver14")]
	public class PlayerData
	{
		[Key] public int UserID { get; set; }
		public int Level { get; set; }
		public int Exp { get; set; }
		public int MP { get; set; }

		[JsonIgnore]public string SkillDatas { get; set; }
		[NotMapped]	public SkillStorage?[] Skills { get; set; }
		public PlayerData()
		{

		}
		public PlayerData(int userID)
		{
			UserID = userID;
			Level = 1;
			Exp = 0;
			Skills = new SkillStorage?[Starver.MaxSkillSlot];
		}

		public void GetSkillDatas(PlayerSkillData[] skills)
		{
			if (Starver.Instance.Config.StorageType == StorageType.MySql && Skills == null)
			{
				if (SkillDatas == null)
				{
					Skills = new SkillStorage?[Starver.MaxSkillSlot];
				}
				else
				{
					Skills = JsonConvert.DeserializeObject<SkillStorage?[]>(SkillDatas);
				}
			}
			for (int i = 0; i < Starver.MaxSkillSlot; i++)
			{
				if (Skills[i] == null)
				{
					skills[i] = default;
				}
				else
				{
					skills[i] = Skills[i].Value;
				}
			}
		}

		public void SetSkillDatas(PlayerSkillData[] skills)
		{
			for (int i = 0; i < Starver.MaxSkillSlot; i++)
			{
				Skills[i] = skills[i];
			}
			if (Starver.Instance.Config.StorageType == StorageType.MySql)
			{
				SkillDatas = JsonConvert.SerializeObject(Skills);
			}
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
		private class DatasContext : DbContext
		{
			public DbSet<PlayerData> Datas { get; set; }
			public DatasContext()
			{
			}
			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				base.OnConfiguring(optionsBuilder);
				var connectionString = TShock.DB.ConnectionString; 
				optionsBuilder.UseMySQL(connectionString);
				//optionsBuilder.Options.
			}
			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
			}
		}
		private string folder;
		private DatasContext context;

		public StorageType StorageType { get; }
		public PlayerDataManager(StorageType storageType)
		{
			StorageType = storageType;
			if (storageType == StorageType.MySql)
			{
				context = new DatasContext();
			}
			else if (storageType == StorageType.Json)
			{
				folder = Path.Combine(Starver.Instance.Config.SavePath, "PlayerDatas");
			}
		}

		public PlayerData GetData(int userID)
		{
			switch(StorageType)
			{
				case StorageType.Json:
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
				case StorageType.MySql:
					{
						return context.Datas.Find(userID) ?? throw new KeyNotFoundException("key: " + nameof(userID));
					}
				default:
					throw new NotImplementedException();
			}
		}
		public void SaveData(PlayerData data)
		{
			switch (StorageType)
			{
				case StorageType.Json:
					{
						var path = GetPath(data.UserID);
						File.WriteAllText(path, data.Serialize());
						break;
					}
				case StorageType.MySql:
					{
						if (context.Datas.Find(data.UserID) == null)
						{
							context.Datas.Add(data);
						}
						else
						{
							context.Entry(data).State = EntityState.Modified;
						}
						context.SaveChanges();
						break;
					}
				default: throw new NotImplementedException();
			}
		}

		private string GetPath(int userID)
		{
			return Path.Combine(folder, "data-" + userID + ".json");
		}
	}
}
