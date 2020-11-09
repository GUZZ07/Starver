using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace Starvers.PlayerBoosts.Realms
{
	using Shapes;
	public class RealmManager
	{
		private Queue<IRealm> realmQueue;

		public ReadOnlyCollection<Type> RealmTypes
		{
			get;
		}

		public RealmManager()
		{
			realmQueue = new Queue<IRealm>();

			var types = new Type[]
			{
				typeof(ApoptoticRealm),
				typeof(ReflectingRealm<Circle>),
				typeof(BlindingRealm<Circle>)
			};
			RealmTypes = new ReadOnlyCollection<Type>(types);
		}

		public void Update()
		{
			int count = realmQueue.Count;
			while (count-- > 0)
			{
				var realm = realmQueue.Dequeue();
				if (realm.Active)
				{
					realm.Update();
					if (realm.Active)
					{
						realmQueue.Enqueue(realm);
					}
				}
			}
		}

		public void Foreach(Action<IRealm> action)
		{
			foreach(var realm in realmQueue)
			{
				action(realm);
			}
		}

		public void Add(IRealm realm)
		{
			realm.Begin();
			realmQueue.Enqueue(realm);
		}

		public void ShowTo(TSPlayer player)
		{
			for (int i = 0; i < RealmTypes.Count; i++)
			{
				var type = RealmTypes[i];
				player.SendInfoMessage($"{i}: {type.Name}");
			}
		}

		public void TriggerByCommand(CommandArgs args)
		{
			var player = args.Player;
			if (args.Parameters.Count < 1 || !int.TryParse(args.Parameters[0], out int idx))
			{
				ShowTo(player);
				return;
			}
			if (!idx.InRange(0, RealmTypes.Count - 1))
			{
				ShowTo(player);
				return;
			}
			var type = RealmTypes[idx];
			var method = type.GetMethod("TriggerByCommand",BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			if (method != null)
			{
				method.Invoke(null, new[] { args });
			}
			else
			{
				var realm = (IRealm)Activator.CreateInstance(type);
				realm.Center = (Vector)args.Player.TPlayer.Center;
				Add(realm);
			}
		}
	}
}
