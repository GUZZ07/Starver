using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.PlayerBoosts.Items
{
	public class ItemBoostManager
	{
		private ItemBoost[] boosts;

		public int Count
		{
			get => boosts.Length;
		}

		public ItemBoostManager()
		{
			Load();
		}

		private void Load()
		{
			boosts = new ItemBoost[]
			{

			};
			Array.Sort(boosts, (l, r) => l.ItemType - r.ItemType);
			foreach (var boost in boosts)
			{
				var type = typeof(BoostInstance<>).MakeGenericType(new[] { boost.GetType() });
				type.GetField("Instance").SetValue(null, boost);
			}
		}

		public bool GetBoost(int itemID, out ItemBoost boost)
		{
			var comparer = Comparer<ItemBoost>.Create((l, r) => l.ItemType - r.ItemType);
			var index = Array.BinarySearch(boosts, new ItemBoost(itemID), comparer);
			if (index >= 0)
			{
				boost = boosts[index];
				return true;
			}
			boost = null;
			return false;
		}
		public ItemBoost GetBoost(int itemID)
		{
			var comparer = Comparer<ItemBoost>.Create((l,r)=>l.ItemType-r.ItemType);
			var index = Array.BinarySearch(boosts, new ItemBoost(itemID), comparer);
			if (index >= 0)
			{
				return boosts[index];
			}
			throw new KeyNotFoundException($"对应ID为{index}的物品强化未找到");
		}
		public T GetBoost<T>() where T : ItemBoost
		{
			return BoostInstance<T>.Instance;
		}

		#region BoostInstance
		private class BoostInstance<T> where T: ItemBoost
		{
			public static T Instance;
		}
		#endregion
	}
}
