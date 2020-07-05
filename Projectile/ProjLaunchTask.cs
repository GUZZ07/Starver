using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Starvers
{
	public struct ProjLaunchTask
	{
		public int Index { get; }
		public Vector2 Velocity { get; }
		public int Delay { get; private set; }
		public Projectile Proj => Main.projectile[Index];

		public ProjLaunchTask(int index, Vector2 velocity, int delay)
		{
			Index = index;
			Velocity = velocity;
			Delay = delay;
		}

		public void Launch()
		{
			Proj.velocity = Velocity;
			Proj.netImportant = true;
			Proj.SendData();
			Proj.netImportant = false;
		}

		public void Launch(Vector2 Velocity)
		{
			Proj.velocity = Velocity;
			Proj.netImportant = true;
			Proj.SendData();
			Proj.netImportant = false;
		}

		public bool CheckLaunch()
		{
			if (--Delay == 0)
			{
				Launch();
				return true;
			}
			return false;
		}

		public void LaunchTo(Entity target, float? speed = null)
		{
			LaunchTo(target.Center, speed);
		}

		public void LaunchTo(Vector2 target, float? speed = null)
		{
			Vector2 vel = Proj.Center - target;
			vel.Length(speed ?? Velocity.Length());
			Proj.velocity = vel;
			Proj.netImportant = true;
			Proj.SendData();
			Proj.netImportant = false;
		}

		public void Cancel(bool killProj = true)
		{
			if (killProj)
			{
				Proj.active = false;
				Proj.netImportant = true;
				Proj.SendData();
				Proj.netImportant = false;
			}
		}
	}
}
