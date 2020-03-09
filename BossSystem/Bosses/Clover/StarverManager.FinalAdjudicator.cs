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
		private class FinalAdjudicator : StarverAdjudicator
		{
			#region Fields
			private float Radium = 16 * 37;
			private StarverManager Manager;
			#endregion
			#region Ctor
			public FinalAdjudicator()
			{
				Silence = true;
			}
			#endregion
			#region DamageIndex
			public override float DamageIndex => Manager.DamageIndex;
			#endregion
			#region Spawn
			public void Respawn()
			{
				base.Spawn(LastCenter, Level);
			}
			public void Spawn(Vector2 where, int lvl = 2000, StarverManager manager = null)
			{
				Spawn(where, lvl, PI * 2 * 3 / 4, Radium);
				Manager = manager;
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
				vector.Angle += PI / 120;
				vector.Length = StarverManager.Radium;
				Center = vector + TargetPlayer.Center;
				#endregion
				#region Mode
				switch (Mode)
				{
					#region SelectMode
					case BossMode.WaitForMode:
						LastCenter = Center;
						modetime = 0;
						switch (lastMode)
						{
							#region Bolt
							case BossMode.WitherBolt:
								Mode = BossMode.WitherLostSoul;
								break;
							#endregion
							#region LostSoul
							case BossMode.WitherLostSoul:
								Mode = BossMode.WitherSphere;
								unsafe
								{
									floats[0] = 60;
								}
								break;
							#endregion
							#region Sphere
							case BossMode.WitherSphere:
								Mode = BossMode.WitherSaucerLaser;
								break;
							#endregion
							#region Laser/Summon
							case BossMode.WitherSaucerLaser:
								Mode = BossMode.WitherBolt;
								break;
								#endregion
						}
						break;
					#endregion
					#region Bolt
					case BossMode.WitherBolt:
						if (modetime > 60 * 8)
						{
							ResetMode();
							break;
						}
						if (Timer % floats[0] == 0)
						{
							WitherBolt();
						}
						break;
					#endregion
					#region LostSoul
					case BossMode.WitherLostSoul:
						if (modetime > 60 * 10)
						{
							ResetMode();
							break;
						}
						WitherLostSoul();
						break;
					#endregion
					#region Laser
					case BossMode.WitherSaucerLaser:
						if (modetime > 60 * 10)
						{
							ResetMode();
							Vel.Y = 0;
							Vel.X = 16 * 10;
							break;
						}
						if (Timer % 2 == 0)
						{
							WitherLaser();
						}
						break;
					#endregion
					#region SummonFollows
					case BossMode.SummonFollows:
						if (modetime > 60 * 6)
						{
							ResetMode();
							break;
						}
						if (Timer % 30 == 0)
						{
							SummonFollows();
						}
						break;
					#endregion
					#region Sphere
					case BossMode.WitherSphere:
						if (modetime > 60 * 9)
						{
							ResetMode();
							floats[0] = 35;
							break;
						}
						if (Timer % floats[0] == 0)
						{
							WitherSphere();
						}
						break;
						#endregion
				}
				#endregion
			}
			#endregion
		}
	}
}
