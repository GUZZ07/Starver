using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace Starvers.Enemies.Bosses
{
	public class CursedSkeletron : StarverBoss
	{
		private Vector FakeVelocity;

		public CursedSkeletron() : base(NPCID.SkeletronHead)
		{
			defLifes = 13;
			defLife = 48000;
			defDefense = 1200;
			AfraidSun = true;
		}

		public override void Spawn(Vector2 position, int level = 2000)
		{
			base.Spawn(position, level);
			TNPC.aiStyle = -1;
			relativePos = new Vector2(0, 16 * 40);
			trackPara = 0;
			withinFollowing = false;
			FakeVelocity = default;
		}

		protected override void ReSpawn()
		{
			base.ReSpawn();
			TNPC.aiStyle = -1;
		}

		protected override void RealAI()
		{
			if (TargetPlayer?.Alive != true)
			{
				TryFindTarget();
				if (TargetPlayer?.Alive != true)
				{
					TurnToAir();
					return;
				}
			}
			UpdateMovement();
			Velocity = FakeVelocity;
			UpdateToClient();
		}

		private const float followingDistance = 16 * 300;
		private bool withinFollowing;
		private Vector2 relativePos;
		private Vector2 fakeTarget;
		private int trackPara;

		private void UpdateMovement()
		{
			if(!withinFollowing)
			{
				if (TNPC.Distance(TargetPlayer.Center) <= followingDistance)
				{
					withinFollowing = true;
					fakeTarget = (2 * TargetPlayer.Center + Center) / 3; // center____ __target
				}
				else
				{
					float s = TNPC.Distance(TargetPlayer.Center) / 16;
					Vector2 vMax = (TargetPlayer.Center - Center).ToLenOf(s * s / 100);
					Vector2 accel = (vMax - FakeVelocity) / 120;
					FakeVelocity += (Vector)accel;
				}
			}
			else
			{
				if (TNPC.Distance(TargetPlayer.Center) > followingDistance)
				{
					withinFollowing = false;
				}
			}
			if (withinFollowing)
			{
				double t = trackPara * Math.PI / 180;
				double ρ = (2 + Math.Abs(3 * Math.Sin(2 * t)) + Math.Abs(2 * Math.Cos(3 * t))) * 16 * 24;
				relativePos = new Vector2
				{
					X = (float)(ρ * Math.Cos(t + Math.PI / 2)),
					Y = (float)(ρ * Math.Sin(t + Math.PI / 2))
				};
				FakeVelocity = (Vector)((relativePos + fakeTarget) - Center) / 4;
				if (FakeVelocity.Length > 20)
				{
					FakeVelocity.Length = 20;
				}
				if (Vector2.Distance(fakeTarget, TargetPlayer.Center) > 10)
				{
					fakeTarget += (TargetPlayer.Center - fakeTarget).ToLenOf(9);
				}
				trackPara++;
			}
		}
	}
}
