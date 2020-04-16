using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Starvers.AuraSystem.Realms
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class ReflectingRealm : CircleRealm
	{
		private int Owner;
		private double angle;
		private int borderProj;
		private int[] BorderProjs;
		private StarverPlayer OwnerPlayer => Starver.Players[Owner];
		public ReflectingRealm() : base(true)
		{
			Owner = Main.myPlayer;
			DefaultTimeLeft = 60 * 30;
		}

		public ReflectingRealm(int owner) : this()
		{
			if (0 <= owner && owner <= Main.myPlayer)
			{
				Owner = owner;
			}
			else
			{
				Owner = -1;
			}
		}

		public override void Start()
		{
			base.Start();
			StartBorder();
		}

		protected override void SetDefault()
		{
			Radium = 16 * 15;
		}

		protected override void InternalUpdate()
		{
			if (Owner == Main.myPlayer)
			{
				foreach (var proj in Main.projectile)
				{
					if (proj.active && proj.hostile == false && AtBorder(proj) && proj.aiStyle >= 0)
						Reflect(proj);
				}
				foreach (var player in Starver.Players)
				{
					if (player != null && player.Active && AtBorder(player))
						Reflect(player);
				}
			}
			else if (Owner == -1)
			{
				foreach (var proj in Main.projectile)
				{
					if (proj.active && AtBorder(proj) && proj.aiStyle >= 0)
						Reflect(proj);
				}
				foreach (var npc in Main.npc)
				{
					if (npc.active && AtBorder(npc))
						Reflect(npc);
				}
				foreach (var player in Starver.Players)
				{
					if (player != null && player.Active && AtBorder(player))
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
					if (proj.active && proj.hostile && AtBorder(proj) && CanHitOwner(proj) && proj.aiStyle >= 0)
						Reflect(proj);
				}
				foreach (var npc in Main.npc)
				{
					if (npc.active && !npc.friendly && AtBorder(npc))
						Reflect(npc);
				}
				foreach (var player in Starver.Players)
				{
					if (player != null && player.Active && CanHitOwner(player) && AtBorder(player))
						Reflect(player);
				}
			}
			UpdateBorder();
		}

		protected void StartBorder()
		{
			if (Owner == Main.myPlayer)
				borderProj = ProjectileID.CannonballHostile;
			else if (0 <= Owner && Owner < Main.myPlayer)
				borderProj = ProjectileID.WaterBolt;
			else
				borderProj = ProjectileID.CannonballFriendly;


			BorderProjs = new int[60];
			for (int i = 0; i < BorderProjs.Length; i++)
			{
				BorderProjs[i] = Utils.NewProj
					(
					position: Center + Vector.FromPolar(Math.PI * 2 / 60 * i + angle, Radium),
					velocity: Vector2.Zero,
					Type: borderProj,
					Damage: 0,
					KnockBack: 0,
					Owner: Main.myPlayer
					);
				Main.projectile[BorderProjs[i]].aiStyle = -2;
			}
		}
		protected void UpdateBorder()
		{
			angle += Math.PI * 2 / 90;
			for (int i = 0; i < BorderProjs.Length; i++)
			{
				if (!Main.projectile[BorderProjs[i]].active || Main.projectile[BorderProjs[i]].type != borderProj)
				{
					BorderProjs[i] = Utils.NewProj
					(
					position: Center + Vector.FromPolar(Math.PI * 2 / 60 * i + angle, Radium),
					velocity: Vector2.Zero,
					Type: borderProj,
					Damage: 0,
					KnockBack: 0,
					Owner: Main.myPlayer
					);
					Main.projectile[BorderProjs[i]].aiStyle = -2;
				}
				else
				{
					Main.projectile[BorderProjs[i]].Center = Center + Vector.FromPolar(Math.PI * 2 / 60 * i + angle, Radium);
				}
				Main.projectile[BorderProjs[i]].SendData();
			}
		}

		protected bool CanHitOwner(Player player)
		{
			return player.hostile &&
				(player.team != OwnerPlayer.Team || player.team == 0);
		}
		protected bool CanHitOwner(Projectile proj)
		{
			if (proj.owner == Main.myPlayer)
				return proj.hostile;
			Player player = Main.player[proj.owner];
			if (player.hostile && (player.team != OwnerPlayer.Team || player.team == 0))
				return true;
			return false;
		}
		protected bool AtBorder(Entity entity)
		{
			return Math.Abs(Vector2.Distance(Center, entity.Center) - Radium) <= 16 * 2.5f;
		}
		protected void Reflect(NPC npc)
		{
			Vector2 Distance = npc.Center - Center;
			npc.velocity -= 2 * Distance * Vector2.Dot(Distance, npc.velocity) / (Radium * Radium);
			npc.SendData();
		}
		protected void Reflect(Projectile proj)
		{
			Vector2 Distance = proj.Center - Center;
			proj.velocity -= 2 * Distance * Vector2.Dot(Distance, proj.velocity) / (Radium * Radium);
			proj.SendData();
		}
		protected void Reflect(StarverPlayer player)
		{
			Vector2 Distance = player.Center - Center;
			player.Velocity -= 2 * Distance * Vector2.Dot(Distance, player.Velocity) / (Radium * Radium);
		}
	}
}
