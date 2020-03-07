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
		private class FinalRedeemer : StarverRedeemer
		{
			#region Fields
			private float Radium = StarverManager.Radium;
			private StarverManager Manager;
			#endregion
			#region Ctor
			public FinalRedeemer()
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
			public void Spawn(Vector2 where, int lvl = 2000,StarverManager manager = null)
			{
				Manager = manager;
				Spawn(where, lvl, PI * 2 * 2 / 4, Radium);
			}
			#endregion
			#region RealAI
			public override void RealAI()
			{
				if (!Manager.Active)
				{
					KillMe();
					return;
				}
				vector.Length = Radium;
				base.RealAI();
				RealNPC.ai[0] = 12f;
				RealNPC.ai[1] = 0f;
				RealNPC.ai[2] = 0f;
			}
			#endregion
		}
	}
}
