using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.BossSystem.Bosses.Clover
{
	using Base;
	using Microsoft.Xna.Framework;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public partial class StarverManager
	{
		private class FinalDestroyer : StarverDestroyer
		{
			#region Fields
			private float Radium = StarverManager.Radium;
			private Vector ForRounding;
			private StarverManager Manager;
			#endregion
			#region Ctor
			public FinalDestroyer()
			{
				Silence = true;
			}
			#endregion
			#region DamageIndex
			public override float DamageIndex => Manager.DamageIndex / 5;
			#endregion
			#region Spawn
			public void Respawn()
			{
				base.Spawn(LastCenter, Level);
			}
			public void Spawn(Vector2 where, int lvl = 2000, StarverManager manager = null)
			{
				base.Spawn(where, lvl);
				Mode = BossMode.WaitForMode;
				lastMode = BossMode.Present;
				Manager = manager;
				Radium = StarverManager.Radium;
				ForRounding.X = 0;
				ForRounding.Y = StarverManager.Radium;
				ForRounding.Angle = PI * 2 * 4 / 4;
			}
			#endregion
			#region RealAI
			public override void RealAI()
			{
				#region Common
				if (!Manager.Active)
				{
					KillMe();
					return;
				}
				RealNPC.ai[0] = 12f;
				RealNPC.ai[1] = 0f;
				RealNPC.ai[2] = 0f;
				ForRounding.Angle += PI / 120;
				ForRounding.Length = Radium;
				Center = TargetPlayer.Center + ForRounding;
				TargetPlayer.TPlayer.ZoneTowerSolar = true;
				TargetPlayer.SendData(PacketTypes.Zones, "", Target);
				#endregion
				#region Mode
				switch (Mode)
				{
					#region SelectMode
					case BossMode.WaitForMode:
						SelectMode();
						break;
					#endregion
					#region DemonSickle
					case BossMode.DemonSickle:
						if (modetime > 60 * 15)
						{
							ResetMode();
						}
						if (Timer % 2 == 0)
						{
							DemonSickle();
						}
						break;
					#endregion
					#region Fire
					case BossMode.Fire:
						if (modetime > 60 * 12)
						{
							ResetMode();
						}
						if (Timer % 2 == 0)
						{
							Fire();
						}
						break;
					#endregion
					#region FlamingScythe
					case BossMode.FlamingScythe:
						unsafe
						{
							if (floats[0] > 8)
							{
								floats[0] = 0;
								ResetMode();
							}
							if (Timer % 90 == 0)
							{
								FlamingScythe((int)floats[0]++);
							}
						}
						break;
					#endregion
					#region Present
					case BossMode.Present:
						if (modetime > 60 * 12)
						{
							ResetMode();
						}
						if (Timer % 75 == 0)
						{
							Present();
						}
						break;
					#endregion
				}
				#endregion
			}
			#endregion
			#region SelectMode
			private new void SelectMode()
			{
				modetime = 0;
				LastCenter = Center;
				switch (lastMode)
				{
					#region DemonSickle
					case BossMode.DemonSickle:
						Mode = BossMode.Fire;
						break;
					#endregion
					#region Fire
					case BossMode.Fire:
						Mode = BossMode.FlamingScythe;
						break;
					#endregion
					#region FlamingScythe
					case BossMode.FlamingScythe:
						Mode = BossMode.Present;
						break;
					#endregion
					#region Present
					case BossMode.Present:
						Mode = BossMode.DemonSickle;
						break;
						#endregion
				}
			}
			#endregion
		}
	}
}
