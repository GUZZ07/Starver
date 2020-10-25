using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using System.Collections.Specialized;

namespace Starvers
{
	public class PlayerManager : IEnumerable<StarverPlayer>, INotifyCollectionChanged
	{
		private StarverPlayer[] Players;
		private int timer;


		private int SaveInterval => Starver.Instance.Config.SaveInterval;

		#region Indexer
		public StarverPlayer this[int index]
		{
			get => Players[index];
			set => Set(index, value);
		}
		#endregion
		#region Ctor
		public PlayerManager(int count)
		{
			Players = new StarverPlayer[count];

			for (int i = 0; i < TShock.Players.Length; i++)
			{
				var player = TShock.Players[i];
				if (player is null)
				{
					continue;
				}
				if (!player.IsLoggedIn)
				{
					Players[i] = StarverPlayer.GetGuest(i);
				}
				else
				{
					Players[i] = new StarverPlayer(i);
				}
			}
		}
		#endregion
		#region Update
		public void PostUpdate()
		{
			foreach (var player in Players)
			{
				player?.PostUpdate();
			}
		}
		public void Update()
		{
			foreach (var player in Players)
			{
				player?.Update();
			}
			timer++;
			if (timer % (SaveInterval * 60) == 0)
			{
				SaveAll();
			}
		}
		#endregion
		#region SaveAll
		public void SaveAll()
		{
			foreach (var player in Players)
			{
				player?.SaveData();
			}
		}
		#endregion
		public StarverPlayer FindPlayerClosestTo(Vector2 position)
		{
			StarverPlayer target = null;
			foreach (var player in Starver.Instance.Players)
			{
				if (player?.Alive != true)
				{
					continue;
				}
				if (target == null)
				{
					target = player;
				}
				else if (Vector2.Distance(position, target.Center) > Vector2.Distance(position, player.Center))
				{
					target = player;
				}
			}
			return target;
		}
		
		#region GetEnumerator
		public IEnumerator<StarverPlayer> GetEnumerator()
		{
			IEnumerable<StarverPlayer> players = Players;
			return players.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
		#region INotifyCollectionChanged
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		private void Set(int index, StarverPlayer value)
		{
			var old = Players[index];
			Players[index] = value;
			OnChanged(index, old);
		}
		private void OnChanged(int index, StarverPlayer old)
		{
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, Players[index], old, index));
		}
		#endregion
	}
}
