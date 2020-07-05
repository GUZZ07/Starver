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


		private BossStateMachine ProjController;

		private PlayerTracker PositionController;

		public EyeEx() : base(NPCID.EyeofCthulhu)
		{
			defLifes = 3;
			defLife = 45000;
			defDefense = 1000;

			PositionController = new PlayerTracker(this)
			{
				MaxSpeed = 10,
				Offset = new Vector2(0, -16 * 20)
			};
		}

		public override void Spawn(Vector2 position, int level = 2000)
		{
			base.Spawn(position, level);
			BeginRush();
			timer = 0;
			ProjController = null;
		}

		protected override void RealAI()
		{
			if (Main.dayTime)
			{
				TurnToAir();
				return;
			}
			VerifyTarget();
			if (TargetPlayer?.Alive != true)
			{
				TurnToAir();
				return;
			}
			if (TNPC.Distance(TargetPlayer.Center) > 16 * 600)
			{
				Center = TargetPlayer.Center + Rand.NextVector2(16 * 100);
			}
			timer++;
			if (ProjController?.IsEnd != false)
			{
				if (rushTime++ >= maxRushTime)
				{
					NextMachine();
				}
			}
			else
			{
				ProjController.Update();
				PositionController.Update();
				if (ProjController.IsEnd)
				{
					BeginRush();
				}
			}
		}

		public override void Kill()
		{
			ProjController?.Abort();
			base.Kill();
		}

		private void VerifyTarget()
		{
			if (TargetPlayer?.Alive != true)
			{
				FindTarget();
			}
		}

		private void NextMachine()
		{
			// TNPC.aiStyle = -1;
			TNPC.ai[0] = 2f;
			// TNPC.ai[1] = 0f;
			// TNPC.ai[3] = 0f;
			TNPC.netUpdate = true;
			ProjController = GetNextMachine(ProjController?.State);
			ProjController.Begin();
			PositionController.IsPause = false;
		}

		private BossStateMachine GetNextMachine(BossState? last)
		{
			return last switch
			{
				BossState.FakeDuckRush => new FaithOfMountainMachine(this),
				BossState.EvilTrident => new FakeDuckRushMachine(this),
				_ => new EvilTridentMachine(this)
				{
					CreateInterval = 5,
					TridentCount = 60,
					Damage = 30,
					LaunchDelay = 30
				}
			};
		}

		private void BeginRush()
		{
			PositionController.IsPause = true;
			rushTime = 0;
			TNPC.aiStyle = 4;
			TNPC.ai[0] = 3f;
			TNPC.ai[1] = 5f; // 为5时是疯狗模式; 2正常状态 0会飘在玩家头顶上
			TNPC.ai[2] = 120f;
			TNPC.ai[3] = 0f;
			TNPC.netUpdate = true;
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
