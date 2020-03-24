using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.WeaponSystem.Weapons.Ranged
{
	using Vector = TOFOUT.Terraria.Server.Vector2;
	using IID = Terraria.ID.ItemID;
	using PID = Terraria.ID.ProjectileID;
	public class PhantasmEx : Weapon
	{
		#region Fields
		private Vector vector;
		#endregion
		#region Ctor
		public PhantasmEx() : base(1, IID.Phantasm, PID.MoonlordArrow, CareerType.Ranged, 175)
		{
			CatchID = PID.Phantasm;
		}
		#endregion
		#region UseWeapon
		public override void UseWeapon(StarverPlayer player, Vector Velocity, int lvl, TShockAPI.GetDataHandlers.NewProjectileEventArgs args)
		{
			vector = Velocity.Vertical();
			vector.Length = 16 * 5;
			player.ProjLine(player.Center - vector, player.Center + vector, Velocity, Math.Min(8, (int)(5 * (1 + Math.Sqrt(lvl)))), CalcDamage(lvl), ProjID);
		}
		#endregion
	}
}
