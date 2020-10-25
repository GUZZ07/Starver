using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Starvers.PlayerBoosts;
using Starvers.PlayerBoosts.Skills;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace Starvers
{
	public class StarverPlayer
	{
		#region Constsnts
		private const int mpCostToUseWeapon = 2;
		#endregion
		#region Types
		#region Guest
		private class GuestPlayer : StarverPlayer
		{
			private static PlayerData NoneData = new PlayerData(-4);
			public override PlayerData Data => NoneData;
			public override double DamageIndex => 0;
			public GuestPlayer(int idx)
			{
				Index = idx;
			}
			public override void SaveData()
			{

			}
			public override void OnLeave()
			{

			}
			public override void OnStrikeNpc(NpcStrikeEventArgs args)
			{
				args.Handled = true;
			}
			public override void Update()
			{

			}
		}
		#endregion
		#endregion
		#region RawDatas
		public int Index
		{
			get;
			protected set;
		}
		public virtual PlayerData Data { get; }
		public virtual Player TPlayer
		{
			get => Main.player[Index];
		}
		public virtual TSPlayer TSPlayer
		{
			get => TShock.Players[Index];
		}
		public int Timer
		{
			get;
			protected set;
		}
		#endregion
		#region From TPlayer
		public string Name
		{
			get => TPlayer.name;
		}
		public bool Alive
		{
			get => !DeadOrGhost;
		}
		public bool DeadOrGhost
		{
			get => TPlayer.DeadOrGhost;
		}
		public int ItemUseDelay { get; set; }
		public Item HeldItem => TPlayer.inventory[TPlayer.selectedItem];
		public double ItemUseAngle
		{
			get
			{
				double angle = TPlayer.itemRotation;
				if (TPlayer.direction == -1)
				{
					angle += Math.PI;
				}
				return angle;
			}
		}
		public bool ControlUseItem => TPlayer.controlUseItem;
		public NetInventory Inventory
		{
			get;
			private set;
		}
		#region Life
		public int Life
		{
			get
			{
				return TPlayer.statLife;
			}
			set
			{
				TPlayer.statLife = value;
				SendData(PacketTypes.PlayerHp, string.Empty, Index);
			}
		}
		public int LifeMax
		{
			get
			{
				return TPlayer.statLifeMax;
			}
			set
			{
				TPlayer.statLifeMax = value;
				SendData(PacketTypes.PlayerHp, string.Empty, Index);
			}
		}
		#endregion
		#region Center
		public Vector2 Center
		{
			get
			{
				return TPlayer.Center;
			}
			set
			{
				TPlayer.Center = value;
				SendData(PacketTypes.PlayerUpdate, "", Index);
			}
		}
		#endregion
		#region Position
		public Vector2 Position
		{
			get
			{
				return TPlayer.position;
			}
			set
			{
				TPlayer.position = value;
				SendData(PacketTypes.PlayerUpdate, "", Index);
			}
		}
		#endregion
		#region Velocity
		public Vector2 Velocity
		{
			get
			{
				return TPlayer.velocity;
			}
			set
			{
				TPlayer.velocity = value;
				SendData(PacketTypes.PlayerUpdate, "", Index);
			}
		}
		#endregion
		#endregion
		#region Private Fields
		private int skillCheckDelay;
		/// <summary>
		/// mp回复速率
		/// </summary>
		private double mpRegen;
		private double mpRegenFromNoMove;
		private double mpRegenFromNoUseItem;

		private int noMove;
		private int noUseItem;

		private int noRegenMP;
		#endregion
		public bool IgnoreCD { get; set; }

		public int AvalonGradationTime { get; set; }

		public SkillIDs LastSkill { get; set; }

		public int Exp
		{
			get => Data.Exp;
			set => OnExpChange(Exp, value);
		}

		public int Level
		{
			get => Data.Level;
			set => OnLevelChange(Level, value);
		}

		public int MP
		{
			get => Data.MP;
			set => Data.MP = value;
		}

		public int MPMax
		{
			get;
			private set;
		}

		public PlayerSkillData[] Skills { get; }

#warning 还没做
		public bool IsVip { get; set; }
#warning 在怪物强化之前得削弱一下玩家
		public virtual double DamageIndex
		{
			get
			{
				int level = Level;
				var value = level switch
				{
					_ when level < 100 => 1 + 0.015 * level,
					_ when 100 <= level && level < 1000 => 1 + 1.5 + Math.Log(level / 100, 2),
					// _ when 1000 <= level && level < 10000 => 1 + 1.5 + Math.Pow(level, 0.2) + 3 * Math.Pow(level / 10000, 10) - 4
					_ when 1000 <= level && level < 10000 => 1.821928094887362 + Math.Pow(level, 0.2) + 3 * Math.Pow(level / 10000, 10),
					_ when 10000 <= level && level < 100000 => 11.131501539689296 + Math.Pow(Math.Log10(level) - 3.7, Math.Log(level / 1000, 2) + 1),
					_ => 20
				};
				return value;
			}
		}
		public virtual double KnockBackIndex
		{
			get => 1 + 3 * Math.Log10(Math.Sqrt(Level));
		}
		#region Ctor
		protected StarverPlayer()
		{

		}

		public StarverPlayer(int index)
		{
			Index = index;
			Skills = new PlayerSkillData[Starver.MaxSkillSlot];
			Inventory = new NetInventory(this);
			try
			{
				Data = Starver.Instance.PlayerDatas.GetData(TSPlayer.Account.ID);
			}
			catch (KeyNotFoundException)
			{
				Data = new PlayerData(TSPlayer.Account.ID);
				Starver.Instance.PlayerDatas.SaveData(Data);
			}
			Data.GetSkillDatas(Skills);
			MPMax = CalcMPMax(Level);
		}
		#endregion
		#region SaveData
		public virtual void SaveData()
		{
			Data.SetSkillDatas(Skills);
			Starver.Instance.PlayerDatas.SaveData(Data);
		}
		#endregion
		#region UpgradeExp
		public int CalcUpgradeExp()
		{
			int divide = IsVip ? 3 : 1;
			return CalcUpgradeExp(Level) / divide;
		}
		#endregion
		#region TryUpgrade
		public void TryUpgrade()
		{
			int divide = IsVip ? 3 : 1;
			var (exp, lvl) = (Exp, Level);
			int expNeed = CalcUpgradeExp(lvl) / divide;
			while (exp >= expNeed)
			{
				exp -= expNeed;
				lvl++;
				expNeed = CalcUpgradeExp(lvl) / divide;
			}
			(Exp, Level) = (exp, lvl);
		}
		#endregion
		#region Cast
		public static explicit operator string(StarverPlayer player)
		{
			return player?.Name ?? "";
		}
		public static implicit operator TSPlayer(StarverPlayer player)
		{
			return player?.TSPlayer;
		}
		public static implicit operator Player(StarverPlayer player)
		{
			return player?.TPlayer;
		}
		public static explicit operator int(StarverPlayer player)
		{
			return player.Index;
		}
		#endregion
		#region OnXXChange
		private void OnExpChange(int oldValue, int newValue)
		{
			if (Level > Starver.Instance.Config.AutoUpgradeLevel)
			{
				int divide = IsVip ? 3 : 1;
				var (exp, lvl) = ((long)newValue, Level);
				int expNeed = CalcUpgradeExp(lvl) / divide;
				while (exp >= expNeed)
				{
					exp -= expNeed;
					lvl++;
					expNeed = CalcUpgradeExp(lvl) / divide;
				}
				Data.Exp = (int)exp;
				Level = lvl;
			}
			else
			{
				Data.Exp = newValue;
			}
		}
		private void OnLevelChange(int oldValue, int newValue)
		{
			if (oldValue == newValue)
			{
				return;
			}
			Data.Level = newValue;
			MPMax = CalcMPMax(Level);
			foreach (var skill in Starver.Instance.Skills)

			{
				if ((skill.LevelNeed ?? 0) == Data.Level)
				{
					SendText($"你已到达{skill}要求最低等级！", 255, 215, 0);
				}
			}


		}
		#endregion
		#region BindSkill
		public void UnBind(int slot)
		{
			Skills[slot].ID = null;
			SaveData();
		}
		public void BindSkill(int slot, StarverSkill skill, bool byProj, int bindId)
		{
			Skills[slot] = new PlayerSkillData
			{
				ID = skill.ID,
				BindByProj = byProj,
				BindID = (short)bindId,
				CD = Skills[slot].CD
			};
			SaveData();
		}
		#endregion
		#region Events
		private void OnUseItem(Item item)
		{
			if (400 <= LifeMax && LifeMax < 3000)
			{
				var slot = Inventory[TPlayer.selectedItem];
				switch (item.type)
				{
					case ItemID.LifeFruit:
						LifeMax = Math.Min(3000, LifeMax + 100);
						slot.Stack--;
						break;
					case ItemID.LifeCrystal:
						LifeMax = Math.Min(3000, LifeMax + 20);
						slot.Stack--;
						break;
				}
			}
			if (item.damage > 0 && item.axe + item.pick == 0)
			{
				noUseItem = 0;
				var fromUseTime = Math.Max(1, Math.Log(16.0 / item.useTime - 1 + Math.E));
				var fromDamage =Math.Log10(10 + TPlayer.GetWeaponDamage(item) / 10.0);
				var cost = CalcMPCost(Level) * fromDamage / fromUseTime;
				// SendBlueText($"{cost}");
				MP = Math.Max(0, (int)(MP - cost));
			}
			if (skillCheckDelay != 0)
			{
				return;
			}
			for (int i = 0; i < Skills.Length; i++)
			{
				ref var skill = ref Skills[i];
				if (skill.ID != null && skill.CD == 0)
				{
					if (skill.IsBindTo(item))
					{
						skill.Release(this);
						skillCheckDelay += 20;
					}
				}
			}
		}
		public virtual void OnNewProj(GetDataHandlers.NewProjectileEventArgs args)
		{
			if (skillCheckDelay != 0)
			{
				return;
			}
			for (int i = 0; i < Skills.Length; i++)
			{
				ref var skill = ref Skills[i];
				if (skill.ID != null && skill.CD == 0)
				{
					if (skill.IsBindTo(Main.projectile[args.Index]))
					{
						var vel = (Vector)args.Velocity;
						vel.Length = Starver.SpearSpeed;
						skill.Release(this, vel);
						skillCheckDelay += 20;
					}
				}
			}
		}
		public virtual void OnGetData(GetDataEventArgs args)
		{
			switch (args.MsgID)
			{
				case PacketTypes.PlayerAnimation:
					{
						using var stream = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
						using var reader = new BinaryReader(stream);
						int index = reader.ReadByte();
						index = Index;
						Player player3 = TPlayer;
						var itemRotation = reader.ReadSingle();
						int itemAnimation = reader.ReadInt16();
						TPlayer.itemRotation = itemRotation;
						TPlayer.itemAnimation = itemAnimation;
						TPlayer.channel = HeldItem.channel;
						if (ItemUseDelay == 0 && ControlUseItem)
						{
							OnUseItem(HeldItem);
							ItemUseDelay += HeldItem.useTime;
						}
						args.Handled = true;
						break;
					}
			}
		}
		public virtual void OnLeave()
		{
			SaveData();
		}
		public virtual void OnStrikeNpc(NpcStrikeEventArgs args)
		{
			var boss = Starver.Instance.Bosses.TryGetBoss(args.Npc);
			if (boss != null)
			{
				boss.Strike(this, args.Damage, args.KnockBack, args.HitDirection, args.Critical);
				return;
			}
			#region Normal
			var raw = args.Damage;
			var index = Math.Sqrt(Math.Sqrt(MP / 200.0)) * 1.2;

			args.Damage = (int)Math.Max(raw, args.Damage * DamageIndex * index);
			var realdamage = (int)Main.CalculateDamageNPCsTake(args.Damage, args.Npc.defense);
			args.Npc.SendCombatText(realdamage.ToString(), Starver.DamageColor);
			var realNPC = args.Npc.realLife > 0 ? Main.npc[args.Npc.realLife] : args.Npc;
			var expGet = Math.Min(realdamage, realNPC.life);
			if (!args.Npc.SpawnedFromStatue && args.Npc.type != NPCID.TargetDummy && expGet > 0)
			{
				Exp += expGet;
			}
			#endregion
		}
		public virtual void PostUpdate()
		{

		}
		public virtual void Update()
		{
			Timer++;
			#region Status Text
			static string CalcMPStar(int MP)
			{
				// ★ 200mp
				// ◆ 50mp
				// ▲ 5mp
				// ● 1mp
				var mp = MP;
				var mp200 = mp / 200;
				mp %= 200;
				var mp50 = mp / 50;
				mp %= 50;
				var mp5 = mp / 5;
				mp %= 5;
				var text = new string('★', mp200)
					+ new string('◆', mp50) + "    \n"
					+ new string('▲', mp5)
					+ new string('●', mp) + "    ";
				return text;
			}
			if (Timer % 60 == 0)
			{
				if (Timer % (60 * 6) >= 3 * 60)
				{
					SendStatusText($@"Level: {Level}      Exp:{Exp}/{CalcUpgradeExp()}
MP({MP}/{MPMax})
{CalcMPStar(MP)}");
				}
				else
				{
					SendStatusText($@"技能状态
    {Skills[0]}
    {Skills[1]}
    {Skills[2]}
    {Skills[3]}
    {Skills[4]}");
				}
			}
			#endregion
			#region Timers
			if (ItemUseDelay > 0)
			{
				ItemUseDelay--;
			}
			if (skillCheckDelay > 0)
			{
				skillCheckDelay--;
			}
			noUseItem++;
			if (Velocity.Length() < 2)
			{
				noMove++;
			}
			else
			{
				noMove = 0;
			}
			#endregion
			#region MP Regeneration
			noRegenMP = Math.Max(0, noRegenMP - 1);
			if (noRegenMP == 0 && Timer % 60 == 0)
			{
				mpRegenFromNoMove = Math.Min(15, noMove / 60.0);
				mpRegenFromNoUseItem = Math.Min(5, noUseItem / 60.0);

				mpRegen = 0;
				mpRegen += CalcMPRegen(Level);// SendBlueText($"{mpRegenFromNoMove}, {mpRegenFromNoUseItem}, {CalcMPRegen(Level)}, {mpRegen}");
				mpRegen += mpRegenFromNoMove + mpRegenFromNoUseItem;
				mpRegen *= (1 + (mpRegenFromNoMove + mpRegenFromNoUseItem) / 5);

				MP = (int)Math.Min(MPMax, MP + mpRegen);
			}
			#endregion
			#region ItemUse
			static bool SpecialItem(Item item)
			{
				switch (item.type)
				{
					case ItemID.Phantasm:
					case ItemID.VortexBeater:
					case ItemID.LaserMachinegun:
					case ItemID.DD2PhoenixBow:
						return true;
				}
				return false;
			}
			if (HeldItem.useStyle == ItemUseStyleID.HoldUp || HeldItem.useStyle == ItemUseStyleID.Swing || SpecialItem(HeldItem))
			{
				if (ItemUseDelay == 0 && ControlUseItem)
				{
					ItemUseDelay += HeldItem.useTime;
					OnUseItem(HeldItem);
				}
			}
			PlayerBoosts.Skills.AvalonGradation.Update(this);
			#endregion
			#region UpdateSkillCD
			for (int i = 0; i < Starver.MaxSkillSlot; i++)
			{
				if (Skills[i].ID == null)
				{
					continue;
				}
				if (!Skills[i].Skill.ForceCD && IgnoreCD)
				{
					Skills[i].CD = 0;
				}
				else if (Skills[i].CD > 0)
				{
					Skills[i].CD--;
				}
			}
			#endregion
			#region NPC Damage
			foreach (var npc in Main.npc)
			{
				if (!npc.active || npc.friendly || npc.damage <= 0)
				{
					continue;
				}
				if (TPlayer.getRect().Intersects(npc.getRect()))
				{
					var rawdamage = Starver.Instance.RecalcDamage(npc);
					var factor = (double)rawdamage / npc.damage;
					var damage = (int)Main.CalculateDamagePlayersTake(rawdamage, (int)(TPlayer.statDefense * factor)) - npc.damage;
					Hurt(damage, PlayerDeathReason.ByNPC(npc.whoAmI), 0);
				}
			}
			#endregion
		}
		#endregion
		#region Utilities
		#region BlockMPRegen
		public void BlockMPRegen(int timeInTick)
		{
			noRegenMP += timeInTick;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return Name;
		}
		#endregion
		#region Projs
		#region FromPolar
		/// <summary>
		/// 极坐标获取角度
		/// </summary>
		/// <param name="angle">所需角度(弧度)</param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public Vector FromPolar(double angle, float radius)
		{
			return Vector.FromPolar(angle, radius);
		}
		#endregion
		#region NewProj
		/// <summary>
		/// 生成弹幕
		/// </summary>
		public int NewProjNoBC(Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack = 20f, float ai0 = 0, float ai1 = 0)
		{
			return Utils.NewProjNoBC(position, velocity, Type, Damage, KnockBack, Index, ai0, ai1);
		}
		/// <summary>
		/// 生成弹幕
		/// </summary>
		public int NewProj(Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack = 20f, float ai0 = 0, float ai1 = 0, int extraUpdates = 0)
		{
			return Utils.NewProj(position, velocity, Type, Damage, KnockBack, Index, ai0, ai1, extraUpdates);
		}
		/// <summary>
		/// 生成弹幕
		/// </summary>
		public int NewProj(Vector2 velocity, int Type, int Damage, float KnockBack = 20f, float ai0 = 0, float ai1 = 0, int extraUpdates = 0)
		{
			return Utils.NewProj(Center, velocity, Type, Damage, KnockBack, Index, ai0, ai1, extraUpdates);
		}
		#endregion
		#region ProjCircle
		/// <summary>
		/// 弹幕圆
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="r"></param>
		/// <param name="speed">向外速率</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		public void ProjCircle(Vector2 Center, float r, float speed, int Type, int number, int Damage, float ai0 = 0, float ai1 = 0)
		{
			double averagerad = Math.PI * 2 / number;
			for (int i = 0; i < number; i++)
			{
				NewProj(Center + FromPolar(averagerad * i, r), FromPolar(averagerad * i, speed), Type, Damage, 4f, ai0, ai1);
			}
		}
		/// <summary>
		/// 弹幕圆
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="angle">偏转角</param>
		/// <param name="r"></param>
		/// <param name="speed">速率</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		public void ProjCircleEx(Vector2 Center, double angle, float r, float speed, int Type, int number, int Damage, float ai0 = 0, float ai1 = 0)
		{
			double averagerad = Math.PI * 2 / number;
			for (int i = 0; i < number; i++)
			{
				NewProj(Center + FromPolar(angle + averagerad * i, r), FromPolar(angle + averagerad * i, speed), Type, Damage, 4f, ai0, ai1);
			}
		}

		/// <summary>
		/// 弹幕圆
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="angle">偏转角</param>
		/// <param name="r"></param>
		/// <param name="speed">速率</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		public void ProjCircleExNoBC(Vector2 Center, double angle, float r, Action<Projectile> action, int Type, int number, int Damage, float ai0 = 0, float ai1 = 0)
		{
			double averagerad = Math.PI * 2 / number;
			for (int i = 0; i < number; i++)
			{
				var idx = NewProjNoBC(Center + FromPolar(angle + averagerad * i, r), default, Type, Damage, 4f, ai0, ai1);
				action(Main.projectile[idx]);
			}
		}

		/// <summary>
		/// 弹幕圆
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="angle">偏转角</param>
		/// <param name="r"></param>
		/// <param name="velocity">速度</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		public void ProjCircleEx(Vector2 Center, double angle, float r, Vector2 velocity, int Type, int number, int Damage, float ai0 = 0, float ai1 = 0)
		{
			double averagerad = Math.PI * 2 / number;
			for (int i = 0; i < number; i++)
			{
				NewProj(Center + FromPolar(angle + averagerad * i, r), velocity, Type, Damage, 4f, ai0, ai1);
			}
		}

		/// <summary>
		/// 弹幕圆
		/// </summary>
		/// <param name="Center"></param>
		/// <param name="r"></param>
		/// <param name="speed">向外速率</param>
		/// <param name="Type"></param>
		/// <param name="number">弹幕总数</param>
		public int[] ProjCircleRet(Vector2 Center, float r, float speed, int Type, int number, int Damage, float ai0 = 0, float ai1 = 0)
		{
			double averagerad = Math.PI * 2 / number;
			int[] arr = new int[number];
			for (int i = 0; i < number; i++)
			{
				arr[i] = NewProj(Center + FromPolar(averagerad * i, r), FromPolar(averagerad * i, speed), Type, Damage, 4f, ai0, ai1);
			}
			return arr;
		}
		#endregion
		#region ProjSector
		/// <summary>
		/// 扇形弹幕
		/// </summary>
		/// <param name="Center">圆心</param>
		/// <param name="speed">向外速率</param>
		/// <param name="r">半径</param>
		/// <param name="interrad">中心半径的方向</param>
		/// <param name="rad">张角</param>
		/// <param name="Damage">伤害(带加成)</param>
		/// <param name="type"></param>
		/// <param name="num">数量</param>
		/// <param name="ai0"></param>
		/// <param name="ai1"></param>
		public void ProjSector(Vector2 Center, float speed, float r, double interrad, double rad, int Damage, int type, int num, float ai0 = 0, float ai1 = 0)
		{
			double start = interrad - rad / 2;
			double average = rad / num;
			for (int i = 0; i < num; i++)
			{
				NewProj(Center + FromPolar(start + i * average, r), FromPolar(start + i * average, speed), type, Damage, 4f, ai0, ai1);
			}
		}
		#endregion
		#region ProjLine
		/// <summary>
		/// 制造速度平行的弹幕直线
		/// </summary>
		/// <param name="Begin">起点</param>
		/// <param name="End">终点</param>
		/// <param name="Vel">速度</param>
		/// <param name="num">数量</param>
		/// <param name="Damage"></param>
		/// <param name="type"></param>
		/// <param name="ai0"></param>
		/// <param name="ai1"></param>
		public void ProjLine(Vector2 Begin, Vector2 End, Vector2 Vel, int num, int Damage, int type, float ai0 = 0, float ai1 = 0)
		{
			Vector2 average = End - Begin;
			average /= num;
			for (int i = 0; i < num; i++)
			{
				NewProj(Begin + average * i, Vel, type, Damage, 3f, ai0, ai1);
			}
		}
		/// <summary>
		/// 制造速度平行的弹幕直线
		/// </summary>
		/// <param name="Begin">起点</param>
		/// <param name="End">终点</param>
		/// <param name="Vel">速度</param>
		/// <param name="num">数量</param>
		/// <param name="Damage"></param>
		/// <param name="type"></param>
		/// <param name="ai0"></param>
		/// <param name="ai1"></param>
		public int[] ProjLineReturns(Vector2 Begin, Vector2 End, Vector2 Vel, int num, int Damage, int type, float ai0 = 0, float ai1 = 0)
		{
			int[] arr = new int[num];
			Vector2 average = End - Begin;
			average /= num;
			for (int i = 0; i < num; i++)
			{
				arr[i] = NewProj(Begin + average * i, Vel, type, Damage, 3f, ai0, ai1);
			}
			return arr;
		}
		#endregion
		#endregion
		#region Buff
		public void AddBuffIfNot(int type, int time = 60 * 60)
		{
			if (!HasBuff(type))
			{
				SetBuff(type, time);
			}
		}
		public void SetBuff(int type, int time = 3600, bool bypass = false)
		{
			TSPlayer.SetBuff(type, time, bypass);
		}
		public bool HasBuff(int type)
		{
			return FindBuffIndex(type) != -1;
		}
		public void RemoveBuff(int type)
		{
			int idx = FindBuffIndex(type);
			if (idx != -1)
			{
				TPlayer.buffTime[idx] = 0;
				TPlayer.buffType[idx] = 0;
				SendData(PacketTypes.PlayerBuff, "", Index);
			}
		}
		public int FindBuffIndex(int type)
		{
			int idx = -1;
			for (int i = 0; i < TPlayer.buffType.Length; i++)
			{
				if (TPlayer.buffType[i] == type)
				{
					idx = i;
					break;
				}
			}
			return idx;
		}
		#endregion
		#region Sends
		public void SendData(PacketTypes msgType, string text = "", int number = 0, float number2 = 0, float number3 = 0, float number4 = 0, int number5 = 0)
		{
			NetMessage.SendData((int)msgType, Index, -1, NetworkText.FromLiteral(text), number, number2, number3, number4, number5);
		}
		public void SendText(string text, Color color)
		{
			TSPlayer.SendMessage(text, color);
		}
		public void SendCombatText(string text, Color color, bool visableToOthers = true)
		{
			TPlayer.SendCombatText(text, color, visableToOthers ? -1 : Index);
		}
		public void SendText(string text, byte r, byte g, byte b)
		{
			TSPlayer.SendMessage(text, r, g, b);
		}
		public void SendBlueText(string text)
		{
			SendText(text, 0, 0, 255);
		}
		public void SendErrorText(string text)
		{
			SendText(text, 255, 0, 0);
		}
		private static readonly string EndLine19 = new string('\n', 19);
		private static readonly string EndLine5 = new string('\n', 5);
		public void SendStatusText(string text)
		{
			text = EndLine19 + text + EndLine5;
			SendData(PacketTypes.Status, text);
		}
		#endregion
		#region Hurt
		public void Hurt(int damage, PlayerDeathReason deathreason, int dir = 0)
		{
			NetMessage.SendPlayerHurt(Index, deathreason, damage, dir, false, false, 0);
		}
		#endregion
		#region Heal
		public void Heal(int health)
		{
			TSPlayer.Heal(health);
		}
		#endregion
		#endregion
		#region Statics
		public static StarverPlayer GetGuest(int idx) => new GuestPlayer(idx);
		public static int CalcUpgradeExp(int lvl)
		{
			if (lvl < 1000)
			{
				return (int)(lvl * 10.5f);
			}
			else if (lvl < 2500)
			{
				return (int)(lvl * 15.5f);
			}
			else if (lvl < (int)1e4)
			{
				return Math.Min(150000, (int)(lvl * Math.Log(lvl)));
			}
			else if (lvl < (int)1e5)
			{
				return 150000;
			}
			else
			{
				return 25 * 25 * 25 * 25;
			}
		}
		/// <summary>
		/// 反比例模型，f(0)=100，f(+∞) -> 2100
		/// </summary>
		/// <param name="lvl"></param>
		/// <returns></returns>
		public static int CalcMPMax(int lvl)
		{
			return (int)(5 * (-80000.0 / (lvl / 4.0 + 200) + 400)) + 100;
		}
		/// <summary>
		/// 单位: MP每秒
		/// </summary>
		/// <param name="lvl"></param>
		/// <returns></returns>
		public static double CalcMPRegen(int lvl)
		{
			static double f(double x)
			{
				var sign = Math.Sign(x);
				x = Math.Abs(x);
				return sign * Math.Pow(x, 1.0 / 7);
			}
			return (100 * f(lvl - 3000) - 100 * f(-3000)) / 7.5;
		}
		public static double CalcMPCost(int lvl)
		{
			return Math.Log10(lvl + 10) * mpCostToUseWeapon / 1.5;
		}
		#endregion
	}

	#region SkillStorage
	public struct SkillStorage
	{
		public byte ID { get; set; }
		public bool BindByProj { get; set; }
		public short BindID { get; set; }

		public static implicit operator PlayerSkillData(in SkillStorage data)
		{
			return new PlayerSkillData
			{
				ID = data.ID,
				BindByProj = data.BindByProj,
				BindID = data.BindID
			};
		}
	}
	#endregion
	#region PlayerSkillData
	public struct PlayerSkillData
	{
		public int CD { get; set; }
		public byte? ID { get; set; }
		public bool BindByProj { get; set; }
		public short BindID { get; set; }

		public StarverSkill Skill => ID.HasValue ? Starver.Instance.Skills[(byte)ID] : null;

		#region Cast
		public static implicit operator SkillStorage?(in PlayerSkillData data)
		{
			if (data.ID == null)
			{
				return null;
			}
			return new SkillStorage
			{
				ID = (byte)data.ID,
				BindByProj = data.BindByProj,
				BindID = data.BindID
			};
		}
		#endregion
		#region Release
		public void Release(StarverPlayer player)
		{
			Release(player, Vector.FromPolar(player.ItemUseAngle, Starver.SpearSpeed));
		}
		public void Release(StarverPlayer player, Vector vel)
		{
			if (Skill is UltimateSkill && player.MP < player.MPMax / 2)
			{
				player.SendCombatText("只有在mp大于50%时才能发动终极技能", Color.Blue);
				return;
			}
			if (Skill.MPCost > player.MP)
			{
				player.SendCombatText("MP不足", Color.HotPink, false);
				return;
			}
			Skill.Release(player, vel);
			CD += Skill.CD;
			player.MP -= Skill.MPCost;
			player.LastSkill = (SkillIDs)ID;
		}
		#endregion
		#region IsBindTo
		public bool IsBindTo(Item item)
		{
			return !BindByProj && BindID == item.type;
		}
		public bool IsBindTo(Projectile proj)
		{
			return BindByProj && BindID == proj.type;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			if (ID == null)
			{
				return string.Empty;
			}
			if (Skill.Banned)
			{
				return $"{Skill}(已被禁用)";
			}
			if (CD > 0)
			{
				return $"{Skill}({CD / 60})";
			}
			return $"{Skill}(MP: {Skill.MPCost})";
		}
		#endregion
	}
	#endregion
}
