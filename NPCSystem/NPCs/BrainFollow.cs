using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace Starvers.NPCSystem.NPCs
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	using BrainEx = BossSystem.Bosses.BrainEx;
	/// <summary>
	/// 请勿使用StarverNPC.NewNPC生成
	/// </summary>
	public class BrainFollow : StarverNPC
	{
		#region Fields
		private float Unit = PI / 100;
		private int dmg;
		private int Mod;
		private BrainEx Brain;
		private AIDelegate Rounding = null;
		#endregion
		#region Properties
		public override float DamageIndex => Brain.DamageIndex;
		#endregion
		#region Ctor
		public BrainFollow():base(1)
		{
			NoTileCollide = true;
			RawType = NPCID.Creeper;
			DefaultDefense = 3100;
			DefaultLife = short.MaxValue;
		}
		#endregion
		#region OnDead
		public override void OnKilled()
		{
			Brain.AliveFollows -= 1;
			Starver.NPCs[Index] = null;
		}
		#endregion
		#region Spawn
		/// <summary>
		/// 
		/// </summary>
		/// <param name="where"></param>
		/// <param name="roundtype">0: 顺时针 1: 逆时针</param>
		/// <returns></returns>
		public bool Spawn(Vector where,BrainEx owner,float start,float Radium = 16 * 10,int roundtype = 0)
		{
			bool flag = base.Spawn(where);
			Starver.NPCs[Index] = NPCs[Index] = this;
			RealNPC.aiStyle = None;
			Brain = owner;
			Target = Brain.Target;
			Timer += (uint)(start * 10);
			if(roundtype == 0)
			{
				dmg = 153;
				Mod = 50;
				Unit = PI / 100;
				Rounding = Round;
			}
			else
			{
				dmg = 139;
				Mod = 70;
				Unit = PI / 50;
				Rounding = Round_Negative;
			}
			Vel = FromPolar(start, Radium);
			//DamagedIndex = Criticallevel / (float)Brain.Level;
			AIUsing[0] = Radium;
			return flag;
		}
		#endregion
		#region RealAI
		protected override void RealAI()
		{
			if (!Brain.Active)
			{
				KillMe();
				return;
			}
			Rounding();
			Target = Brain.Target;
			if (Timer % Mod  == 0)
			{
				FakeVelocity = (Vector)(TargetPlayer.Center - Center);
				FakeVelocity.Length = 17.5f;
				Proj(Center, FakeVelocity, ProjectileID.EyeLaser, dmg);
				FakeVelocity = Vector.Zero;
			}
			//SendData();
		}
		#endregion
		#region Round
		/// <summary>
		/// 顺时针
		/// </summary>
		private void Round()
		{
			Vel.Angle += Unit;
			Center = Brain.Center + Vel;
		}
		/// <summary>
		/// 逆时针
		/// </summary>
		private void Round_Negative()
		{
			Vel.Angle -= Unit;
			Center = Brain.Center + Vel;
		}
		#endregion
	}
}
