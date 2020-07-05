using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace Starvers.Enemies.Bosses
{
	public class EyeEx : StarverBoss
	{
		private const int maxRushTime = 60 * 5;

		private int timer;
		private int rushTime;
		private BossStateMachine machine;

		public EyeEx() : base(NPCID.EyeofCthulhu)
		{
			defLifes = 3;
			defLife = 45000;
			defDefense = 1000;
		}

		public override void Spawn(Vector2 position, int level = 2000)
		{
			base.Spawn(position, level);
			TNPC.ai[0] = 3f;
			TNPC.ai[1] = 5f; // 为5时是疯狗模式; 2正常状态
			TNPC.ai[2] = 120f;
			TNPC.ai[3] = 0f;
			TNPC.netUpdate = true;
			timer = 0;
			rushTime = 0;
			machine = null;
		}

		protected override void RealAI()
		{
			VerifyTarget();
			if (TargetPlayer?.Alive != true)
			{
				TurnToAir();
				return;
			}
			timer++;
			if (machine == null)
			{
				if (rushTime++ >= maxRushTime)
				{
					TNPC.aiStyle = -1;
					TNPC.ai[1] = 2f;
					TNPC.netUpdate = true;
					rushTime = 0;
					machine = new EvilTridentMachine(this);
					machine.Begin();
				}
			}
			else
			{
				machine.Update();
				if (machine.IsEnd)
				{
					machine = null;
					TNPC.aiStyle = 4;
					TNPC.ai[1] = 5f;
					TNPC.ai[2] = 120;
					TNPC.ai[3] = 0;
					TNPC.netUpdate = true;
				}
				else
				{
					Velocity = TargetPlayer.Center + new Vector2(0, -16 * 20) - Center;
					Velocity /= 13;
					{
						UpdateToClient();
					}
				}
			}
		}

		public override void Kill()
		{
			machine?.Abort();
			base.Kill();
		}

		private void VerifyTarget()
		{
			if (TargetPlayer?.Alive != true)
			{
				FindTarget();
			}
		}

		private void FindTarget()
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
				else if (TNPC.Distance(target.Center) > TNPC.Distance(player.Center))
				{
					target = player;
				}
			}
			Target = target?.Index ?? -1;
		}
	}
}
