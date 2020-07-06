using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Starvers.Enemies.Bosses
{
	public class BossManager
	{
		private StarverBoss[] bosses;

		public int Count
		{
			get => bosses.Length;
		}

		public StarverBoss this[int index]
		{
			get => bosses[index];
		}

		public BossManager()
		{
			LoadBosses();
		}

		public T GetBoss<T>() where T: StarverBoss
		{
			return BossInstance<T>.Instance;
		}

		public StarverBoss TryGetBoss(NPC npc)
		{
			return bosses.FirstOrDefault(boss => boss.Active && boss.Index == npc.whoAmI);
		}

		public void Update()
		{
			foreach (var boss in bosses)
			{
				boss.AI();
			}
		}

		#region LoadBosses
		private void LoadBosses()
		{
			bosses = new[]
			{
				new EyeEx()
			};
			foreach (var boss in bosses)
			{
				var type = typeof(BossInstance<>).MakeGenericType(new[] { boss.GetType() });
				type.GetField("Instance").SetValue(null, boss);
			}
		}
		#endregion
		#region BossInstance
		private class BossInstance<T> where T : StarverBoss
		{
			public static T Instance;
		}
		#endregion
	}
}
