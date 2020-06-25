using Microsoft.Xna.Framework;
using Starvers.PlayerBoosts;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Starvers
{
	#region Using Alias
	using Assembly = System.Reflection.Assembly;
	#endregion
	[ApiVersion(2, 1)]
	public class Starver : TerrariaPlugin
	{
		#region Fields
		public const string ConfigPath = "tshock//StarverConfig.json";
		public readonly static Color DamageColor = Color.Yellow;
		public const int MaxSkillSlot = 5;
		#endregion
		#region Properties
		public static Starver Instance { get; private set; }

		public StarverConfig Config { get; private set; }
		public PlayerDataManager PlayerDatas { get; private set; }
		public StarverPlayer[] Players { get; private set; }
		public SkillManager Skills { get; private set; }
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
			Skills = new SkillManager();
			PlayerDatas = new PlayerDataManager(StorageType.MySql);
			#region Test
#if false
			var data = new PlayerData(-444) { Level = 3 };
			var skills = new PlayerSkillData[5];
			skills[0] = new PlayerSkillData
			{
				ID = (byte)SkillIDs.LawAias,
				BindByProj = true,
				BindID = ProjectileID.Spear
			};
			data.SetSkillDatas(skills);
			PlayerDatas.SaveData(data);
#endif
			#endregion
			Players = new StarverPlayer[TShock.Players.Length];
			#region Hooks
			ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
			ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
			ServerApi.Hooks.NpcStrike.Register(this, OnNpcStrike);
			ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
			TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += OnPostLogin;
			#endregion
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			#region Hooks
			ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
			ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
			ServerApi.Hooks.NpcStrike.Deregister(this, OnNpcStrike);
			ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
			TShockAPI.Hooks.PlayerHooks.PlayerPostLogin -= OnPostLogin;
			#endregion
		}

		#endregion
		#region Events
		private void OnPostLogin(PlayerPostLoginEventArgs args)
		{
			Players[args.Player.Index] = new StarverPlayer(args.Player.Index);
		}
		private void OnJoin(JoinEventArgs args)
		{
			if (!TShock.Players[args.Who].IsLoggedIn)
			{
				Players[args.Who] = StarverPlayer.GetGuest(args.Who);
			}
		}
		private void OnLeave(LeaveEventArgs args)
		{
			Players[args.Who]?.OnLeave();
			Players[args.Who] = null;
		}
		private void OnUpdate(EventArgs args)
		{
			foreach (var player in Players)
			{
				if (player == null)
				{
					continue;
				}
				player?.Update();
			}
		}

		private void OnNpcStrike(NpcStrikeEventArgs args)
		{
			var player = Players[args.Player.whoAmI];
			player.OnStrikeNpc(args);
		}
		#endregion
	}
}
