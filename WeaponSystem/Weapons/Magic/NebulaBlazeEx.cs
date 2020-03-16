using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.WeaponSystem.Weapons.Magic
{
	using TShockAPI;
	using IID = Terraria.ID.ItemID;
	using PID = Terraria.ID.ProjectileID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class NebulaBlazeEx : Weapon
	{
		#region Ctor
		public NebulaBlazeEx() : base(1, IID.NebulaBlaze, PID.NebulaBlaze2, CareerType.Magic, 282)
		{
			CatchID = PID.NebulaBlaze1;
		}
		#endregion
		#region UseWeapon
		public override void UseWeapon(StarverPlayer player, Vector Velocity, int lvl, GetDataHandlers.NewProjectileEventArgs args)
		{
			player.ProjCircle(player.Center, 16 * 3, 29, ProjID, 4, CalcDamage(lvl));
		}
		#endregion
	}
}
