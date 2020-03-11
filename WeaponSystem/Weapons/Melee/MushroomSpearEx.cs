using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starvers.WeaponSystem.Weapons.Melee
{
	using TShockAPI;
	using IID = Terraria.ID.ItemID;
	using PID = Terraria.ID.ProjectileID;
	using Vector = TOFOUT.Terraria.Server.Vector2;
	public class MushroomSpearEx : Weapon
	{
		#region Ctor
		public MushroomSpearEx() : base(2, IID.MushroomSpear, PID.InfluxWaver, CareerType.Melee, 612)
		{
			CatchID = PID.MushroomSpear;
		}
		#endregion
		#region UseWeapon
		public override void UseWeapon(StarverPlayer player, Vector Velocity, int lvl, GetDataHandlers.NewProjectileEventArgs args)
		{
			int maxCount = 10 + lvl / 10;
			int count = 0;
			foreach (var npc in Terraria.Main.npc)
			{
				if (count > maxCount)
				{
					break;
				}
				if (!npc.active || npc.friendly)
				{
					continue;
				}
				if (Microsoft.Xna.Framework.Vector2.Distance(player.Center, npc.Center) < 16 * 90)
				{
					count++;
					player.ProjCircle(npc.Center, 16 * 10, 13, ProjID, 6, CalcDamage(lvl));
				}
			}
			if (player.TPlayer.hostile)
			{
				foreach (var ply in Starver.Players)
				{
					if (count > maxCount)
					{
						break;
					}
					if (ply is null || !ply.Active)
					{
						continue;
					}
					if (ply.TPlayer.hostile && (player.TPlayer.team != ply.TPlayer.team || ply.TPlayer.team == 1 || ply.TPlayer.team == 0))
					{
						if (Microsoft.Xna.Framework.Vector2.Distance(player.Center, ply.Center) < 16 * 90)
						{
							count++;
							player.ProjCircle(ply.Center, 16 * 10, 13, ProjID, 6, CalcDamage(lvl));
						}
					}
				}
			}
		}
		#endregion
	}
}
