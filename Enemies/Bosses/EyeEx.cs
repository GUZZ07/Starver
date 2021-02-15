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
		#region Fields
		private const int maxRushTime = 60 * 5;

		private int timer;
		private int rushTime;
		private byte GazingState;

		private BossStateMachine currentProjController;

		private BossStateMachine ProjController
		{
			get
			{
				return currentProjController;
			}
			set
			{
				currentProjController = value;
				if (value != null)
				{
					value.OnEnd += BeginRush;
				}
			}
		}

		private PlayerTracker TargetTracker;
		#endregion
		#region Ctor
		public EyeEx() : base(NPCID.EyeofCthulhu)
		{
			defLifes = 2;
			defLife = 45000;
			defDefense = 1000;
			AfraidSun = true;

			TargetTracker = new PlayerTracker(this)
			{
				MaxSpeed = 10,
				Offset = new Vector2(0, -16 * 20),
				MaxDistance = 16 * 600
			};
		}
		#endregion
		#region Spawn
		public override void Spawn(Vector2 position, int level = 2000)
		{
			base.Spawn(position, level);
			BeginRush();
			timer = 0;
			ProjController = null;
			GazingState = 0;
		}
		#endregion
		#region LifeDown
		protected override void LifesDown()
		{
			base.LifesDown();
			if (GazingState == 0 && Lifes <= LifesMax / 2)
			{
				GazingState |= 0b00000001;
				SwitchToGazing(Color.Magenta);
			}
			else if (GazingState == 0b00000001 && Lifes <= LifesMax / 4)
			{
				GazingState |= 0b00000011;
				SwitchToGazing(new Color(255, 0, 188));
			}
		}
		#endregion
		#region RealAI
		protected override void RealAI()
		{
			timer++;
			TargetTracker.Update();
			if (!Active)
			{
				return;
			}
			if (ProjController?.IsEnd != false)
			{
				if (rushTime++ >= maxRushTime)
				{
					NextMachine();
				}
			}
			else
			{
				TNPC.ai[0] = 2f;
				if (TNPC.ai[1] % 20 == 19f)
				{
					TNPC.ai[1] = 1f;
				}
				TNPC.ai[2] = (float)Math.Abs(0.6 * Math.Sin(timer / 180 * Math.PI * 5 / 90));
				TNPC.ai[3] = 0f;
				ProjController.Update();
			}
		}
		#endregion
		#region Kill
		public override void Kill()
		{
			ProjController?.Abort();
			base.Kill();
		}
		#endregion
		#region Target
		#endregion
		#region NextMachine
		private void SwitchToGazing(Color messageColor)
		{
			TNPC.ai[0] = 2f;
			TargetTracker.DontMove = false;
			ProjController?.Abort();
			ProjController = new GazingYouMachine(this)
			{
				MessageColor = messageColor,
				TotalTime = 60 * 9
			};
			ProjController.Begin();
		}
		private void NextMachine()
		{
			// TNPC.aiStyle = -1;
			TNPC.ai[0] = 2f;
			// TNPC.ai[1] = 0f;
			TNPC.ai[3] = 0f;
			TNPC.netUpdate = true;
			ProjController = GetNextMachine(ProjController?.State);
			ProjController.Begin();
			TargetTracker.DontMove = false;
		}

		private BossStateMachine GetNextMachine(BossState? last)
		{
			switch (last)
			{
				case BossState.GazingYou:
					DontTakeDamage = false;
					break;
				case BossState.FaithOfMountain when Lifes <= LifesMax / 2 && GazingProbability():
					return new GazingYouMachine(this);
			}
			return last switch
			{
				BossState.EyeSharknado => new FaithOfMountainMachine(this),
				BossState.FakeDukeRush => new EyeSharknadoMachine(this),
				BossState.EvilTrident => new FakeDukeRushMachine(this),
				_ => new EvilTridentMachine(this)
				{
					CreateInterval = 5,
					TridentCount = 60,
					Damage = 30,
					LaunchDelay = 30
				}
			};
		}

		private bool GazingProbability()
		{
			var probability = 1 - (double)Lifes / LifesMax;
			probability /= 3;
			return Rand.Probability(probability);
		}
		#endregion
		#region BeginRush
		private void BeginRush()
		{
			TargetTracker.DontMove = true;
			rushTime = 0;
			TNPC.aiStyle = 4;
			TNPC.ai[0] = 3f;
			TNPC.ai[1] = 5f; // 为5时是疯狗模式; 2正常状态 0会飘在玩家头顶上
			TNPC.ai[2] = 120f;
			TNPC.ai[3] = 0f;
			TNPC.netUpdate = true;
		}
		#endregion
	}
}
