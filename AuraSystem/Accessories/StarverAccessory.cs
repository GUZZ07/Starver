using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
		public virtual void OnUseItem(StarverPlayer player)
		{

		}
	}
}
