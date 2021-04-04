using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.BossSystem.Bosses.Clover
{
	using Microsoft.Xna.Framework;
	using Terraria.ID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public partial class StarverManager
	{
		private class FinalWander : StarverWander
		{
			#region Fields
			private float Radium;
			private StarverManager Manager;
			private Vector ForRounding;
			#endregion
			#region Ctor
			public FinalWander()
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
			public void Spawn(Vector2 where, int lvl = 2000,StarverManager manager = null)
			{
				Manager = manager;
				Radium = StarverManager.Radium;
				Spawn(where, lvl, PI * 2 * 1 / 4,Radium);
				Ammo = ProjectileID.VortexLaser;
				ExVersion = true;
			}
			protected void Spawn(Vector2 where, int lvl = CriticalLevel, double AngleStart = 0, float radium = -1)
			{
				base.Spawn(where, lvl);
				if (radium > 0)
				{
					ForRounding.X = radium;
				}
				ForRounding.Angle = AngleStart;
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
				ForRounding.Length = StarverManager.Radium;
				Center = TargetPlayer.Center + ForRounding;
				#endregion
				#region Mode
				switch (Mode)
				{
					#region SelectMode
					case BossMode.WaitForMode:
						SelectMode();
						break;
					#endregion
					#region Shoot1
					case BossMode.CraShoot1:
						if (modetime > 60 * 7.5)
						{
							ResetMode();
							floats[1] = 0;
							break;
						}
						if (Timer % (5 / 2) == 0)
						{
							Shoot1();
						}
						break;
					#endregion
					#region Shoot2
					case BossMode.CraShoot2:
						if (modetime > 60 * 10)
						{
							ResetMode();
							floats[1] = 0;
							break;
						}
						if (Timer % (60 / 2) == 0)
						{
							Shoot2();
						}
						break;
					#endregion
					#region Shoot3
					case BossMode.CraShoot3:
						if (modetime > 60 * 10)
						{
							ResetMode();
							floats[1] = 0;
							break;
						}
						if (Timer % (55 / 2) == 0)
						{
							Shoot3();
						}
						break;
					#endregion
					#region Shoot4
					case BossMode.Crashoot4:
						if (modetime > 60 * 12)
						{
							ResetMode();
							break;
						}
						Shoot4();
						break;
					#endregion
					#region VortexSphere
					case BossMode.CraVortexSphere:
						if (floats[1] > 20)
						{
							floats[1] = 0;
							ResetMode();
							break;
						}
						if (Timer % 20 == 0)
						{
							VortexSphere();
						}
						break;
					#endregion
					#region SummonFollows
					case BossMode.SummonFollows:
						if (modetime > 60 * 7)
						{
							ResetMode();
							break;
						}
						if (Timer % 25 == 0)
						{
							SummonFollows();
						}
						break;
					#endregion
					#region Mist
					case BossMode.Mist:
						if (modetime > 60 * 9)
						{
							ResetMode();
							floats[1] = 0;
							break;
						}
						floats[1] += PI / 9;
						if (Timer % 60 == 0)
						{
							foreach (var player in Starver.Players)
							{
								if (player == null || !player.Active)
								{
									continue;
								}
								for (int i = 0; i < 8; i++)
								{
									Proj(player.Center + FromPolar(floats[1] + PI * i / 4, 16 * 20), FromPolar(floats[1] + PI * i / 4 + PI * 3 / 4, 20), ProjectileID.CultistBossIceMist, 266, 5f, -3e3f, 1);
								}
							}
						}
						break;
					#endregion
					#region FireBall
					case BossMode.CraFireBall:
						if (floats[1] > 6)
						{
							floats[1] = 0;
							ResetMode();
							break;
						}
						if (Timer % (int)(60 + 15 * floats[1]) == 0)
						{
							FireBall();
						}
						break;
						#endregion
				}
				#endregion
			}
			#endregion
			#region AIs
			#region Shoot1
			private unsafe new void Shoot1()
			{
				floats[1] += PI / 40;
				vector.Angle = (TargetPlayer.Center - Center).Angle();
				UnitX = vector;
				if (floats[1] > PI)
				{
					floats[1] -= PI;
				}
				else if (floats[1] > PI / 2)
				{
					vector.Angle += PI - floats[1];
					UnitX.AngleAdd(-(PI - floats[1]));
				}
				else
				{
					vector.Angle += floats[1];
					UnitX.AngleAdd(-floats[1]);
				}
				Vel = vector;
				Vel.Y *= -1;
				Proj(Center, UnitX, Ammo, 240, 2f);
				Proj(Center, vector, Ammo, 240, 2f);
				Proj(Center, Vel, Ammo, 200, 2f);
			}
			#endregion
			#endregion
		}
	}
}
