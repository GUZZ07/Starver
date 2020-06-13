using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace Starver
{
	public class StarverPlayer
	{
		public int Index { get; }
		public virtual PlayerData Data { get; }
		public virtual Player TPlayer => Main.player[Index];
		public virtual TSPlayer TSPlayer => TShock.Players[Index];

		public int Level
		{
			get => Data.Level;
			set => Data.Level = value;
		}

		public double DamageIndex => 1 + Level * 0.01;

		protected StarverPlayer()
		{

		}

		public StarverPlayer(int index)
		{
			Index = index;
			Data = Starver.Instance.PlayerDatas.GetData(TSPlayer.Account.ID);
		}

		public virtual void Save()
		{
			Starver.Instance.PlayerDatas.SaveData(Data);
		}
	}
}
