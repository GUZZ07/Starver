using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOFOUT.Terraria.Server;
using TShockAPI;

namespace Starvers.WeaponSystem.Weapons.Ranged
{
	using IDs = Terraria.ID;
	public class VortexBeaterEx : Weapon
	{
		#region Ctor
		public VortexBeaterEx() : base(0,IDs.ItemID.VortexBeater, IDs.ProjectileID.VortexBeaterRocket, CareerType.Ranged, 185)
		{
			CatchID = IDs.ProjectileID.VortexBeater;
		}
		#endregion
		#region Release
		public override void UseWeapon(StarverPlayer player, Vector2 Velocity, int lvl, GetDataHandlers.NewProjectileEventArgs args)
		{
			var vertical = Velocity.Vertical();
			var dmg = CalcDamage(lvl);
			vertical.Normalize();
			if (lvl > 5)
			{
				player.NewProj(player.Center, Velocity + vertical * 2, ProjID, dmg, 10f);
				player.NewProj(player.Center, Velocity - vertical * 2, ProjID, dmg, 10f);
			}
			if (lvl > 3)
			{
				player.NewProj(player.Center, Velocity + vertical * 1, ProjID, dmg, 10f);
				player.NewProj(player.Center, Velocity - vertical * 1, ProjID, dmg, 10f);
			}
			player.NewProj(player.Center, Velocity, ProjID, dmg, 10f);
		}
		#endregion
	}
}
