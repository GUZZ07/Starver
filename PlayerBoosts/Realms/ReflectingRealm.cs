using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace Starvers.PlayerBoosts.Realms
{
	using Shapes;
	public class ReflectingRealm<T> : SRealm<T>
		where T : IReflectiveShape, new()
	{
		private int Owner = -1;

		private StarverPlayer OwnerPlayer => Starver.Instance.Players[Owner];
		public int BorderProjID
		{
			get;
			set;
		}
		public T Reflector
        {
			get => shape;
        }

		public ReflectingRealm(): this(new T())
		{

		}
		public ReflectingRealm(T shape) : base(shape)
		{
			BorderProjID = ProjectileID.WaterBolt;
			Owner = Main.myPlayer;
			DefTimeLeft = 60 * 30;
			shape.BorderProjID = BorderProjID;
		}

		public ReflectingRealm(int owner, T shape) : this(shape)
		{
			if (0 <= owner && owner < Main.myPlayer)
			{
				Owner = owner;
				shape.BorderProjID = ProjectileID.WaterBolt;
			}
			else if (owner == Main.myPlayer)
			{
				shape.BorderProjID = ProjectileID.CursedFlameHostile;
				Owner = owner;
			}
			else
			{
				shape.BorderProjID = ProjectileID.FrostBlastHostile;
				Owner = -1;
			}
		}

		protected override void InternalUpdate()
		{
			base.InternalUpdate();
			if (Owner == Main.myPlayer)
			{
				foreach (var proj in Main.projectile)
				{
					if (proj.active && proj.hostile == false && shape.AtBorder(proj) && proj.aiStyle >= 0)
						Reflect(proj);
				}
				foreach (var player in Starver.Instance.Players)
				{
					if (player != null && player.Alive && shape.AtBorder(player))
						Reflect(player);
				}
			}
			else if (Owner == -1)
			{
				foreach (var proj in Main.projectile)
				{
					if (proj.active && shape.AtBorder(proj) && proj.aiStyle >= 0)
						Reflect(proj);
				}
				foreach (var npc in Main.npc)
				{
					if (npc.active && shape.AtBorder(npc))
						Reflect(npc);
				}
				foreach (var player in Starver.Instance.Players)
				{
					if (player != null && player.Alive && shape.AtBorder(player))
						Reflect(player);
				}
			}
			else if (OwnerPlayer == null)
			{
				Kill();
			}
			else
			{
				foreach (var proj in Main.projectile)
				{
					if (proj.active && proj.hostile && shape.AtBorder(proj) && CanHitOwner(proj) && proj.aiStyle >= 0)
						Reflect(proj);
				}
				foreach (var npc in Main.npc)
				{
					if (npc.active && !npc.friendly && shape.AtBorder(npc))
						Reflect(npc);
				}
				foreach (var player in Starver.Instance.Players)
				{
					if (player != null && player.Alive && CanHitOwner(player) && shape.AtBorder(player))
						Reflect(player);
				}
			}
		}
		#region Reflect
		protected void Reflect(Projectile proj)
		{
			shape.Reflect(proj);
			proj.SendData();
		}
		protected void Reflect(NPC npc)
		{
			shape.Reflect(npc);
			npc.SendData();
		}
		protected void Reflect(Player player)
		{
			shape.Reflect(player);
			player.SendData();
		}
        #endregion
        #region CanHit
        protected bool CanHitOwner(Player player)
		{
			return player.hostile &&
				(player.team != OwnerPlayer.Team || player.team == 0);
		}
		protected bool CanHitOwner(Projectile proj)
		{
			if (proj.owner == Main.myPlayer)
				return proj.hostile;
			var player = Main.player[proj.owner];
			if (player.hostile && (player.team != OwnerPlayer.Team || player.team == 0))
				return true;
			return false;
		}
		#endregion
		#region Trigger
		private static void TriggerByCommand(CommandArgs args)
		{
			int owner;
			if (args.Parameters.Count >= 2)
			{
				owner = int.Parse(args.Parameters[1]);
			}
			else
			{
				owner = 255;
			}
			var realm = new ReflectingRealm<T>(owner, new T())
			{
				Center = (Vector)args.TPlayer.Center,
			};
			if (args.Parameters.Count >= 3 && int.TryParse(args.Parameters[2], out int timeLeft))
			{
				realm.DefTimeLeft = timeLeft;
			}
			Starver.Instance.Realms.Add(realm);
		}
        #endregion
    }
}
