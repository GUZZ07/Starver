using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Starvers
{
	public class PlayerManager : IEnumerable<StarverPlayer>
	{
		private StarverPlayer[] Players;
		private int timer;

		private int SaveInterval => Starver.Instance.Config.SaveInterval;

		#region Indexer
		public StarverPlayer this[int index]
		{
			get => Players[index];
			set => Players[index] = value;
		}
		#endregion
		#region Ctor
		public PlayerManager(int count)
		{
			Players = new StarverPlayer[count];
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
	}
}
