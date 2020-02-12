using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
	using Microsoft.Xna.Framework;
	using Terraria.ID;

namespace Starvers.BossSystem
{
	using Bosses;
	using Bosses.Base;
	using Bosses.Clover;
	public class StarverBossManager : IStarverPlugin
	{
		#region Fields
		private int EndBossDelay = 0;
		private uint Timer;
		internal static bool TriedSpawn;
		/// <summary>
		/// 生成那个boss?
		/// </summary>
		internal static int SpawnIndex;
		internal static int SpawnDelay;
		internal static bool Cleared;

		public const short EndTrialSummoner = ItemID.WhiteCultistArcherBanner;
		#endregion
		#region Ctor
		#endregion
		#region Interfaces
		public delegate void AIDelegate(object args);
		public StarverConfig Config => StarverConfig.Config;
		public bool Enabled
		{
			get
			{
				if (Config.EnableBoss)
				{
					if (!Config.EnableNPC)
					{
						StarverPlayer.Server.SendInfoMessage("使用Starver插件的boss组件需要先开启NPC, Task以及Aura组件");
						return false;
					}
					return true;
				}
				return false;
			}
		}
		public void Load()
		{
			foreach (var boss in Bosses)
			{
				BossAIs.Add(boss.AI);
			}

			ServerApi.Hooks.NpcKilled.Register(Starver.Instance, OnKilled);
			ServerApi.Hooks.NpcLootDrop.Register(Starver.Instance, OnDrop);
			Commands.ChatCommands.Add(new Command(Perms.Boss.Spawn, BossCommand, "stboss"));
			ServerApi.Hooks.GameUpdate.Register(Starver.Instance, OnUpdate);
			GetDataHandlers.PlayerSlot += OnSlot;
		}
		public void UnLoad()
		{
			ServerApi.Hooks.NpcKilled.Deregister(Starver.Instance, OnKilled);
			ServerApi.Hooks.NpcLootDrop.Deregister(Starver.Instance, OnDrop);
			ServerApi.Hooks.GameUpdate.Deregister(Starver.Instance, OnUpdate);
			GetDataHandlers.PlayerSlot -= OnSlot;
		}
		#endregion
		#region Properties
		public bool EndTrial
		{
			get => StarverBoss.EndTrial;
			set => StarverBoss.EndTrial = value;
		}
		public int EndTrialProcess
		{
			get => StarverBoss.EndTrialProcess;
			set => StarverBoss.EndTrialProcess = value;
		}
		public static StarverBoss[] Bosses { get; private set; } = new StarverBoss[]
			{
				new EyeEx(),
				new BrainEx(),
				new QueenBeeEx(),
				new SkeletronEx(),
				new DarkMageEx(),
				new RedDevilEx(),
				new PigronEx(),
				new PrimeEx(),
				new DestroyerEx(),
				new RetinazerEx(),
				new SpazmatismEx(),
				new StarverWander(),
				new StarverRedeemer(),
				new StarverAdjudicator(),
				new StarverDestroyer(),
				new StarverManager(),
				new CultistEx()
			};
		public static StarverManager Clover => Bosses[Bosses.Length - 2] as StarverManager;
		public static Random Rand => Starver.Rand;
		public static List<AIDelegate> BossAIs = new List<AIDelegate>();
		#endregion
		#region Events
		#region OnSlot
		private void OnSlot(object sender, GetDataHandlers.PlayerSlotEventArgs args)
		{
			if (args.Slot == 49 && args.Type == EndTrialSummoner)
			{
				if (Bosses[Bosses.Length - 1].CanSpawn && !Bosses[Bosses.Length - 1].Active)
				{
					Main.player[args.PlayerId].inventory[args.Slot].stack -= 1; ;
					NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.PlayerId, args.Slot);
					Bosses[Bosses.Length - 1].Spawn(Main.player[args.PlayerId].Center + Rand.NextVector2(16 * 20));
				}
			}
		}
		#endregion
		#region OnUpdate
		private void OnUpdate(EventArgs args)
		{
			Timer++;
			#region UpdateMoon
			if (!TShock.Config.DisableHardmode)
			{
				if (EndTrial)
				{
					Cleared = false;
					if (Timer % 30 == 0)
					{
						foreach (var player in Starver.Players)
						{
							player?.UpdateMoon();
						}
					}
				}
				else
				{
					if (!Cleared)
					{
						Cleared = true;
						foreach (var player in Starver.Players)
						{
							player?.UpdateMoonClear();
						}
					}
				}
			}
			#endregion
			#region SetSkillCanUse
			if (Config.EnableBoss)
			{
				StarverBoss.AliveBoss = 0;
				foreach (var boss in Bosses)
				{
					if (boss.Active)
					{
						StarverBoss.AliveBoss += 1;
					}
				}
			}
			#endregion
			#region SpawnBoss
			if (Config.TaskNow >= 22 && StarverBoss.AliveBoss == 0)
			{
				if (Main.dayTime)
				{
					TriedSpawn = false;
					SpawnDelay = 0;
				}
				else
				{
					#region SelectWhichToSpawn
					if (!TriedSpawn)
					{
						TriedSpawn = true;
						bool CanSpawn = false;
						foreach (var ply in Starver.Players)
						{
							if (ply is null)
							{
								continue;
							}
							if(ply.Active)
							{
								CanSpawn = true;
								break;
							}
						}
						if (EndTrial == false && CanSpawn && Rand.Next(100) >= 10)
						{
							SpawnDelay = 60 * 30;
							SpawnIndex = Rand.Next(Bosses.Length - 2);
							if (!Bosses[SpawnIndex].CanSpawn)
							{
								do
								{
									SpawnIndex = Rand.Next(Bosses.Length - 2);
								}
								while (!Bosses[SpawnIndex].CanSpawn);
							}
							StarverPlayer.All.SendMessage(Bosses[SpawnIndex].ComingMessage, Bosses[SpawnIndex].ComingMessageColor);
						}
					}
					#endregion
					#region Spawning
					if (SpawnDelay > 0)
					{
						SpawnDelay--;
						if (SpawnDelay == 0)
						{
							foreach (var ply in Starver.Players)
							{
								if (ply is null || !ply.Active) 
								{
									continue;
								}
								Bosses[SpawnIndex].Spawn(ply.Center + Rand.NextVector2(16 * 20));
								break;
							}
						}
					}
					#endregion
				}
			}
			#endregion
			#region BossAI
			foreach (var boss in Bosses)
			{
				try
				{
					boss.AI();
				}
				catch(KeyNotFoundException)
				{
					continue;
				}
				catch (Exception e)
				{
					StarverPlayer.Server.SendInfoMessage(e.ToString());
				}
			}
			#endregion
			#region EndTrial
			if (EndTrial)
			{
				switch (EndTrialProcess)
				{
					#region 0(Starting)
					case 0:
						KillTower();
						WorldGen.TriggerLunarApocalypse();
						SummonTower(NPCID.LunarTowerVortex);
						StarverPlayer.All.SendMessage("这将是你们最后一战...", Color.DarkGreen);
						EndTrialProcess++;
						break;
					#endregion
					#region 1(Vortex Invading)
					case 1:
						if (NPC.ShieldStrengthTowerVortex < 1)
						{
							TowerTakeDamage();
						}
						if (!NPC.TowerActiveVortex)
						{
							KillTower();
							EndBossDelay = 60 * 30;
							EndTrialProcess++;
						}
						break;
					#endregion
					#region 2(CrazyWang)
					case 2:
						if (EndBossDelay-- == 60 * 30)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 28)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 26)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 23)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 20)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 18)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 13)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 10)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 7)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 3)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 1)
						{
							Bosses[Bosses.Length - 6].Spawn(SelectLuckyPlayer() + Rand.NextVector2(16 * 80), StarverBoss.CriticalLevel + 1000);
							EndBossDelay = -1;
						}
						break;
					#endregion
					#region 3(Stardust Invading)
					case 3:
						if (EndBossDelay < 0)
						{
							EndBossDelay = 0;
							WorldGen.TriggerLunarApocalypse();
							SummonTower(NPCID.LunarTowerStardust);
							StarverPlayer.All.SendMessage("战斗还在继续...", Color.DarkGreen);
						}
						if (!NPC.TowerActiveStardust)
						{
							KillTower();
							EndBossDelay = 60 * 30;
							EndTrialProcess++;
						}
						break;
					#endregion
					#region 4(Deaths)
					case 4:
						if (EndBossDelay-- == 60 * 30)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 28)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 26)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 23)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 20)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 18)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 13)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 10)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 7)
						{
							StarverPlayer.All.SendMessage("Empty", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 3)
						{
							StarverPlayer.All.SendMessage("祝你们好运", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 1)
						{
							Bosses[Bosses.Length - 5].Spawn(SelectLuckyPlayer() + Rand.NextVector2(16 * 80), StarverBoss.CriticalLevel + 1000);
							EndBossDelay = -1;
						}
						break;
					#endregion
					#region 5(Nebula Invading)
					case 5:
						if (EndBossDelay < 0)
						{
							EndBossDelay = 0;
							SummonTower(NPCID.LunarTowerNebula);
							StarverPlayer.All.SendMessage("战斗还将延续...", Color.DarkGreen);
						}
						if (!NPC.TowerActiveNebula)
						{
							KillTower();
							EndBossDelay = 60 * 30;
							EndTrialProcess++;
						}
						break;
					#endregion
					#region 6(Wither)
					case 6:
						if (EndBossDelay-- == 60 * 30)
						{
							StarverPlayer.All.SendMessage("Empty", Color.DeepPink);
						}
						else if (EndBossDelay == 60 * 28)
						{
							StarverPlayer.All.SendMessage("Empty", Color.DeepPink);
						}
						else if (EndBossDelay == 60 * 26)
						{
							StarverPlayer.All.SendMessage("Empty", Color.DeepPink);
						}
						else if (EndBossDelay == 60 * 23)
						{
							StarverPlayer.All.SendMessage("Empty", Color.DeepPink);
						}
						else if (EndBossDelay == 60 * 20)
						{
							StarverPlayer.All.SendMessage("Empty", Color.DeepPink);
						}
						else if (EndBossDelay == 60 * 18)
						{
							StarverPlayer.All.SendMessage("我们遇到大麻烦了", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 13)
						{
							StarverPlayer.All.SendMessage("快跑", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 10)
						{
							StarverPlayer.All.SendMessage("我们不是他的对手的", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 7)
						{
							StarverPlayer.All.SendMessage("除非...", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 3)
						{
							StarverPlayer.All.SendMessage("用上那个", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 1)
						{
							Bosses[Bosses.Length - 4].Spawn(SelectLuckyPlayer() + Rand.NextVector2(16 * 80), StarverBoss.CriticalLevel + 1000);
							EndBossDelay = -1;
						}
						break;
					#endregion
					#region 7(Solar Invading)
					case 7:
						if (EndBossDelay < 0)
						{
							EndBossDelay = 0;
							SummonTower(NPCID.LunarTowerSolar);
							StarverPlayer.All.SendMessage("战斗已接近尾声...", Color.DarkGreen);
						}
						if (!NPC.TowerActiveSolar)
						{
							KillTower();
							EndBossDelay = 60 * 30;
							EndTrialProcess++;
						}
						break;
					#endregion
					#region 8(TOFOUT)
					case 8:
						if (EndBossDelay-- == 60 * 30)
						{
							StarverPlayer.All.SendMessage("他正在逐步接近", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 28)
						{
							StarverPlayer.All.SendMessage("那个世界是另一个世界的复制品", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 26)
						{
							StarverPlayer.All.SendMessage("他在那个空无一人的世界里", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 23)
						{
							StarverPlayer.All.SendMessage("将整个世界作为了实验品", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 20)
						{
							StarverPlayer.All.SendMessage("开始了他的实验", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 18)
						{
							StarverPlayer.All.SendMessage("一开始他的实验品经常变成空气, 他一直为此苦恼", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 13)
						{
							StarverPlayer.All.SendMessage("但是到了后来他已经能够随心所欲的控制它们", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 10)
						{
							StarverPlayer.All.SendMessage("会爆炸的僵尸...会射箭的魔化盔甲...四处射激光的眼球", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 7)
						{
							StarverPlayer.All.SendMessage("还有那四处漂浮的骷髅...都是他的作品", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 3)
						{
							StarverPlayer.All.SendMessage("你们也许也会", Color.BlueViolet);
						}
						else if (EndBossDelay == 60 * 1)
						{
							Bosses[Bosses.Length - 3].Spawn(SelectLuckyPlayer() + Rand.NextVector2(16 * 80), StarverBoss.CriticalLevel + 1000);
							EndBossDelay = -1;
						}
						break;
					#endregion
					#region 9(Clover Coming)
					case 9:
						EndBossDelay = 60 * 60;
						EndTrialProcess++;
						break;
					#endregion
					#region 10(Clover)
					case 10:
						if (EndBossDelay-- == 0)
						{
							StarverPlayer.All.SendMessage("这是你们的最后一战", Color.Black);
							Bosses[Bosses.Length - 2].Spawn(SelectLuckyPlayer() + Rand.NextVector2(16 * 80), StarverBoss.CriticalLevel + 2001);
						}
						break;
					#endregion
					#region 11(Victory)
					case 11:
						EndTrial = false;
						StarverPlayer.All.SendMessage("这是...赢了?", Color.Aqua);
						EndTrialProcess = 0;
						break;
						#endregion
				}
			}
			#endregion
		}
		#endregion
		#region OnKilled
		internal static void OnKilled(NpcKilledEventArgs args)
		{
			if (StarverBoss.AliveBoss < 0)
			{
				return;
			}
			else
			{
				foreach(StarverBoss boss in Bosses)
				{
					if (boss.Active && args.npc.whoAmI == boss.Index)
					{
						boss.LifeDown();
						return;
					}
				}
			}
		}
		#endregion
		#region OnDrop
		internal static void OnDrop(NpcLootDropEventArgs args)
		{
			if (StarverBoss.AliveBoss < 0)
			{
				return;
			}
			else
			{
				foreach (StarverBoss boss in Bosses)
				{
					if (boss.Active && args.NpcArrayIndex == boss.Index)
					{
						args.Handled = true;
						return;
					}
				}
			}
		}
		#endregion
		#endregion
		#region Command
		internal static void BossCommand(CommandArgs args)
		{
			if(args.Parameters.Count < 1)
			{
				int i = 0;
				args.Player.SendInfoMessage("临界等级: {0}", StarverBoss.CriticalLevel);
				foreach(StarverBoss boss in Bosses)
				{
					args.Player.SendInfoMessage("{0}: {1}", ++i, boss.GetType().Name);
				}
			}
			else
			{
				
				int idx;
				int lvl;
				if (args.Parameters[0] == "*")
				{
					idx = -1;
				}
				else
				{
					try
					{
						idx = int.Parse(args.Parameters[0]);
					}
					catch
					{
						idx = 0;
					}
				}
					try
					{
						lvl = int.Parse(args.Parameters[1]);
					}
					catch
					{
						lvl = StarverBoss.CriticalLevel;
					}
				if (idx == 0)
				{
					foreach (StarverBoss boss in Bosses)
					{
						boss.KillMe();
					}
					StarverBoss.AliveBoss = 0;
				}
				else if(idx == -1)
				{
					foreach(var boss in Bosses)
					{
						try
						{
							boss.Spawn(Rand.NextVector2(16 * 15) + args.TPlayer.Center, lvl);
						}
						catch(Exception e)
						{
							args.Player.SendErrorMessage(e.ToString());
						}
					}
				}
				else
				{
					idx--;
					if (idx >= Bosses.Length)
					{
						args.Player.SendErrorMessage("参数错误");
					}
					try
					{
						Bosses[idx].Spawn(Rand.NextVector2(16 * 15) + args.TPlayer.Center, lvl);
					}
					catch (Exception e)
					{
						args.Player.SendErrorMessage(e.ToString());
					}
				}

			}
		}
		#endregion
		#region EndTrial
		#region SummonTower
		private void SummonTower(int MainTower)
		{
			#region TransformTower
			foreach (var NPC in Main.npc)
			{
				if (!NPC.active)
				{
					continue;
				}
				if (NPC.type == NPCID.LunarTowerNebula ||
					NPC.type == NPCID.LunarTowerSolar ||
					NPC.type == NPCID.LunarTowerStardust ||
					NPC.type == NPCID.LunarTowerVortex)
				{
					NPC.SetDefaults(MainTower);
					NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, null, NPC.whoAmI);
				}
				if (NPC.type == MainTower)
				{
					NPC.dontTakeDamage = true;
				}
			}
			#endregion
			#region Shield
			switch (MainTower)
			{
				case NPCID.LunarTowerVortex:
					NPC.ShieldStrengthTowerVortex = 600;
					break;
				case NPCID.LunarTowerStardust:
					NPC.ShieldStrengthTowerStardust = 600;
					break;
				case NPCID.LunarTowerNebula:
					NPC.ShieldStrengthTowerNebula = 600;
					break;
				case NPCID.LunarTowerSolar:
					NPC.ShieldStrengthTowerSolar = 600;
					break;
			}
			#endregion
		}
		#endregion
		#region TowerCanTakeDamage
		private void TowerTakeDamage()
		{
			foreach(var NPC in Main.npc)
			{
				if (!NPC.active)
				{
					continue;
				}
				if (NPC.type == NPCID.LunarTowerNebula ||
					NPC.type == NPCID.LunarTowerSolar || 
					NPC.type == NPCID.LunarTowerStardust || 
					NPC.type == NPCID.LunarTowerVortex)
				{
					NPC.dontTakeDamage = false;
				}
			}
		}
		#endregion
		#region KillTower
		private void KillTower()
		{
			foreach (var NPC in Main.npc)
			{
				if (!NPC.active)
				{
					continue;
				}
				if (NPC.type == NPCID.LunarTowerNebula ||
					NPC.type == NPCID.LunarTowerSolar ||
					NPC.type == NPCID.LunarTowerStardust ||
					NPC.type == NPCID.LunarTowerVortex)
				{
					NPC.active = false;
				}
			}
			NPC.MoonLordCountdown = 0;
		}
		#endregion
		#region SelectLuckyPlayer
		private Vector2 SelectLuckyPlayer()
		{
			Vector2 pos = default;
			foreach(var ply in Starver.Players)
			{
				if(ply is null || ply.Active)
				{
					continue;
				}
				pos = ply.Center;
				break;
			}
			return pos;
		}
		#endregion
		#endregion
	}
}
