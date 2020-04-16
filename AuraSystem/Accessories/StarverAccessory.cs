using Starvers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace Starvers.AuraSystem.Accessories
{
	public abstract class StarverAccessory
	{
		public int ItemType { get; }
		public int LevelNeed { get; }

		protected StarverAccessory(int itemType, int levelNeed)
		{
			ItemType = itemType;
			LevelNeed = levelNeed;
		}

		public abstract void UpdateAccessory(StarverPlayer player);
		public virtual bool CanUseAccessory(StarverPlayer player)
		{
			return player.Level >= LevelNeed;
		}
		public virtual void OnDamaged(StarverPlayer player, int damage, bool ctit, bool pvp)
		{

		}
		public virtual void OnUseItem(StarverPlayer player)
		{

		}
		public virtual void PreStrike(StarverPlayer player, NPCStrikeEventArgs args)
		{
			
		}
	}
}
