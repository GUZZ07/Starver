using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace Starvers
{
	public class StarverPlayer
	{
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
		}
		#endregion
		public int Index { get; protected set; }
		public virtual PlayerData Data { get; }
		public virtual Player TPlayer => Main.player[Index];
		public virtual TSPlayer TSPlayer => TShock.Players[Index];
		#region From TPlayer
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
#warning 还没做
		public bool IsVip { get; set; }

		public virtual double DamageIndex => 1 + Level * 0.01;

		protected StarverPlayer()
		{
			
		}

		public StarverPlayer(int index)
		{
			Index = index;
			try
			{
				Data = Starver.Instance.PlayerDatas.GetData(TSPlayer.Account.ID);
			}
			catch(KeyNotFoundException)
			{
				Data = new PlayerData(TSPlayer.Account.ID);
				Starver.Instance.PlayerDatas.SaveData(Data);
			}
		}

		public virtual void SaveData()
		{
			Starver.Instance.PlayerDatas.SaveData(Data);
		}
		public int CalcUpgradeLevel()
		{
			int divide = IsVip ? 3 : 1;
			return CalcUpgradeLevel(Level) / divide;
		}
		#region OnXXChange
		private void OnExpChange(int oldValue, int newValue)
		{
			if (Level > Starver.Instance.Config.AutoUpgradeLevel)
			{
				int divide = IsVip ? 3 : 1;
				var (exp, lvl) = (newValue, Level);
				int expNeed = CalcUpgradeLevel(lvl) / divide;
				while (exp >= expNeed)
				{
					exp -= expNeed;
					lvl++;
					expNeed = CalcUpgradeLevel(lvl) / divide;
				}
				(Exp, Level) = (exp, lvl);
			}
		}
		private void OnLevelChange(int oldValue,int newValue)
		{
			Data.Level = newValue;
		}
		#endregion
		#region Events
		private void OnUseItem()
		{

		}
		public virtual void OnGetData(GetDataEventArgs args)
		{

		}
		public virtual void OnLeave()
		{

		}
		public virtual void OnStrikeNpc(NpcStrikeEventArgs args)
		{
			args.Damage = (int)(args.Damage * DamageIndex);
			args.Npc.SendCombatText(args.Damage.ToString(), Starver.DamageColor);
			var realdamage = (int)Main.CalculateDamageNPCsTake(args.Damage, args.Npc.defense);
			Exp += realdamage;
		}
		public virtual void Update()
		{
			SendStatusText($"Level: {Level}\nExp:{Exp}");
		}
		#endregion
		#region Utilities
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
		public void SendText(string text, byte r, byte g, byte b)
		{
			TSPlayer.SendMessage(text, r, g, b);
		}
		public void SendBlueText(string text)
		{
			SendText(text, 0, 0, 255);
		}
		private static readonly string EndLine19 = new string('\n', 19);
		private static readonly string EndLine20 = new string('\n', 20);
		public void SendStatusText(string text)
		{
			text = EndLine19 + text + EndLine20;
			SendData(PacketTypes.Status, text);
		}
		#endregion
		#endregion
		#region Statics
		public static StarverPlayer GetGuest(int idx) => new GuestPlayer(idx);
		public static int CalcUpgradeLevel(int lvl)
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
		#endregion
	}
}
