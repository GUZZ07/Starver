using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace Starvers.AuraSystem.Realms
{
	public class PointPlayer : CircleRealm
	{
		private Vector2 Velocity;
		private int idx = -1;
		public Projectile Proj
		{
			get
			{
				if (idx == -1 || !Main.projectile[idx].active || Main.projectile[idx].type != ProjID)
				{
					return null;
				}
				return Main.projectile[idx];
			}
		}
		public int ProjID { get; }
		public int Owner { get; }
		public float Speed { get; set; }
		public Rectangle? Range { get; set; }
		public StarverPlayer Player => Starver.Players[Owner];
		public PointPlayer(int owner, int projID, float radium = 16) : base(true)
		{
			Owner = owner;
			ProjID = projID;
			Radium = radium;
			DefaultTimeLeft = 60 * 60;
		}
		protected override void SetDefault()
		{
		}
		protected override void InternalUpdate()
		{
			if(Player == null)
			{
				Kill();
				return;
			}
			CalcVelocity();
			UpdateMovement();
			UpdateProj();
		}
		private void UpdateProj()
		{
			if (Proj == null)
			{
				idx = Projectile.NewProjectile(Center.X, Center.Y, 0, 0, ProjID, -1, 0);
			}
			Proj.tileCollide = false;
			Proj.aiStyle = -1;
			Proj.netImportant = true;
			Proj.Center = Center;
			Proj.velocity = Velocity;
			Proj.SendData();
		}
		private void CalcVelocity()
		{
			Vector2 velocity = default;
			if (Player.TPlayer.controlUp)
			{
				velocity.Y += -Speed;
			}
			if (Player.TPlayer.controlDown)
			{
				velocity.Y += Speed;
			}
			if (Player.TPlayer.controlLeft)
			{
				velocity.X += -Speed;
			}
			if (Player.TPlayer.controlRight)
			{
				velocity.X += Speed;
			}
			if (velocity != default)
			{
				velocity.Normalize();
			}
			Velocity = velocity * Speed;
		}
		private void UpdateMovement()
		{
			Center += Velocity;
			if (Range != null)
			{
				Rectangle range = Range.Value;
				if (Center.X < range.Left)
				{
					Center = new Vector2(range.Left, Center.Y);
				}
				else if (Center.X >= range.Right)
				{
					Center = new Vector2(range.Right, Center.Y);
				}
				if (Center.Y < range.Top)
				{
					Center = new Vector2(Center.X, range.Top);
				}
				else if (Center.Y >= range.Bottom)
				{
					Center = new Vector2(Center.X, range.Bottom);
				}
			}
		}
		public bool Collided(CircleRealm circle)
		{
			return Vector2.Distance(Center, circle.Center) < Radium + circle.Radium;
		}
		public override void Kill()
		{
			StarverPlayer.All.SendDeBugMessage("catched");
			Proj?.KillMeEx();
			base.Kill();
		}
	}
}
