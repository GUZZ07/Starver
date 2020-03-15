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
	public class PhantomPhoenix : Weapon
	{
		#region Fields
		protected Vector vel;
		protected const double rad = Math.PI / 12;
		protected double rd = 0;
		#endregion
		#region Ctor
		public PhantomPhoenix() : base(2,IID.DD2PhoenixBow,PID.DD2PhoenixBowShot,CareerType.Ranged, 43)
		{
			CatchID = PID.DD2PhoenixBow;
		}
		#endregion
		#region UseWeapon
		public override void UseWeapon(StarverPlayer player, Vector Velocity,int lvl, TShockAPI.GetDataHandlers.NewProjectileEventArgs args)
		{
			player.ProjSector(player.Center, Velocity.Length, 16 * 3, Velocity.Angle, (float)(Math.PI / 3), CalcDamage(lvl), ProjID, lvl > 10 ? 3 : 5);
		}
		#endregion
	}
}
